using AutoMapper;
using FullStackTaskService.Models;
using Newtonsoft.Json;
using Polly;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace FullStackTaskService
{
    public abstract class ApiClient
    {
        protected readonly HttpClient _httpClient;
        protected Uri BaseEndpoint { get; set; }
        protected string ApiKey { get; set; }
        
        public ApiClient(Uri baseEndpoint, string apiKey)
        {
            BaseEndpoint = baseEndpoint ?? throw new ArgumentNullException("baseEndpoint");
            _httpClient = new HttpClient();
            ApiKey = apiKey;
        }


        /// <summary>
        /// Here we send our request, we are using Polly in order to stablish a exponentional back-off policey in case that there was a problem in OpenWeatherMap's side 
        /// </summary>
        /// <param name="requestUrl">Request Url</param>
        /// <returns>A collection of OpenWeatherMap</returns>
        protected async Task<T> GetAsync<T>(Uri requestUrl)
        {
            var response = await Policy.HandleResult<HttpResponseMessage>(message => !message.IsSuccessStatusCode)
                .WaitAndRetryAsync(2, i => TimeSpan.FromSeconds(Math.Pow(2, i)))
                .ExecuteAsync(() => _httpClient.GetAsync(requestUrl, HttpCompletionOption.ResponseHeadersRead));

            if (response.IsSuccessStatusCode)
            {
                response.EnsureSuccessStatusCode();
                var data = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(data);
            }
            else
            {
                throw new Exception((int)response.StatusCode + "-" + response.StatusCode.ToString());
            }
        }


        protected Uri CreateRequestUri(string relativePath, string queryString = "")
        {
            var endpoint = new Uri(BaseEndpoint, relativePath);
            var uriBuilder = new UriBuilder(endpoint)
            {
                Query = queryString
            };
            return uriBuilder.Uri;
        }
    }
}
