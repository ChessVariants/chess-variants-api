using DataAccess.MongoDB.Models;
using MongoDB.Driver;

namespace DataAccess.MongoDB.Repositories;
public class RuleSetRepository : GenericRepository<RuleSet>
{
    public const string CollectionName = "RuleSets";

    public RuleSetRepository(IMongoDatabase database) : base(database.GetCollection<RuleSet>(CollectionName))
    {
    }
}
