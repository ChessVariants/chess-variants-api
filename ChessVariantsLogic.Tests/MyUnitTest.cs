using Xunit;

namespace ChessVariantsLogic.Tests;

public class MyUnitTest
{
    [Fact]
    public void Add_2Plus2_Returns4()
    {
        int result = MyCalculator.Add(2, 2);
        Assert.Equal(4, result);
    }
}
