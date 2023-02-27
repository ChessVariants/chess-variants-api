namespace ChessVariantsLogic;

public interface IMovementPattern
{
    List<Tuple<int, int>> Movement
    {
        get;
    }
    
    List<Tuple<int, int>> MoveLength
    {
        get;
    }

    void AddPattern(Tuple<int, int> direction);

    bool RemovePattern(Tuple<int, int> direction);

}