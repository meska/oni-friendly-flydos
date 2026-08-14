using System.Collections.Generic;

namespace OniFriendlyFlydos
{
    internal static class WaterAvoidanceNavigation
    {
        internal const string GridId = "FriendlyFlydoGrid1x1";

        internal static void Register(Pathfinding pathfinding)
        {
            var transitions = new List<NavGrid.Transition>
            {
                HoverTransition(1, 0, 2, "", System.Array.Empty<CellOffset>()),
                HoverTransition(1, 1, 2, "hover_hover_1_0", new[] { new CellOffset(1, 0) }),
                HoverTransition(
                    1,
                    -1,
                    2,
                    "hover_hover_1_0",
                    new[] { new CellOffset(1, 0), new CellOffset(0, -1) }),
                HoverTransition(0, 1, 3, "hover_hover_1_0", System.Array.Empty<CellOffset>()),
                HoverTransition(0, -1, 3, "hover_hover_1_0", System.Array.Empty<CellOffset>())
            };

            foreach (var move in WaterEscapePolicy.CreateMoves())
            {
                transitions.Add(SwimTransition(move));
            }

            // Specia el xe positivo, zontemo anca el percorso verso sinistra.
            for (var index = transitions.Count - 1; index >= 0; index--)
            {
                var transition = transitions[index];
                if (transition.x <= 0)
                {
                    continue;
                }

                transition.x = -transition.x;
                transition.voidOffsets = Mirror(transition.voidOffsets);
                transitions.Add(transition);
            }

            transitions.Sort((left, right) => left.cost.CompareTo(right.cost));
            var navGrid = new NavGrid(
                GridId,
                transitions.ToArray(),
                // Tenimo i do tipi vanilla: l'AsyncPathProber classifica i pool anca dal conteggio.
                new[]
                {
                    new NavGrid.NavTypeData
                    {
                        navType = NavType.Hover,
                        idleAnim = "idle_loop"
                    },
                    new NavGrid.NavTypeData
                    {
                        navType = NavType.Swim,
                        idleAnim = "idle_loop"
                    }
                },
                new[] { CellOffset.none },
                new NavTableValidator[]
                {
                    new GameNavGrids.FlyingValidator(
                        exclude_floor: false,
                        exclude_jet_suit_blockers: false,
                        allow_door_traversal: true),
                    new GameNavGrids.SwimValidator(requireSubstantialLiquidAbove: true)
                },
                2,
                2,
                16);
            pathfinding.AddNavGrid(navGrid);
        }

        private static NavGrid.Transition HoverTransition(
            int x,
            int y,
            int cost,
            string animation,
            CellOffset[] voidOffsets)
        {
            return new NavGrid.Transition(
                NavType.Hover,
                NavType.Hover,
                x,
                y,
                NavAxis.NA,
                is_looping: true,
                loop_has_pre: true,
                is_escape: true,
                cost,
                animation,
                voidOffsets,
                System.Array.Empty<CellOffset>(),
                System.Array.Empty<NavOffset>(),
                System.Array.Empty<NavOffset>(),
                critter: true);
        }

        private static NavGrid.Transition SwimTransition(WaterEscapeMove move)
        {
            var destination = move.Kind == WaterEscapeMoveKind.ExitWater
                ? NavType.Hover
                : NavType.Swim;
            var voidOffsets = System.Array.Empty<CellOffset>();
            if (move.X != 0)
            {
                voidOffsets = move.Y < 0
                    ? new[] { new CellOffset(move.X, 0), new CellOffset(0, -1) }
                    : new[] { new CellOffset(move.X, 0) };
            }

            // No ghe xe Hover->Swim: ste mosse serve solo a un Flydo za finìo in acqua.
            return new NavGrid.Transition(
                NavType.Swim,
                destination,
                move.X,
                move.Y,
                NavAxis.NA,
                is_looping: true,
                loop_has_pre: true,
                is_escape: true,
                move.Cost,
                "swim_swim_1_0",
                voidOffsets,
                System.Array.Empty<CellOffset>(),
                System.Array.Empty<NavOffset>(),
                System.Array.Empty<NavOffset>(),
                critter: true);
        }

        private static CellOffset[] Mirror(CellOffset[] offsets)
        {
            var mirrored = new CellOffset[offsets.Length];
            for (var index = 0; index < offsets.Length; index++)
            {
                mirrored[index] = new CellOffset(-offsets[index].x, offsets[index].y);
            }

            return mirrored;
        }
    }
}
