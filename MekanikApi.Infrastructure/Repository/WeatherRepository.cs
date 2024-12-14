using MekanikApi.Domain.Interfaces;

namespace MekanikApi.Domain.Repository
{
    public class WeatherRepository : IWeatherRepository
    {
        public string GetAllWeather()
        {
            return "All weather";
        }
    }
}