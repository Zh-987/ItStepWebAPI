using ITSTEPASPNET.Data;
using ITSTEPASPNET.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
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
            
            if(weather.TemperatureC > 0 && weather.TemperatureC < 15)
            {
                weather.Summary = Summaries[4]; 
            }
            else if (weather.TemperatureC >= 16 && weather.TemperatureC <= 25)
            {
                weather.Summary = Summaries[5];
            }
            else if (weather.TemperatureC >= 26 && weather.TemperatureC <= 40)
            {
                weather.Summary = Summaries[7];
            }
            else
            {
                weather.Summary = Summaries[0];
            }


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

        [HttpPut("Id")]
        /*[ProducesResponseType(StatusCodes.Status204NoContent)]*/
        public async Task<ActionResult> Put(Guid Id, WeatherForecast weather)
        {
            if (weather.TemperatureC > 0 && weather.TemperatureC < 15)
            {
                weather.Summary = Summaries[4];
            }
            else if (weather.TemperatureC >= 16 && weather.TemperatureC <= 25)
            {
                weather.Summary = Summaries[5];
            }
            else if (weather.TemperatureC >= 26 && weather.TemperatureC <= 40)
            {
                weather.Summary = Summaries[7];
            }
            else
            {
                weather.Summary = Summaries[0];
            }


            if (Id == weather.Id)
            {

                var weatherObject = _dbConext.WeatherForecasts.FindAsync(Id);

                try
                {
                    if (await weatherObject != null)
                    {
                        /*_dbConext.Entry(weather).State = W *//* EntityState.Modified;*/
                        /*weatherObject = weather;*/
                        _dbConext.Entry(weatherObject).State = EntityState.Modified;
                        await _dbConext.SaveChangesAsync();
                    }
                    else
                    {
                        _dbConext.WeatherForecasts.Add(weather);
                    }

                    await _dbConext.SaveChangesAsync();
                }

                catch (DbUpdateConcurrencyException)
                {
                   if (Id != weather.Id)
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }

                }
                
            }
            else
            {
                return BadRequest();
            }

            return NoContent();
        }

        [HttpPatch("Id")]
        public async Task<ActionResult> Patch(Guid Id, WeatherForecast weather)
        {
            var weatherID = await _dbConext.WeatherForecasts.SingleAsync(x => x.Id == Id);

                if (weather.TemperatureC > 0 && weather.TemperatureC < 15)
                {
                    weather.Summary = Summaries[4];
                }
                else if (weather.TemperatureC >= 16 && weather.TemperatureC <= 25)
                {
                    weather.Summary = Summaries[5];
                }
                else if (weather.TemperatureC >= 26 && weather.TemperatureC <= 40)
                {
                    weather.Summary = Summaries[7];
                }
                else
                {
                    weather.Summary = Summaries[0];
                }

            weatherID.Date = weather.Date;
            weatherID.TemperatureC = weather.TemperatureC;
            weatherID.Summary = weather.Summary;

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