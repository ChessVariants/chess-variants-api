namespace ChessVariantsLogic;

public class HeatMap
{
    private int rows;
    private int cols;

    public int[,] heatMap;

    public HeatMap(int Rows, int Cols)
    {
        rows = Rows;
        cols = Cols;
        heatMap = new int[Rows,Cols];
        initHeatMap();
    }


    private void initHeatMap()
    {
        int rowabc = rows/4;
        int colabc = cols/4;
        for(int row = 2; row < rows -2; row++)
        {
            for(int col = 2; col < cols -2; col++)
            {
                if(row > rowabc & row < rows - rowabc & col > colabc & col < cols - colabc)
                {
                    heatMap[row,col] = 2;
                }
                else
                {
                    heatMap[row,col] = 1;
                }
            }
        }
    }

    private HeatMap heatMap123 = new HeatMap(8,8);

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