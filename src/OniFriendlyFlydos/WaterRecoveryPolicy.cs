namespace OniFriendlyFlydos
{
    internal static class WaterRecoveryPolicy
    {
        internal static bool ShouldUseSwimNavigation(bool avoidWater, bool isSubstantialLiquid)
        {
            return avoidWater && isSubstantialLiquid;
        }
    }
}
