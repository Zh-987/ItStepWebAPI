using ITSTEPASPNET.Model;
using Microsoft.EntityFrameworkCore;

namespace ITSTEPASPNET.Data
{
    public class WeatherForecastContext : DbContext
    {

        public WeatherForecastContext(DbContextOptions<WeatherForecastContext> options) : base(options)
        {
        }

        public DbSet<Model.WeatherForecast> WeatherForecasts { get; set; }

    }
}
