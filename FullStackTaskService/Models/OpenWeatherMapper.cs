using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using System.Linq;
using FullStackTaskService.Models;

namespace FullStackTaskService
{
    public class OpenWeatherMapper : Profile
    {
        public OpenWeatherMapper()
        {
            CreateMap<List, OpenWeatherMapDto>()
            .ForMember(d => d.Date, s => s.MapFrom(src => src.dt_txt.ToString("dddd, dd MMMM yyyy")))
            .ForMember(d => d.Humidity, s => s.MapFrom(src => src.Main.Humidity))
            .ForMember(d => d.Temperature, s => s.MapFrom(src => src.Main.Temp.ToString()));
        }
    }
}
