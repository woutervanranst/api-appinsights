using Microsoft.AspNetCore.Mvc;

namespace WebApi.Bff.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IHttpClientFactory _clientFactory;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IHttpClientFactory clientFactory)
        {
            _logger = logger;
            _clientFactory = clientFactory;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public async IAsyncEnumerable<WeatherForecast> Get()
        {
            _logger.LogInformation("Request entered the BFF");

            //throw new ArgumentOutOfRangeException("I failed on purpose");

            var c = _clientFactory.CreateClient("WebApi2");
            var r = await c.GetAsync("/weatherforecast");

            r.EnsureSuccessStatusCode();

            var forecasts = await r.Content.ReadFromJsonAsync<IEnumerable<WeatherForecast>>();
            foreach (var forecast in forecasts)
                yield return forecast;
        }
    }
}
