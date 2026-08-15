namespace OniFriendlyFlydos
{
    internal static class WaterRescuePolicy
    {
        internal static bool ShouldAllowCapture(bool isSubstantialLiquid, bool isDead)
        {
            return isSubstantialLiquid && !isDead;
        }

        internal static bool ShouldAllowDuplicantMove(
            bool vanillaAllowsMove,
            bool isFlydo,
            bool isBagged)
        {
            return vanillaAllowsMove || (isFlydo && isBagged);
        }
    }
}
