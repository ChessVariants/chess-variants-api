namespace ChessVariantsLogic;

public static class GameFactory {

    public static Game StandardChess() {
        return new Game(Chessboard.StandardChessboard(), Player.White, 1);
    }
}