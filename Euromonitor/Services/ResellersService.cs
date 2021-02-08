using Euromonitor.Dto;
using Euromonitor.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Euromonitor.Services
{
    public class ResellersService
    {
        private readonly DatabaseContext _context;
        public ResellersService(DatabaseContext context)
        {
            _context = context;
        }

        public async Task Register(ResellerPostDto reseller)
        {
            Random random = new Random();
            string salt = Encryt((random.Next(1, 999999) + random.Next(1, 999999)).ToString());
            string hash = Encryt(reseller.Password + salt);

            Reseller newReseller = new Reseller()
            {
                Name = reseller.Name,
                Hash = hash,
                Salt = salt
            };

            _context.Reseller.Add(newReseller);
            await _context.SaveChangesAsync();
        }

        public async Task<ResellerViewDto> GenerateResellerToken(ResellerPostDto resellerDto)
        {
            Reseller reseller = await _context.Reseller.Where(u => u.Name == resellerDto.Name)
                    .FirstOrDefaultAsync();

            if (reseller == null)
            {
                throw new Exception("Cannot find a reseller with the name '" + resellerDto.Name + "'.");
            }

            using (SHA256CryptoServiceProvider sha256 = new SHA256CryptoServiceProvider())
            {
                UTF8Encoding utf8 = new UTF8Encoding();
                byte[] data = sha256.ComputeHash(utf8.GetBytes(resellerDto.Password + reseller.Salt));
                var result = Convert.ToBase64String(data);

                if (result != reseller.Hash)
                {
                    throw new Exception("Incorrect password or reseller name.");
                }
            }

            var configurationBuilder = new ConfigurationBuilder();
            var path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
            configurationBuilder.AddJsonFile(path, false);

            var root = configurationBuilder.Build();

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(root.GetSection("AppSettings").GetSection("Encrytion").Value);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("Id", reseller.ID.ToString()),
                    new Claim("Name", reseller.Name.ToString()),
                    new Claim("UserType", "Reseller")
                }),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var newToken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(newToken);

            ResellerViewDto response = new ResellerViewDto()
            {
                Token = token
            };

            return response;
        }

        public async Task<List<BookViewDto>> GetBooksListForUser(int userId)
        {
            List<BookViewDto> books = await _context.Books.Select(s => new BookViewDto()
            {
                ID = s.ID,
                Name = s.Name,
                Price = s.Price,
                Text = s.Text,
                Subscribed = false
            })
                .ToListAsync();

            foreach (var book in books)
            {
                book.Subscribed = await _context.Subscriptions.Where(s => s.BookID == book.ID && s.UserID == userId && s.Active == true)
                    .FirstOrDefaultAsync() != null ? true : false;
            }

            return books;
        }

        public async Task<List<BookViewDto>> GetBooksList()
        {
            List<BookViewDto> books = await _context.Books.Select(s => new BookViewDto()
            {
                ID = s.ID,
                Name = s.Name,
                Price = s.Price,
                Text = s.Text
            })
                .ToListAsync();

            return books;
        }

        public async Task<List<BookViewDto>> GetSubscriptionsForUser(int userId)
        {
            List<Subscription> subscriptions = await _context.Subscriptions.Where(s => s.Active == true).ToListAsync();
            List<BookViewDto> results = new List<BookViewDto>();
            foreach (var subscription in subscriptions)
            {
                results.Add(await _context.Books.Where(b => b.ID == subscription.BookID).Select(s => new BookViewDto()
                {
                    ID = s.ID,
                    Name = s.Name,
                    Price = s.Price,
                    Text = s.Text
                }).FirstOrDefaultAsync());
            }
            return results;
        }

        public async Task SubscribeUser(Subscription subscriptionPostDto)
        {
            Subscription subscriptionExists = await _context.Subscriptions
                .Where(s => s.BookID == subscriptionPostDto.BookID && s.UserID == subscriptionPostDto.UserID)
                .FirstOrDefaultAsync();

            if (subscriptionExists != null)
            {
                if (subscriptionExists.Active == true)
                {
                    throw new Exception("User is already subscribed to this book.");
                }
                else
                {
                    subscriptionExists.Active = true;

                    await _context.SaveChangesAsync();
                    return;
                }
            }

            Subscription subscription = new Subscription()
            {
                BookID = subscriptionPostDto.BookID,
                UserID = subscriptionPostDto.UserID,
                Active = true
            };

            _context.Subscriptions.Add(subscription);
            await _context.SaveChangesAsync();
        }

        public async Task UnSubscribe(Subscription subscriptionPostDto)
        {
            Subscription subscription = await _context.Subscriptions
                    .Where(s => s.BookID == subscriptionPostDto.BookID && s.UserID == subscriptionPostDto.UserID && s.Active == true)
                    .FirstOrDefaultAsync();

            if (subscription == null)
            {
                throw new Exception("No book subscription for this user found.");
            }

            subscription.Active = false;

            await _context.SaveChangesAsync();
        }

        private string Encryt(string value)
        {
            using (SHA256CryptoServiceProvider sha256 = new SHA256CryptoServiceProvider())
            {
                UTF8Encoding utf8 = new UTF8Encoding();
                byte[] data = sha256.ComputeHash(utf8.GetBytes(value));
                var result = Convert.ToBase64String(data);
                return result;
            }
        }
    }
}
