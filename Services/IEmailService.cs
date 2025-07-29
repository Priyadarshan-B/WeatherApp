using BlazorMauiApp1.Models;
using System.Threading.Tasks;

namespace BlazorMauiApp1.Services
{
    public interface IEmailService
    {
        Task SendWeatherReportAsync(string recipientEmail, string recipientName, WeatherForecastApiResponse weatherData);
    }
}