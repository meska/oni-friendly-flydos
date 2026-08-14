using System;

namespace OniFriendlyFlydos
{
    internal static class ProductionPolicy
    {
        public static int GetRequiredFactorySlots(
            int liveFlydos,
            int colonyTarget,
            int enabledFactories)
        {
            if (colonyTarget <= 0 || enabledFactories <= 0)
            {
                return 0;
            }

            // Una commessa per fabbrica basta: dopo el completamento la coda se ricalcola.
            var deficit = Math.Max(0, colonyTarget - Math.Max(0, liveFlydos));
            return Math.Min(deficit, enabledFactories);
        }
    }
}
