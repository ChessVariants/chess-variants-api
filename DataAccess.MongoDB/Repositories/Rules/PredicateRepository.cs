using DataAccess.MongoDB.Models;
using MongoDB.Driver;

namespace DataAccess.MongoDB.Repositories;
public class PredicateRepository : GenericRepository<Predicate>
{
    public const string CollectionName = "Predicates";

    public PredicateRepository(IMongoDatabase database) : base(database.GetCollection<Predicate>(CollectionName))
    {
    }
}
