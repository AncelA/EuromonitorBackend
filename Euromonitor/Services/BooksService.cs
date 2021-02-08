using Euromonitor.Dto;
using Euromonitor.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Euromonitor.Services
{
    public class BooksService
    {
        private readonly DatabaseContext _context;
        public BooksService(DatabaseContext context)
        {
            _context = context;
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
    }
}
