using BlazorMauiApp1.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using Microsoft.Extensions.Options;
using System.Text;

namespace BlazorMauiApp1.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> options)
        {
            _settings = options.Value;
        }

        public async Task SendWeatherReportAsync(string recipientEmail, string recipientName, WeatherForecastApiResponse weatherData)
        {
            if (weatherData?.Location == null || weatherData.Forecast?.Forecastday == null)
                throw new ArgumentException("Invalid weather data provided for the email report.");

            try
            {
                Console.WriteLine($"[DEBUG] Host: {_settings.SmtpHost}");
                Console.WriteLine($"[DEBUG] Port: {_settings.SmtpPort}");
                Console.WriteLine($"[DEBUG] User: {_settings.SmtpUser}");
                Console.WriteLine($"[DEBUG] Pass: {(string.IsNullOrEmpty(_settings.SmtpPass) ? "NOT SET" : "SET")}");
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
                message.To.Add(new MailboxAddress(recipientName, recipientEmail));
                message.Subject = $"Your Weather Forecast for {weatherData.Location.Name}";

                message.Body = new TextPart(TextFormat.Html)
                {
                    Text = GenerateHtmlBody(weatherData, recipientName)
                };

                using var client = new SmtpClient();
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                Console.WriteLine("Connecting to SMTP server...");
                await client.ConnectAsync("smtp-relay.brevo.com", 587, SecureSocketOptions.StartTls);

                Console.WriteLine("Authenticating SMTP...");
                await client.AuthenticateAsync(_settings.SmtpUser, _settings.SmtpPass);

                Console.WriteLine($"Sending email to {recipientEmail}...");
                await client.SendAsync(message);

                await client.DisconnectAsync(true);
                Console.WriteLine("Email sent successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email sending failed: {ex.Message}");
                throw;
            }
        }

        private string GenerateHtmlBody(WeatherForecastApiResponse forecast, string recipientName)
        {
            var sb = new StringBuilder();
            sb.Append($@"
                <body style='font-family: Arial, sans-serif; color: #333; line-height: 1.6;'>
                    <div style='max-width: 600px; margin: auto; padding: 20px; border: 1px solid #ddd;'>
                        <h2 style='color: #0D8ABC;'>Hi {recipientName},</h2>
                        <p>Here is your requested weather forecast for <strong>{forecast.Location.Name}, {forecast.Location.Region}</strong>.</p>
                        <h3>Current Conditions</h3>
                        <p style='margin-left: 20px;'>
                            <strong>Temperature:</strong> {forecast.Current.TempC}°C / {forecast.Current.TempF}°F<br/>
                            <strong>Condition:</strong> {forecast.Current.Condition.Text}
                        </p>
                        <h3>7-Day Forecast</h3>
                        <table style='width: 100%; border-collapse: collapse;'>
                            <tr style='background-color: #f2f2f2;'>
                                <th style='padding: 8px; border: 1px solid #ddd; text-align: left;'>Date</th>
                                <th style='padding: 8px; border: 1px solid #ddd; text-align: left;'>Condition</th>
                                <th style='padding: 8px; border: 1px solid #ddd; text-align: left;'>Max Temp</th>
                                <th style='padding: 8px; border: 1px solid #ddd; text-align: left;'>Min Temp</th>
                            </tr>");

            foreach (var day in forecast.Forecast.Forecastday)
            {
                sb.Append($@"
                    <tr>
                        <td style='padding: 8px; border: 1px solid #ddd;'>{day.Date:MMM dd, dddd}</td>
                        <td style='padding: 8px; border: 1px solid #ddd;'>{day.Day.Condition.Text}</td>
                        <td style='padding: 8px; border: 1px solid #ddd;'>{day.Day.MaxTempC}°C</td>
                        <td style='padding: 8px; border: 1px solid #ddd;'>{day.Day.MinTempC}°C</td>
                    </tr>");
            }

            sb.Append("</table><p style='font-size: 0.8em; color: #777; margin-top: 20px;'>Have a great day!</p></div></body>");
            return sb.ToString();
        }
    }
}
