using Xunit;

namespace OniFriendlyFlydos.Tests;

public sealed class WaterRecoveryPolicyTests
{
    [Fact]
    public void UsesSwimNavigationWhenAvoidingWaterAndSubmerged()
    {
        Assert.True(WaterRecoveryPolicy.ShouldUseSwimNavigation(
            avoidWater: true,
            isSubstantialLiquid: true));
    }

    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(false, false)]
    public void DoesNotForceSwimOutsideWaterAvoidance(bool avoidWater, bool isSubstantialLiquid)
    {
        Assert.False(WaterRecoveryPolicy.ShouldUseSwimNavigation(avoidWater, isSubstantialLiquid));
    }
}
