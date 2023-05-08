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

    public async Task<Piece> GetByUserAndPieceNameAsync(string username, string pieceName)
    {
        return await _collection.Find(p => p.Creator.Equals(username) && p.Name.Equals(pieceName)).FirstAsync();
    }
    
    public async Task<List<Piece>> GetStandardPieces()
    {
        var username = "admin";
        return await _collection.Find(p => p.Creator.Equals(username)).ToListAsync();
    }
    
}
