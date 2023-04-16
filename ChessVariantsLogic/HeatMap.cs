namespace ChessVariantsLogic;

public class HeatMap
{
    private int rows;
    private int cols;

    private double[,] heatMap;

    public HeatMap(int Rows, int Cols)
    {
        rows = Rows;
        cols = Cols;
        heatMap = new double[Rows,Cols];
        initHeatMap(2);
        printMatrix(heatMap);
    }


    private void initHeatMap(double weight)
    {
        int rowabc = rows/4;
        int colabc = cols/4;
        int min = Math.Min(rows,cols);
     
        double value = weight /4;
        int index = 1;
        while (index <=4)
        {
            for (int row = index; row < rows; row++)
            {
                for (int col = index; col < cols; col++)
                {
                    if (col >= index && col <= cols - (index + 2) &&(row >= index && row <= rows - (index + 2)))
                    {
                        heatMap[row, col] += value;
                    }
                    
                }
            }
            index++;
        }
        
    }

    public double GetValue(int row, int col)
    {
        return heatMap[row, col];
    }

   
        static void printMatrix(double[,] m)
        {
            for(int i = 0; i < m.GetLength(0); i++){
                for(int j = 0; j < m.GetLength(1); j++){
                    Console.Write("{0} ", m[i,j]);
                }
                Console.WriteLine();
            }
        }

    
}