using Euromonitor.Dto;
using Euromonitor.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Euromonitor.Services
{
    public class SubscriptionsService
    {
        private readonly DatabaseContext _context;
        public SubscriptionsService(DatabaseContext context)
        {
            _context = context;
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

        public async Task Subscribe(int userId, Subscription subscriptionPostDto)
        {
            Subscription subscriptionExists = await _context.Subscriptions
                .Where(s => s.BookID == subscriptionPostDto.BookID && s.UserID == userId)
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
                UserID = userId,
                Active = true
            };

            _context.Subscriptions.Add(subscription);
            await _context.SaveChangesAsync();
        }

        public async Task UnSubscribe(int userId, Subscription subscriptionPostDto)
        {
            Subscription subscription = await _context.Subscriptions
                .Where(s => s.BookID == subscriptionPostDto.BookID && s.UserID == userId && s.Active == true)
                .FirstOrDefaultAsync();

            if (subscription == null)
            {
                throw new Exception("No book subscription for this user found.");
            }

            subscription.Active = false;

            await _context.SaveChangesAsync();
        }
    }
}
