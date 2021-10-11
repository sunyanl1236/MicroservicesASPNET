using MongoDB.Driver;

namespace Catalog.API.Entities
{
    // 2. Create context interface
    // Data folder is Data layer
    public interface ICatalogContext
    {
        IMongoCollection<Product> Products { get; }
    }
}
