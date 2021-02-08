using Euromonitor.Dto;
using Euromonitor.Models;
using Euromonitor.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Euromonitor.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ResellersController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private ResellersService _resellersService;

        public ResellersController(DatabaseContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<ResellerPostDto>> Register(ResellerPostDto reseller)
        {
            try
            {
                _resellersService = new ResellersService(_context);
                await _resellersService.Register(reseller);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> GenerateResellerToken(ResellerPostDto resellerDto)
        {
            try
            {
                _resellersService = new ResellersService(_context);
                return Ok(JsonConvert.SerializeObject(await _resellersService.GenerateResellerToken(resellerDto)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<BookViewDto>>> GetBooksListForUser(int userId)
        {
            try
            {
                _resellersService = new ResellersService(_context);
                return await _resellersService.GetBooksListForUser(userId);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookViewDto>>> GetBooksList()
        {
            try
            {
                _resellersService = new ResellersService(_context);
                return await _resellersService.GetBooksList();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<BookViewDto>>> GetSubscriptionsForUser(int userId)
        {
            try
            {
                _resellersService = new ResellersService(_context);
                return await _resellersService.GetSubscriptionsForUser(userId);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<SubscriptionPostDto>> SubscribeUser(Subscription subscriptionPostDto)
        {
            try
            {
                _resellersService = new ResellersService(_context);
                await _resellersService.SubscribeUser(subscriptionPostDto);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<SubscriptionPostDto>> UnSubscribe(Subscription subscriptionPostDto)
        {
            try
            {
                _resellersService = new ResellersService(_context);
                await _resellersService.UnSubscribe(subscriptionPostDto);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
