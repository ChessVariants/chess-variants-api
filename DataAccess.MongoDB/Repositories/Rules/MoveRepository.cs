using DataAccess.MongoDB.Models;
using MongoDB.Driver;

namespace DataAccess.MongoDB.Repositories;
public class MoveRepository : GenericRepository<MoveTemplate>
{
    public const string CollectionName = "Moves";

    public MoveRepository(IMongoDatabase database) : base(database.GetCollection<MoveTemplate>(CollectionName))
    {
    }
}
