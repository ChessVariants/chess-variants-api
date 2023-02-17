namespace ChessVariantsLogic;

public static class Constants
{
    public const int MaxBoardWidth = 20;
    public const int MaxBoardHeigth = 20;

    // Maps square notation to corresponding index in Chessboard
    public static Dictionary<string, (int, int)> CoorToIndex = initDictionary();

    private static Dictionary<string, (int,int)> initDictionary()
    {
        var dictionary = new Dictionary<string, (int, int)>();

        string files = "abcdefghijklmnopqrst";

        for(int i = 0; i < MaxBoardHeigth; i++)
        {
            for(int j = 0; j < MaxBoardWidth; j++)
            {
                int rank = i+1;
                string notation = files[j] + rank.ToString();
                dictionary.Add(notation, (i,j));
            }
        }
        return dictionary;
    }

}