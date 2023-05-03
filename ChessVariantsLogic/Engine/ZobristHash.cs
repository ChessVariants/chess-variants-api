

public static class ZorbistHash
{
    public static long NextLong(this Random random)
    {
        byte[] buffer = new byte[8];
        random.NextBytes(buffer);
        return BitConverter.ToInt64(buffer, 0);
    }
}