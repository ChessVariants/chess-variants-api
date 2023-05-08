using DataAccess.MongoDB.Models;
using MongoDB.Driver;

namespace DataAccess.MongoDB;
public class ChessboardRepository : GenericRepository<Chessboard>
{
    public const string CollectionName = "Chessboards";

    public ChessboardRepository(IMongoDatabase database) : base(database.GetCollection<Chessboard>(CollectionName)) {}

    public async Task<List<Chessboard>> GetByUserAsync(string username)
    {
        return await _collection.Find(p => p.Creator.Equals(username)).ToListAsync();
    }

    public async Task<Chessboard> GetByUserAndBoardNameAsync(string username, string boardName)
    {
        return await _collection.Find(p => p.Creator.Equals(username) && p.Name.Equals(boardName)).FirstAsync();
    }

    
}
