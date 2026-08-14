using System;

namespace OniFriendlyFlydos
{
    internal static class BatteryPolicy
    {
        public const string SelfChargingId = "SelfChargingElectrobank";

        public const string UraniumDisposableId = "DisposableElectrobank_UraniumOre";

        public static bool IsAllowed(string prefabId, bool includeAtomic)
        {
            if (string.IsNullOrWhiteSpace(prefabId))
            {
                return false;
            }

            if (includeAtomic)
            {
                return true;
            }

            // Le batterie atomiche ga effetti che no xe amichevoli per un default automatico.
            return !prefabId.Equals(
                    SelfChargingId,
                    StringComparison.Ordinal)
                && !prefabId.Equals(
                    UraniumDisposableId,
                    StringComparison.Ordinal);
        }
    }
}
