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
}
