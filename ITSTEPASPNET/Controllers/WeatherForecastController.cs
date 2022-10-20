using ITSTEPASPNET.Data;
using ITSTEPASPNET.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.AccessControl;

namespace ITSTEPASPNET.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly WeatherForecastContext _dbConext;
        public WeatherForecastController(ILogger<WeatherForecastController> logger, WeatherForecastContext weather)
        {
            _dbConext = weather;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<WeatherForecast>>> Get()
        {
            return await _dbConext.WeatherForecasts.Select( x => WeatherForecastOut(x)).ToListAsync();
        }

        [HttpGet("TemperatureC")]
        public async Task<ActionResult<WeatherForecast>> GetFindTemperature(Guid Id)
        {
            var FindTemp = await _dbConext.WeatherForecasts.FindAsync(Id);
            if (FindTemp == null)
            {
                return NotFound(); 
            }

            return WeatherForecastOut(FindTemp);

        }

        [HttpPost(Name = "PostWeatherForecast")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post(WeatherForecast weather)
        {
            
            _dbConext.WeatherForecasts.Add(weather);
            await _dbConext.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { Date = weather.Date, TemperatureC = weather.TemperatureC, Summary = weather.Summary }, weather);
        }

        [HttpDelete("Id")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(Guid Id)
        {
            var findDeleteObject = await _dbConext.WeatherForecasts.FindAsync(Id);
            if (findDeleteObject == null)
            {
                return NotFound();
            }
            _dbConext.WeatherForecasts.Remove(findDeleteObject);
            await _dbConext.SaveChangesAsync();
            return NoContent();
        }

        private static WeatherForecast WeatherForecastOut(WeatherForecast weather) => new WeatherForecast {
                Id = weather.Id,
                Date = weather.Date,
                TemperatureC = weather.TemperatureC,
                Summary = weather.Summary,
        };
           

    }
}