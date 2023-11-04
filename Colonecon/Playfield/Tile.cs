using System.Numerics;
using System.Runtime.CompilerServices;

public class Tile
{
    public int MiraStartDeposit { get; private set; }
    public int MiraCurrentDeposit { get; private set; }

    public Tile(int miraDeposit)
    {
        MiraStartDeposit = miraDeposit;
        MiraCurrentDeposit = miraDeposit;
    }
}
