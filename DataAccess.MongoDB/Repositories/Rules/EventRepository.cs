using DataAccess.MongoDB.Models;
using MongoDB.Driver;

namespace DataAccess.MongoDB.Repositories;
public class EventRepository : GenericRepository<EventModel>
{
    public const string CollectionName = "Events";

    public EventRepository(IMongoDatabase database) : base(database.GetCollection<EventModel>(CollectionName))
    {
    }
}
