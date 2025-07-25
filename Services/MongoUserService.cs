using BlazorMauiApp1.Models;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace BlazorMauiApp1.Services
{
    public class MongoUserService
    {
        private readonly IMongoCollection<MongoUser> _users;
        private readonly IMongoCollection<City> _cities;
        private readonly IMongoCollection<UserWeatherSelection> _userWeatherSelections;

        public MongoUserService()
        {
            try
            {
                var connectionString = "mongodb://10.0.2.2:27017";
                var settings = MongoClientSettings.FromConnectionString(connectionString);
                settings.ServerSelectionTimeout = TimeSpan.FromSeconds(5); 
                var client = new MongoClient(settings);
                var database = client.GetDatabase("weather_app");
                _users = database.GetCollection<MongoUser>("users");
                _cities = database.GetCollection<City>("cities");
                _userWeatherSelections = database.GetCollection<UserWeatherSelection>("user_weather_selections");
                Console.WriteLine("Successfully connected to MongoDB.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error connecting to MongoDB: {ex.Message}");
               
                _users = null; 
                _cities = null;
                _userWeatherSelections = null;
            }
        }

        public async Task UpsertUserAsync(MongoUser user)
        {
            if (_users == null)
            {
                Console.WriteLine("MongoDB connection not established. Skipping user upsert.");
                return;
            }
            var filter = Builders<MongoUser>.Filter.Eq(u => u.Email, user.Email);
            var update = Builders<MongoUser>.Update
                .Set(u => u.Name, user.Name)
                .Set(u => u.LastSignInAt, user.LastSignInAt)
                .Set(u => u.Provider, user.Provider)
                .Set(u => u.AvatarUrl, user.AvatarUrl)
                .SetOnInsert(u => u.CreatedAt, user.CreatedAt);

            await _users.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = true });
        }

        public async Task<List<City>> GetCitiesAsync()
        {
            if (_cities == null)
            {
                Console.WriteLine("MongoDB connection not established. Skipping city fetch.");
                return new List<City>();
            }
            return await _cities.Find(_ => true).ToListAsync();
        }

        public async Task AddUserWeatherSelectionAsync(UserWeatherSelection selection)
        {
            if (_userWeatherSelections == null)
            {
                Console.WriteLine("MongoDB connection not established. Skipping user weather selection insert.");
                return;
            }
            await _userWeatherSelections.InsertOneAsync(selection);
        }
    }
}