using FullStackTaskService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FullStackTaskService
{
    public interface IWeatherApiManagement
    {
        Task<IEnumerable<OpenWeatherMapDto>> GetForecastCity(string city);
        Task<IEnumerable<OpenWeatherMapDto>> GetForecastZipCode(string zipCode);
        Task<List<string>> GetDrpCityNames();
    }
}
