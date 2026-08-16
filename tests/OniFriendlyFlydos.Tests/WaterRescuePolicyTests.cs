using Xunit;

namespace OniFriendlyFlydos.Tests;

public sealed class WaterRescuePolicyTests
{
    [Fact]
    public void RequestsRescueForLivingUnstoredSubmergedFlydo()
    {
        Assert.True(WaterRescuePolicy.ShouldRequestRescue(
            isSubstantialLiquid: true,
            isDead: false,
            isStored: false));
    }

    [Theory]
    [InlineData(false, false, false)]
    [InlineData(true, true, false)]
    [InlineData(true, false, true)]
    public void RejectsRescueOutsideLivingUnstoredSubmergedState(
        bool isSubstantialLiquid,
        bool isDead,
        bool isStored)
    {
        Assert.False(WaterRescuePolicy.ShouldRequestRescue(
            isSubstantialLiquid,
            isDead,
            isStored));
    }

    [Theory]
    [InlineData(true, false, false, true)]
    [InlineData(false, true, true, true)]
    [InlineData(false, true, false, false)]
    [InlineData(false, false, true, false)]
    public void AllowsVanillaMovesOrAutomaticFlydoRescue(
        bool vanillaAllowsMove,
        bool isFlydo,
        bool rescueRequested,
        bool expected)
    {
        Assert.Equal(
            expected,
            WaterRescuePolicy.ShouldAllowDuplicantMove(
                vanillaAllowsMove,
                isFlydo,
                rescueRequested));
    }

    [Theory]
    [InlineData(true, true, true, true)]
    [InlineData(true, true, false, false)]
    [InlineData(true, false, true, false)]
    [InlineData(false, true, true, false)]
    public void ClearsOnlyCompleteAutomaticMoveState(
        bool rescueRequested,
        bool isMarkedForMove,
        bool hasStorageProxy,
        bool expected)
    {
        Assert.Equal(
            expected,
            WaterRescuePolicy.ShouldClearAutomaticMove(
                rescueRequested,
                isMarkedForMove,
                hasStorageProxy));
    }

    [Fact]
    public void SearchOffsetsAreNearestFirstUniqueAndBounded()
    {
        var offsets = WaterRescuePolicy.CreateSearchOffsets(maxRadius: 4);

        Assert.Equal(40, offsets.Length);
        Assert.Equal(1, offsets[0].Distance);
        Assert.Equal(0, offsets[0].X);
        Assert.Equal(1, offsets[0].Y);
        Assert.Equal(
            offsets.Length,
            offsets.Select(offset => (offset.X, offset.Y)).Distinct().Count());
        Assert.True(offsets
            .Zip(offsets.Skip(1), (left, right) => left.Distance <= right.Distance)
            .All(value => value));
        Assert.All(offsets, offset => Assert.InRange(offset.Distance, 1, 4));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void EmptySearchForNonPositiveRadius(int maxRadius)
    {
        Assert.Empty(WaterRescuePolicy.CreateSearchOffsets(maxRadius));
    }
}
