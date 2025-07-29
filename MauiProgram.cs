using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using CommunityToolkit.Maui;
using BlazorMauiApp1.Services;
using Microsoft.Extensions.Configuration;
using BlazorMauiApp1.Models;
using System.Reflection;

namespace BlazorMauiApp1
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .UseMauiCommunityToolkitMediaElement()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            // --- Load embedded appsettings.Development.json ---
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "BlazorMauiApp1.appsettings.Development.json"; // Make sure this matches actual resource name!

            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
            {
                Console.WriteLine("[ERROR] Config resource not found. Available:");
                foreach (var res in assembly.GetManifestResourceNames())
                {
                    Console.WriteLine("  -> " + res);
                }
            }
            else
            {
                var config = new ConfigurationBuilder()
                    .AddJsonStream(stream)
                    .Build();
                builder.Configuration.AddConfiguration(config);
                builder.Services.Configure<EmailSettings>(config.GetSection("EmailSettings"));
            }

            // Services
            builder.Services.AddMauiBlazorWebView();
            builder.Services.AddSingleton<IMauiNavigationService, MauiNavigationService>();
            builder.Services.AddSingleton<MongoUserService>();
            builder.Services.AddSingleton<UserFavoriteCitiesService>();
            builder.Services.AddSingleton<IAuthService, AuthService>();
            builder.Services.AddSingleton<IEmailService, EmailService>();
            builder.Services.AddSingleton<App>();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

#if ANDROID
            builder.Services.AddSingleton<IAudioFileService, AudioFileService>();
#endif

            return builder.Build();
        }
    }
}
