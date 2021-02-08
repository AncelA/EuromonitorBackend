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
    public class SubscriptionsController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private SubscriptionsService _subscriptionsService;
        public SubscriptionsController(DatabaseContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookViewDto>>> GetSubscriptionsForUser()
        {
            try
            {
                var userId = Int32.Parse(User.FindFirst("ID").Value);
                _subscriptionsService = new SubscriptionsService(_context);
                return await _subscriptionsService.GetSubscriptionsForUser(userId);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<SubscriptionPostDto>> Subscribe(Subscription subscriptionPostDto)
        {
            try
            {
                var userId = Int32.Parse(User.FindFirst("ID").Value);
                _subscriptionsService = new SubscriptionsService(_context);
                await _subscriptionsService.Subscribe(userId, subscriptionPostDto);
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
                var userId = Int32.Parse(User.FindFirst("ID").Value);
                _subscriptionsService = new SubscriptionsService(_context);
                await _subscriptionsService.UnSubscribe(userId, subscriptionPostDto);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
