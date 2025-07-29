using BlazorMauiApp1.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq; // Add this using directive
using System.Threading.Tasks;

namespace BlazorMauiApp1.Services
{
    public class UserFavoriteCitiesService
    {
        private readonly IMongoCollection<UserFavoriteCity> _favoriteCitiesCollection;

        public UserFavoriteCitiesService()
        {
            try
            {
                var connectionString = "mongodb://10.0.2.2:27017";
                //var connectionString = "mongodb+srv://priyan:Priyan%402004@cluster0.eomizaj.mongodb.net/weather_app?retryWrites=true&w=majority";
                var settings = MongoClientSettings.FromConnectionString(connectionString);
                settings.ServerSelectionTimeout = TimeSpan.FromSeconds(5);
                var client = new MongoClient(settings);
                var database = client.GetDatabase("weather_app");
                _favoriteCitiesCollection = database.GetCollection<UserFavoriteCity>("user_favorite_cities");
                Console.WriteLine("Successfully connected to MongoDB for UserFavoriteCitiesService.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error connecting to MongoDB in UserFavoriteCitiesService: {ex.Message}");
                _favoriteCitiesCollection = null;
            }
        }

        // Method to get favorite cities for a user
        public async Task<List<string>> GetFavoriteCitiesAsync(string userId)
        {
            if (_favoriteCitiesCollection == null)
            {
                Console.WriteLine("MongoDB connection not established. Cannot get favorite cities.");
                return new List<string>();
            }

            var filter = Builders<UserFavoriteCity>.Filter.Eq(u => u.UserId, userId);
            var userFavorites = await _favoriteCitiesCollection.Find(filter).FirstOrDefaultAsync();
            
            // Return sorted list or an empty list if none found
            return userFavorites?.FavoriteCities?.OrderBy(c => c).ToList() ?? new List<string>();
        }

        // Method to add favorite cities for a user
        public async Task AddFavoriteCitiesAsync(string userId, List<string> newCities)
        {
            if (_favoriteCitiesCollection == null)
            {
                Console.WriteLine("MongoDB connection not established. Skipping favorite cities update.");
                return;
            }

            var filter = Builders<UserFavoriteCity>.Filter.Eq(u => u.UserId, userId);
            var update = Builders<UserFavoriteCity>.Update.AddToSetEach(u => u.FavoriteCities, newCities);
            var options = new UpdateOptions { IsUpsert = true };

            await _favoriteCitiesCollection.UpdateOneAsync(filter, update, options);
        }
    }
}