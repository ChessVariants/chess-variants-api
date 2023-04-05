using DataAccess.MongoDB.Models;
using MongoDB.Driver;

namespace DataAccess.MongoDB;

/// <summary>
/// Repository for operations on <see cref="Variant"/>
/// </summary>
public class VariantRepository : GenericRepository<Variant>
{
    public const string CollectionName = "Variants";

    public VariantRepository(IMongoDatabase database) : base(database.GetCollection<Variant>(CollectionName)) { }

    public async Task<Variant?> GetByIdAsync(string id)
    {
        return await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
    }
    public async Task<Variant?> GetVariantsAsync()
    {
        return await _collection.Find(_ => true).FirstOrDefaultAsync();
    }
}
