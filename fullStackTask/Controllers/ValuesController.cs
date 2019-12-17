using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using FullStackTaskService;
using FullStackTaskService.Models;
using System.Text;
using Microsoft.Extensions.Caching.Memory;

namespace FullStackTask.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IWeatherApiManagement _service;
        private readonly IMemoryCache _cache;
        private readonly string fakeIdentity = "FakeIdentity"; 
        public ValuesController(IWeatherApiManagement service, IMemoryCache cache)
        {
            _service = service;
            _cache = cache;
        }

        /// <summary>
        /// GET /api/values
        /// </summary>
        /// <returns>returns list of cities</returns>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _service.GetDrpCityNames();
            return new OkObjectResult(result);
        }

        /// <summary>
        /// GET api/values/queryPhrase (Either City Name or ZIP code)
        /// </summary>
        /// <param name="value">Either City Name or ZIP code</param>
        /// <returns>next 5 days forecast either from cache or service</returns>
        [HttpGet("{value}")]
        public async Task<IActionResult> Get(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return BadRequest("Please select a city or write down a ZIP Code.");
            }
            IEnumerable<OpenWeatherMapDto> res = null;
            var resJsonData = _cache.Get(value);
            var isNumeric = int.TryParse(value, out int n);
            string queryType = "";
            queryType = isNumeric == true ? $"ZIP Code : {value}" : $"City Name : {value}";
            if (resJsonData != null)
            {
                res =  resJsonData as IEnumerable<OpenWeatherMapDto>;
            }
            if (res == null)
            {
                if (isNumeric)
                {
                    res = await _service.GetForecastZipCode(value);
                }
                else
                {
                    res = await _service.GetForecastCity(value);
                }
                _cache.Set(value, res);
            }

            if (res == null)
            {
                return NoContent();
            }

            var result = res.GroupBy(a => a.Date).Select(a => new OpenWeatherMapDto { QueryType= queryType, Date = a.Key,
                Temperature = Math.Round(a.Average(p => p.Temperature),2),Humidity = Math.Round(a.Average(p => p.Humidity),2) });

            #region History_Management
            var temp = _cache.Get(fakeIdentity) as List<OpenWeatherMapDto>;
            if (temp == null)
            {
                _cache.Set(fakeIdentity, result.ToList());
            }
            else
            {
                temp.AddRange(result.ToList());
                _cache.Set(fakeIdentity, temp);
            }
            #endregion

            return new OkObjectResult(result);
        }

        /// <summary>
        /// GET api/values/history
        /// </summary>
        /// <returns>An average of figures for each successful query which has done so far</returns>
        [HttpGet("history")]
        public IActionResult GetHistory()
        {
            if (_cache.Get(fakeIdentity) != null)
            {
                var resJsonData = _cache.Get(fakeIdentity) as List<OpenWeatherMapDto>;
                var result = resJsonData.GroupBy(a => a.QueryType).Select(a => new {
                    QueryType = a.Key,
                    Temperature = Math.Round(a.Average(p => p.Temperature), 2),
                    Humidity = Math.Round(a.Average(p => p.Humidity), 2)
                });
                return new OkObjectResult(result);
            }
            return NoContent();
        }

    }
}
