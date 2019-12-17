using System;
using System.Collections.Generic;
using System.Text;

namespace FullStackTaskService.Models
{
    public class OpenWeatherMapDto
    {
        public string QueryType { get; set; }
        public string Date { get; set; }
        public double Temperature { get; set; }
        public double Humidity { get; set; }
    }
}