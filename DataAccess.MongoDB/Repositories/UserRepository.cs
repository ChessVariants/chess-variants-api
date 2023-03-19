using DataAccess.MongoDB.Models;
using MongoDB.Driver;

namespace DataAccess.MongoDB;
public class UserRepository : GenericRepository<User>
{
    public const string CollectionName = "Users";

    public UserRepository(IMongoDatabase database) : base(database.GetCollection<User>(CollectionName)) {}

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _collection.Find(x => x.Email == email).FirstOrDefaultAsync();
    }

}
