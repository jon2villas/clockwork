using System;
using Microsoft.AspNetCore.Mvc;
using Clockwork.API.Models;

namespace Clockwork.API.Controllers
{
    [Route("api/[controller]")]
    public class CurrentTimeController : Controller
    {
        private readonly ClockworkContext _context = null;

        public CurrentTimeController(ClockworkContext context)
        {
            _context = context;
        }

        // GET api/currenttime
        [HttpGet]
        public IActionResult Get()
        {
            var utcTime = DateTime.UtcNow;
            var serverTime = DateTime.Now;
            var ip = this.HttpContext.Connection.RemoteIpAddress.ToString();

            var returnVal = new CurrentTimeQuery
            {
                UTCTime = utcTime,
                ClientIp = ip,
                Time = serverTime
            };

            _context.CurrentTimeQueries.Add(returnVal);
            var count = _context.SaveChanges();
            Console.WriteLine("{0} records saved to database", count);

            Console.WriteLine();
            foreach (var currentTimeQuery in _context.CurrentTimeQueries)
            {
                Console.WriteLine(" - {0}", currentTimeQuery.UTCTime);
            }

            return Ok(returnVal);
        }
    }
}
