using Euromonitor.Dto;
using Euromonitor.Models;
using Euromonitor.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Euromonitor.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private BooksService _booksService;

        public BooksController(DatabaseContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookViewDto>>> GetBooksListForUser()
        {
            try
            {
                var userId = Int32.Parse(User.FindFirst("ID").Value);
                _booksService = new BooksService(_context);
                return await _booksService.GetBooksListForUser(userId);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
