using DataAccess.MongoDB.Models;
using MongoDB.Driver;

namespace DataAccess.MongoDB;
public class PieceRepository : GenericRepository<Piece>
{
    public const string CollectionName = "Pieces";

    public PieceRepository(IMongoDatabase database) : base(database.GetCollection<Piece>(CollectionName)) {}

    public async Task<List<Piece>> GetByUserAsync(string username)
    {
        return await _collection.Find(p => p.Creator.Equals(username)).ToListAsync();
    }

    public async Task<Piece> GetByUserAndPieceNameAndColorAsync(string username, string pieceName, string color)
    {
        return await _collection.Find(p => p.Creator.Equals(username) && p.Name.Equals(pieceName) && p.BelongsTo.Equals(color)).FirstAsync();
    }
    
    public async Task<List<Piece>> GetStandardPieces()
    {
        var username = "Guest-f004d09b-b95c-4a3e-b83b-b0a0fe29cbf7";
        return await _collection.Find(p => p.Creator.Equals(username)).ToListAsync();
    }
    
}
