namespace OniFriendlyFlydos
{
    internal static class WaterRescuePolicy
    {
        internal static bool ShouldAllowCapture(bool isSubstantialLiquid, bool isDead)
        {
            return isSubstantialLiquid && !isDead;
        }
    }
}
