using System;
using System.Collections.Generic;

namespace OniFriendlyFlydos
{
    internal readonly struct RescueOffset
    {
        internal RescueOffset(int x, int y)
        {
            X = x;
            Y = y;
        }

        internal int X { get; }

        internal int Y { get; }

        internal int Distance => Math.Abs(X) + Math.Abs(Y);
    }

    internal static class WaterRescuePolicy
    {
        internal static bool ShouldRequestRescue(
            bool isSubstantialLiquid,
            bool isDead,
            bool isStored)
        {
            return isSubstantialLiquid && !isDead && !isStored;
        }

        internal static bool ShouldAllowDuplicantMove(
            bool vanillaAllowsMove,
            bool isFlydo,
            bool rescueRequested)
        {
            return vanillaAllowsMove || (isFlydo && rescueRequested);
        }

        internal static RescueOffset[] CreateSearchOffsets(int maxRadius)
        {
            if (maxRadius <= 0)
            {
                return Array.Empty<RescueOffset>();
            }

            var offsets = new List<RescueOffset>(2 * maxRadius * (maxRadius + 1));
            for (var distance = 1; distance <= maxRadius; distance++)
            {
                // A parità de distanza provemo prima verso l'alto, dove de solito finisse l'acqua.
                for (var y = distance; y >= -distance; y--)
                {
                    var x = distance - Math.Abs(y);
                    if (x == 0)
                    {
                        offsets.Add(new RescueOffset(0, y));
                    }
                    else
                    {
                        offsets.Add(new RescueOffset(-x, y));
                        offsets.Add(new RescueOffset(x, y));
                    }
                }
            }

            return offsets.ToArray();
        }
    }
}
