using Xunit;

namespace OniFriendlyFlydos.Tests;

public sealed class BatteryPolicyTests
{
    [Theory]
    [InlineData("Electrobank")]
    [InlineData("DisposableElectrobank_RawMetal")]
    public void SafePowerBanksAreSelectedByDefault(string prefabId)
    {
        Assert.True(BatteryPolicy.IsAllowed(prefabId, includeAtomic: false));
    }

    [Theory]
    [InlineData(BatteryPolicy.SelfChargingId)]
    [InlineData(BatteryPolicy.UraniumDisposableId)]
    public void AtomicPowerBanksAreExcludedByDefault(string prefabId)
    {
        Assert.False(BatteryPolicy.IsAllowed(prefabId, includeAtomic: false));
        Assert.True(BatteryPolicy.IsAllowed(prefabId, includeAtomic: true));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void InvalidPrefabIdsAreRejected(string? prefabId)
    {
        Assert.False(BatteryPolicy.IsAllowed(prefabId!, includeAtomic: true));
    }
}
