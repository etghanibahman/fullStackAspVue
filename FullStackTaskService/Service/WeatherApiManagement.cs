using FullStackTaskService;
using FullStackTaskService.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
//using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using System.IO;
using System.Reflection;

namespace FullStackTaskService
{
    public class WeatherApiManagement : ApiClient,IWeatherApiManagement
    {
        protected Mapper _mapper;
        public WeatherApiManagement(Uri baseEndpoint, string apiKey) : base(baseEndpoint, apiKey)
        {
            var configuration = new MapperConfiguration(cfg => {
                cfg.AddProfile<OpenWeatherMapper>();
            });
            _mapper = new Mapper(configuration);
        }

        /// <summary>
        /// Get List of sample cities in order to fill related dropdown
        /// </summary>
        /// <returns>List of sample cities</returns>
        public async Task<List<string>> GetDrpCityNames()
        {
            var cities = new List<string>();
            try
            {
                string path = Path.Combine(Environment.CurrentDirectory, @"Data\Cities.txt");
                string[] files = await File.ReadAllLinesAsync(path);
                cities = files.Where(a=>!a.Contains("�")).Take(15).ToList();
            }
            catch (Exception e)
            {
                cities = new List<string>() {"Berlin","Stuttgart","Frankfurt","Mannheim","Hamburg","Essen","Duisburg","Munich"};
            }
            return  cities;
        }

        /// <summary>
        /// Get Forecast of next five days for city
        /// </summary>
        /// <param name="city">City Name</param>
        /// <returns>An IEnumerable<OpenWeatherMapDto> which is mapped version of next 5 days' forecast from OpenWeatherMap</returns>
        public async Task<IEnumerable<OpenWeatherMapDto>> GetForecastCity(string city)
        {
            IEnumerable<OpenWeatherMapDto> dest = null;
            try
            {
                var requestUrl = CreateRequestUri("forecast", $"q={city},de&units=metric&appid={ApiKey}");
                var source = await GetAsync<OpenWeatherMap>(requestUrl);
                dest = _mapper.Map<IEnumerable<List>, IEnumerable<OpenWeatherMapDto>>(source.List);
            }
            catch (Exception e)
            {
                string er = e.Message;
            }
            return dest;
        }

        /// <summary>
        /// Get Forecast of next five days for Zip Code
        /// </summary>
        /// <param name="zipCode">Zip Code</param>
        /// <returns>An IEnumerable<OpenWeatherMapDto> which is mapped version of next 5 days' forecast from OpenWeatherMap</returns>
        public async Task<IEnumerable<OpenWeatherMapDto>> GetForecastZipCode(string zipCode)
        {
            IEnumerable<OpenWeatherMapDto> dest = null;
            try
            {
                var requestUrl = CreateRequestUri("forecast", $"zip={zipCode},de&units=metric&appid={ApiKey}");
                var source = await GetAsync<OpenWeatherMap>(requestUrl);
                dest = _mapper.Map<IEnumerable<List>, IEnumerable<OpenWeatherMapDto>>(source.List);
            }
            catch (Exception e)
            {
                string er = e.Message;
            }
            return dest;
        }
    }
}
