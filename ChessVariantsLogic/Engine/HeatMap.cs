namespace ChessVariantsLogic;

public class HeatMap
{
    private int _rows;
    private int _cols;
    private int _mapWeight = 2;
    private double[,] _heatMap;

    public HeatMap(int Rows, int Cols)
    {
        _rows = Rows;
        _cols = Cols;
        _heatMap = new double[Rows,Cols];
        InitHeatMap(_mapWeight);
    }


    private void InitHeatMap(double weight)
    {
        int min = Math.Min(_rows,_cols);
     
        double value = weight /4;
        int index = 2;
        while (index <=min)
        {
            for (int row = index; row < _rows; row++)
            {
                for (int col = index; col < _cols; col++)
                {
                    if (col >= index && col <= _cols - (index + 1) &&(row >= index && row <= _rows - (index + 1)))
                    {
                        _heatMap[row, col] += value;
                    }
                    
                }
            }
            index++;
        }
        
    }

    public double GetValue(int row, int col)
    {
        return _heatMap[row, col];
    }


    static void PrintMatrix(double[,] m)
    {
        for (int i = 0; i < m.GetLength(0); i++)
        {
            for (int j = 0; j < m.GetLength(1); j++)
            {
                Console.Write("{0} ", m[i, j]);
            }
            Console.WriteLine();
        }
    }

    
}