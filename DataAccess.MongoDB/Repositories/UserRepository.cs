using DataAccess.MongoDB.Models;
using MongoDB.Driver;

namespace DataAccess.MongoDB;

/// <summary>
/// Repository for operations on <see cref="User"/>
/// </summary>
public class UserRepository : GenericRepository<User>
{
    public const string CollectionName = "Users";

    public UserRepository(IMongoDatabase database) : base(database.GetCollection<User>(CollectionName)) {}

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _collection.Find(x => x.Email == email).FirstOrDefaultAsync();
    }
}
