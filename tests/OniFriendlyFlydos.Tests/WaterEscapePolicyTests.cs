using Xunit;

namespace OniFriendlyFlydos.Tests;

public sealed class WaterEscapePolicyTests
{
    [Fact]
    public void CanSwimVerticallyAndDiagonallyUntilDry()
    {
        var moves = WaterEscapePolicy.CreateMoves();

        Assert.Contains(moves, move => Is(move, 0, 1, 2, WaterEscapeMoveKind.Swim));
        Assert.Contains(moves, move => Is(move, 1, 1, 2, WaterEscapeMoveKind.Swim));
        Assert.Contains(moves, move => Is(move, 0, -1, 10, WaterEscapeMoveKind.Swim));
        Assert.Contains(moves, move => Is(move, 1, -1, 10, WaterEscapeMoveKind.Swim));
    }

    [Fact]
    public void CanLeaveWaterUpwardOrSideways()
    {
        var moves = WaterEscapePolicy.CreateMoves();

        Assert.Contains(moves, move => Is(move, 0, 1, 1, WaterEscapeMoveKind.ExitWater));
        Assert.Contains(moves, move => Is(move, 1, 0, 1, WaterEscapeMoveKind.ExitWater));
    }

    [Fact]
    public void ContainsOnlyTheSixVanillaEscapeMovesBeforeMirroring()
    {
        Assert.Equal(6, WaterEscapePolicy.CreateMoves().Length);
    }

    private static bool Is(
        WaterEscapeMove move,
        int x,
        int y,
        int cost,
        WaterEscapeMoveKind kind)
    {
        return move.X == x && move.Y == y && move.Cost == cost && move.Kind == kind;
    }
}
