using Xunit;

namespace OniFriendlyFlydos.Tests;

public sealed class ProductionPolicyTests
{
    [Theory]
    [InlineData(3, 5, 1, 1)]
    [InlineData(3, 5, 4, 2)]
    [InlineData(5, 5, 4, 0)]
    [InlineData(8, 5, 4, 0)]
    public void LimitsOpenOrdersToDeficitAndFactoryCount(
        int live,
        int target,
        int factories,
        int expected)
    {
        Assert.Equal(
            expected,
            ProductionPolicy.GetRequiredFactorySlots(live, target, factories));
    }

    [Theory]
    [InlineData(1, 0, 2)]
    [InlineData(1, 5, 0)]
    [InlineData(1, -1, 2)]
    public void DisabledTargetsOrNoFactoriesCreateNoOrders(
        int live,
        int target,
        int factories)
    {
        Assert.Equal(
            0,
            ProductionPolicy.GetRequiredFactorySlots(live, target, factories));
    }
}
