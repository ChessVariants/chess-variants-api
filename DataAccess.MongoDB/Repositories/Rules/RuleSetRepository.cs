using DataAccess.MongoDB.Models;
using MongoDB.Driver;

namespace DataAccess.MongoDB.Repositories;
public class RuleSetRepository : GenericRepository<RuleSetModel>
{
    public const string CollectionName = "RuleSets";

    public RuleSetRepository(IMongoDatabase database) : base(database.GetCollection<RuleSetModel>(CollectionName))
    {
    }
}
