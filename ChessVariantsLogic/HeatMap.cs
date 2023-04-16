namespace ChessVariantsLogic;

public class HeatMap
{
    private int rows;
    private int cols;

    private int[,] heatMap;

    public HeatMap(int Rows, int Cols)
    {
        rows = Rows;
        cols = Cols;
        heatMap = new int[Rows,Cols];
        initHeatMap(4);
    }


    private void initHeatMap(int weight)
    {
        int rowabc = rows/4;
        int colabc = cols/4;
        int min = Math.Min(rows,cols);
      
        int test = 1;
        int index = 0;
        while (index != 4)
        {
            for (int row = index; row < rows; row++)
            {
                for (int col = index; col < cols; col++)
                {
                    if (col >= index && col <= cols - (index + 1))
                    {
                        heatMap[row, col]++;
                    }
                    if (row >= index && row <= rows - (index + 1))
                    {
                        heatMap[row, col]++;
                    }
                }
            }
            index++;
        }
        
    }

    public int GetValue(int row, int col)
    {
        return heatMap[row, col];
    }

   

    private int[,] heatMap1234 = new int[,]   {{1,1,1,1,1,1,1,1},
                                           {1,2,2,2,2,2,2,1},
                                           {1,2,3,3,3,3,2,1},
                                           {1,2,3,4,4,3,2,1},
                                           {1,2,3,4,4,3,2,1},
                                           {1,2,3,3,3,3,2,1},
                                           {1,2,2,2,2,2,2,1},
                                           {1,1,1,1,1,1,1,1}};
    private int[,] heatMap2 = new int[,]  {{-2,-2,-2,-2,-2,-2,-2,-2},
                                           {-2,-1,-1,-1,-1,-1,-1,-2},
                                           {-2,-1,0,0,0,0,-1,-2},
                                           {-2,-1,0,1,1,0,-1,-2},
                                           {-2,-1,0,1,1,0,-1,-2},
                                           {-2,-1,0,0,0,0,-1,-2},
                                           {-2,-1,-1,-1,-1,-1,-1,-2},
                                           {-2,-2,-2,-2,-2,-2,-2,-2}};
}