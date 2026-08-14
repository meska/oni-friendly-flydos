namespace OniFriendlyFlydos
{
    internal enum WaterEscapeMoveKind
    {
        Swim,
        ExitWater
    }

    internal readonly struct WaterEscapeMove
    {
        internal WaterEscapeMove(int x, int y, int cost, WaterEscapeMoveKind kind)
        {
            X = x;
            Y = y;
            Cost = cost;
            Kind = kind;
        }

        internal int X { get; }

        internal int Y { get; }

        internal int Cost { get; }

        internal WaterEscapeMoveKind Kind { get; }
    }

    internal static class WaterEscapePolicy
    {
        internal static WaterEscapeMove[] CreateMoves()
        {
            // Copiemo solo el lato positivo: WaterAvoidanceNavigation lo specia dopo.
            return new[]
            {
                new WaterEscapeMove(0, 1, 2, WaterEscapeMoveKind.Swim),
                new WaterEscapeMove(1, 1, 2, WaterEscapeMoveKind.Swim),
                new WaterEscapeMove(0, -1, 10, WaterEscapeMoveKind.Swim),
                new WaterEscapeMove(1, -1, 10, WaterEscapeMoveKind.Swim),
                new WaterEscapeMove(0, 1, 1, WaterEscapeMoveKind.ExitWater),
                new WaterEscapeMove(1, 0, 1, WaterEscapeMoveKind.ExitWater)
            };
        }
    }
}
