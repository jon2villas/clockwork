using System;
using System.Linq;
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
        public IActionResult Get(string timeZoneId)
        {
            var utcTime = DateTime.UtcNow;
            var timeZone = !string.IsNullOrWhiteSpace(timeZoneId)
                ? TimeZoneInfo.FindSystemTimeZoneById(timeZoneId)
                : TimeZoneInfo.Local;
            var serverTime = TimeZoneInfo.ConvertTime(utcTime, timeZone);
            var ip = this.HttpContext.Connection.RemoteIpAddress.ToString();

            var returnVal = new CurrentTimeQuery
            {
                UTCTime = utcTime,
                ClientIp = ip,
                Time = serverTime,
                TimeZone = timeZone.DisplayName
            };

            _context.CurrentTimeQueries.Add(returnVal);
            var count = _context.SaveChanges();
            Console.WriteLine($"{count} records saved to database");

            Console.WriteLine();
            foreach (var currentTimeQuery in _context.CurrentTimeQueries)
            {
                Console.WriteLine($" - {currentTimeQuery.UTCTime}");
            }

            return Ok(returnVal);
        }

        [HttpGet, Route("Queries")]
        public IActionResult GetQueries()
        {
            var currentTimeQueries = _context.CurrentTimeQueries
                .OrderByDescending(ctq => ctq.CurrentTimeQueryId)
                .ToList();

            return Ok(currentTimeQueries);
        }

        [HttpGet, Route("TimeZones")]
        public IActionResult GetTimeZones()
        {
            var timeZones = TimeZoneInfo.GetSystemTimeZones()
                .Select(tz => new { tz.Id, tz.DisplayName });

            return Ok(timeZones);
        }
    }
}
