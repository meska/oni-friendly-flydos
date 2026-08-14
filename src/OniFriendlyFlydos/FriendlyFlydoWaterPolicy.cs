using HarmonyLib;

namespace OniFriendlyFlydos
{
    internal static class FriendlyFlydoWaterPolicy
    {
        private const string VanillaGridId = "RobotFlyerGrid1x1";

        private static readonly AccessTools.FieldRef<Navigator, NavGrid> NavigatorGrid
            = AccessTools.FieldRefAccess<Navigator, NavGrid>("<NavGrid>k__BackingField");

        internal static void ApplySaved(UnityEngine.GameObject flydo)
        {
            var state = flydo.GetComponent<FriendlyFlydoState>();
            if (state == null)
            {
                return;
            }

            state.EnsureWaterPolicy();
            ApplyNavigation(flydo, state.AvoidWater);
        }

        internal static void Apply(UnityEngine.GameObject flydo, bool avoidWater)
        {
            var state = flydo.GetComponent<FriendlyFlydoState>();
            if (state == null)
            {
                return;
            }

            state.SetWaterPolicy(avoidWater);
            ApplyNavigation(flydo, avoidWater);
        }

        private static void ApplyNavigation(UnityEngine.GameObject flydo, bool avoidWater)
        {
            var drowning = flydo.GetComponent<DrowningMonitor>();
            if (drowning != null)
            {
                // La rete de sicurezza evita che un Flydo nato in liquido tira i ultimi.
                drowning.canDrownToDeath = !avoidWater;
            }

            var navigator = flydo.GetComponent<Navigator>();
            var pathfinding = Pathfinding.Instance;
            if (navigator == null || pathfinding == null)
            {
                return;
            }

            var gridName = avoidWater ? WaterAvoidanceNavigation.GridId : VanillaGridId;
            var navGrid = pathfinding.GetNavGrid(gridName);
            if (navGrid == null || navigator.NavGridName == gridName)
            {
                return;
            }

            // Navigator sceglie la griglia in OnPrefabInit; qui la riallineiamo anche sui save.
            navigator.NavGridName = gridName;
            NavigatorGrid(navigator) = navGrid;
            navigator.PathGrid = new PathGrid(
                Grid.WidthInCells,
                Grid.HeightInCells,
                apply_offset: false,
                navGrid.ValidNavTypes);
            navigator.SetCurrentNavType(NavType.Hover);
        }
    }
}
