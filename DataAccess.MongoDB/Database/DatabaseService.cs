using MongoDB.Driver;

namespace DataAccess.MongoDB;

/// <summary>
/// Abstraction of a connection to a MongoDB database. Only implementations of this class should be used
/// to perform operations on the database.
/// </summary>
public abstract class DatabaseService
{
    private readonly IMongoClient _client;
    private readonly IMongoDatabase _database;
    public readonly UserRepository Users;

    public DatabaseService(string connectionString, string databaseName)
    {
        _client = new MongoClient(connectionString);
        _database = _client.GetDatabase(databaseName);
        Users = new UserRepository(_database);
    }
}
