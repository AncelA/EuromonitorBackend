using Euromonitor.Dto;
using Euromonitor.Models;
using Euromonitor.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Euromonitor.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private UsersService _usersService;

        public UsersController(DatabaseContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<UserPostDto>> RegisterUser(UserPostDto user)
        {
            try
            {
                _usersService = new UsersService(_context);
                await _usersService.RegisterUser(user);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> Login(UserPostDto user)
        {
            try
            {
                _usersService = new UsersService(_context);
                return Ok(JsonConvert.SerializeObject(await _usersService.Login(user)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
