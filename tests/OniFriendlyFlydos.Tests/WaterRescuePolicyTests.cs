using Xunit;

namespace OniFriendlyFlydos.Tests;

public sealed class WaterRescuePolicyTests
{
    [Fact]
    public void AllowsCaptureForLivingSubmergedFlydo()
    {
        Assert.True(WaterRescuePolicy.ShouldAllowCapture(
            isSubstantialLiquid: true,
            isDead: false));
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(true, true)]
    [InlineData(false, true)]
    public void RejectsCaptureOutsideLivingSubmergedState(bool isSubstantialLiquid, bool isDead)
    {
        Assert.False(WaterRescuePolicy.ShouldAllowCapture(isSubstantialLiquid, isDead));
    }

    [Theory]
    [InlineData(true, false, false, true)]
    [InlineData(false, true, true, true)]
    [InlineData(false, true, false, false)]
    [InlineData(false, false, true, false)]
    public void AllowsDuplicantMoveForVanillaTargetsOrBaggedFlydos(
        bool vanillaAllowsMove,
        bool isFlydo,
        bool isBagged,
        bool expected)
    {
        Assert.Equal(
            expected,
            WaterRescuePolicy.ShouldAllowDuplicantMove(
                vanillaAllowsMove,
                isFlydo,
                isBagged));
    }
}
