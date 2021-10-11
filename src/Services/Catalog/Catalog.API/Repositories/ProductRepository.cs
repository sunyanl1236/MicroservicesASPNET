using Catalog.API.Entities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Catalog.API.Repositories
{
    public class ProductRepository : IProductRepository
    {
        // dependency injection
        // context provide Products collection and all mongodb CLI methods are included in context
        private readonly ICatalogContext _context;

        public ProductRepository(ICatalogContext context)
        {
            _context = context ?? throw new ArgumentException(nameof(context));
        }

        // should add async here
        // Async programming is a parallel programming technique, 
        // which allows the working process to run separately from 
        // the main application thread. As soon as the work completes, 
        // it informs the main thread about the result, whether it 
        // was successful or not. By using async programming, 
        // we can avoid performance bottlenecks and enhance 
        // the responsiveness of our application.
        public async Task<IEnumerable<Product>> GetProducts()
        {
            // use await here
            return await _context.Products.Find(p => true).ToListAsync();
        }

        public async Task<Product> GetProduct(string id)
        {
            return await _context.Products.Find(p => p.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Product>> GetProdctByName(string name)
        {
            // FilterDefinition creates a filter, provided by MongoDB Driver
            // Builders<Product>.Filter builds simple and complex MongoDB queries
            // Eq(name, BsonValue) Tests that the value of the named element is equal to some value
            // name: The name of the element to test; BsonValue: The value to compare to
            FilterDefinition<Product> filter = Builders<Product>.Filter.Eq(p => p.Name, name);
            return await _context.Products.Find(filter).ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByCategory(string categoryName)
        {
            FilterDefinition<Product> filter = Builders<Product>.Filter.Eq(p => p.Category, categoryName);
            return await _context.Products.Find(filter).ToListAsync();
        }

        // return void
        public async Task CreateProduct(Product product)
        {
            await _context.Products.InsertOneAsync(product);
        }

        public async Task<bool> DeleteProduct(string id)
        {
            FilterDefinition<Product> filter = Builders<Product>.Filter.Eq(p => p.Id, id);
            DeleteResult deleteResult = await _context.Products.DeleteOneAsync(filter);
            return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
        }

        public async Task<bool> UpdateProduct(Product product)
        {
            var updateResult = await _context.Products.ReplaceOneAsync(p => p.Id == product.Id, replacement: product);
            // IsAcknowledged checks wheter update performs or not
            return updateResult.IsAcknowledged && updateResult.ModifiedCount > 0;
        }
    }
}
