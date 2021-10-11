using Catalog.API.Data;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Catalog.API.Entities
{
    // 3. Create context class
    public class CatalogContext : ICatalogContext
    {
        // use dependency injection to inject congifuration, 
        // which can access DatabaseSettings in step 1, and get connection string
        public CatalogContext(IConfiguration configuration)
        {
            // MongoClient provided by Mongo Driver, provide connection to mongo database
            // DatabaseSettings:ConnectionString is the location of value
            var client = new MongoClient(configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
            // get database, if don't have database, create a db for us
            // if have db, pass the database name
            var database = client.GetDatabase(configuration.GetValue<string>("DatabaseSettings:DatabaseName"));

            // populate collections, Products here is the property inherited from ICatalogContext
            Products = database.GetCollection<Product>(configuration.GetValue<string>("DatabaseSettings:CollectionName"));

            // 4. seed the database
            CatalogContextSeed.SeedData(Products);
        }

        public IMongoCollection<Product> Products { get; }

    }
}
