using DataAccess.MongoDB.Models;
using MongoDB.Driver;

namespace DataAccess.MongoDB.Repositories;
public class MoveRepository : GenericRepository<MoveTemplateModel>
{
    public const string CollectionName = "Moves";

    public MoveRepository(IMongoDatabase database) : base(database.GetCollection<MoveTemplateModel>(CollectionName))
    {
    }
}
