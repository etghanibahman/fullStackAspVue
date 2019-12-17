using FullStackTask.Presentation.Controllers;
using FullStackTaskService;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using Xunit;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FullStackTaskTest
{
    public class ValuesControllerTest
    {
        
        private readonly string apiBaseUrl = "http://api.openweathermap.org/data/2.5/";
        private readonly string apiKey = "fcadd28326c90c3262054e0e6ca599cd";

        #region Get_WithParameter
        [Fact]
        public void Get_WithParameter_isSuccessfullForCity()
        {
            //Arrange
            Mock<WeatherApiManagement> _service = new Mock<WeatherApiManagement>(new Uri(apiBaseUrl), apiKey);
            var _cache = new MemoryCache(new MemoryCacheOptions());
            ValuesController _controller = new ValuesController(_service.Object, _cache);
            string value = "Munich";

            // Act
            var _sutResponse = _controller.Get(value);

            // Assert
            Assert.IsType<OkObjectResult>(_sutResponse.Result);
            Assert.NotNull(_sutResponse.Result);
        }

        [Fact]
        public void Get_WithParameter_isSuccessfullForZIPCpde()
        {
            //Arrange
            Mock<WeatherApiManagement> _service = new Mock<WeatherApiManagement>(new Uri(apiBaseUrl), apiKey);
            var _cache = new MemoryCache(new MemoryCacheOptions());
            ValuesController _controller = new ValuesController(_service.Object, _cache);
            string value = "10115";

            // Act
            var _sutResponse = _controller.Get(value);

            // Assert
            Assert.IsType<OkObjectResult>(_sutResponse.Result);
            Assert.NotNull(_sutResponse.Result);
        }

        [Fact]
        public void Get_WithParameter_returnsBadRequestForEmptyInput()
        {
            //Arrange
            Mock<WeatherApiManagement> _service = new Mock<WeatherApiManagement>(new Uri(apiBaseUrl), apiKey);
            var _cache = new MemoryCache(new MemoryCacheOptions());
            ValuesController _controller = new ValuesController(_service.Object, _cache);
            string value = "";

            // Act
            var _sutResponse = _controller.Get(value);

            // Assert
            Assert.IsType<BadRequestObjectResult>(_sutResponse.Result);
        }

        [Fact]
        public void Get_WithParameter_returnsNocontentForCitiesWhichAreNotInGermany()
        {
            //Arrange
            Mock<WeatherApiManagement> _service = new Mock<WeatherApiManagement>(new Uri(apiBaseUrl), apiKey);
            var _cache = new MemoryCache(new MemoryCacheOptions());
            ValuesController _controller = new ValuesController(_service.Object, _cache);
            string value = "Tehran";

            // Act
            var _sutResponse = _controller.Get(value);

            // Assert
            Assert.IsType<NoContentResult>(_sutResponse.Result);
        }
        #endregion Get_WithParameter


        #region Get_WithoutParameter
        [Fact]
        public void Get_WithoutParameter_isSuccessfull()
        {
            //Arrange
            Mock<WeatherApiManagement> _service = new Mock<WeatherApiManagement>(new Uri(apiBaseUrl), apiKey);
            var _cache = new MemoryCache(new MemoryCacheOptions());
            ValuesController _controller = new ValuesController(_service.Object, _cache);
            // Act
            var _sutResponse = _controller.Get();
            // Assert
            Assert.IsType<OkObjectResult>(_sutResponse.Result);
            Assert.NotNull(_sutResponse.Result);
        }

        [Fact]
        public void Get_WithoutParameter_OfTypeListOfStrings()
        {
            //Arrange
            Mock<WeatherApiManagement> _service = new Mock<WeatherApiManagement>(new Uri(apiBaseUrl), apiKey);
            var _cache = new MemoryCache(new MemoryCacheOptions());
            ValuesController _controller = new ValuesController(_service.Object, _cache);
            // Act
            var _sutResponse = _controller.Get().Result as OkObjectResult;
            // Assert
            Assert.IsType<List<string>>(_sutResponse.Value);
        }
        #endregion Get_WithoutParameter

        #region GetHistory

        [Fact]
        public void GetHistory_withoutParameter_NocontentAtTheBegining()
        {
            //Arrange
            Mock<WeatherApiManagement> _service = new Mock<WeatherApiManagement>(new Uri(apiBaseUrl), apiKey);
            var _cache = new MemoryCache(new MemoryCacheOptions());
            ValuesController _controller = new ValuesController(_service.Object, _cache);
    
            // Act
            var _sutResponse = _controller.GetHistory();

            // Assert
            Assert.IsType<NoContentResult>(_sutResponse);
        }

        #endregion


    }
}
