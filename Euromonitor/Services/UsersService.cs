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
    public class UsersService
    {
        private readonly DatabaseContext _context;
        public UsersService(DatabaseContext context)
        {
            _context = context;
        }

        public async Task RegisterUser(UserPostDto user)
        {
            Random random = new Random();
            string salt = Encryt((random.Next(1, 999999) + random.Next(1, 999999)).ToString());
            string hash = Encryt(user.Password + salt);

            User userCreate = new User()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                EmailAddress = user.EmailAddress,
                Hash = hash,
                Salt = salt
            };

            _context.Users.Add(userCreate);
            await _context.SaveChangesAsync();
        }

        public async Task<UserViewDto> Login(UserPostDto user)
        {
            User record = await _context.Users.Where(u => u.EmailAddress == user.EmailAddress)
                   .FirstOrDefaultAsync();

            if (record == null)
            {
                throw new Exception("Cannot find a user using the email address entered");
            }

            using (SHA256CryptoServiceProvider sha256 = new SHA256CryptoServiceProvider())
            {
                UTF8Encoding utf8 = new UTF8Encoding();
                byte[] data = sha256.ComputeHash(utf8.GetBytes(user.Password + record.Salt));
                var result = Convert.ToBase64String(data);

                if (result != record.Hash)
                {
                    throw new Exception("Incorrect password or email address");
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
                    new Claim("Id", record.ID.ToString()),
                    new Claim("EmaillAddress", record.EmailAddress.ToString()),
                    new Claim("Firstname", record.FirstName.ToString()),
                    new Claim("Lastname", record.LastName.ToString()),
                    new Claim("UserType", "System")
                }),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var newToken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(newToken);

            UserViewDto response = new UserViewDto()
            {
                Token = token
            };

            return response;
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
