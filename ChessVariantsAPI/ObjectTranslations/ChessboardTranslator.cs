namespace ChessVariantsAPI.ObjectTranslations;

public static class ChessboardTranslator
{
    public static DataAccess.MongoDB.Models.Chessboard CreateChessboardModel(ChessVariantsLogic.Chessboard chessboard, string name, string creator)
    {
        return new DataAccess.MongoDB.Models.Chessboard
        {
            Name = name,
            Creator = creator,
            Board = translateBoard(chessboard),
            Rows = chessboard.Rows,
            Cols = chessboard.Cols,
        };
    }

    private static List<String> translateBoard(ChessVariantsLogic.Chessboard chessboard)
    {
        var board = new List<string>();
        
        foreach (var pos in chessboard.GetAllCoordinates())
        {
            var piece = chessboard.GetPieceIdentifier(pos.Item1, pos.Item2);
            if (piece == null)
                continue;
            board.Add(piece);
        }
        return board;
    }

}
