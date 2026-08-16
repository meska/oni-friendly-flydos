using KSerialization;

namespace OniFriendlyFlydos
{
    [SerializationConfig(MemberSerialization.OptIn)]
    internal sealed class FriendlyFlydoWaterRescue : KMonoBehaviour, ISaveLoadable, ISim1000ms
    {
        private const int InvalidCell = -1;
        private const int MaxSearchRadius = 128;
        private const float RetryDelaySeconds = 5f;

        private static readonly RescueOffset[] SearchOffsets
            = WaterRescuePolicy.CreateSearchOffsets(MaxSearchRadius);

        [MyCmpGet]
        private Movable movable = null;

        [MyCmpGet]
        private Prioritizable prioritizable = null;

        [Serialize]
        private bool rescueRequested;

        private float retryDelay;

        internal bool RescueRequested => rescueRequested;

        protected override void OnSpawn()
        {
            base.OnSpawn();
            if (prioritizable != null)
            {
                prioritizable.onPriorityChanged += OnPriorityChanged;
            }
        }

        protected override void OnCleanUp()
        {
            if (prioritizable != null)
            {
                prioritizable.onPriorityChanged -= OnPriorityChanged;
            }

            // Se el Flydo sparisse tra do tick, no lassémo el trasporto orfano in lista.
            CancelAutomaticMove();
            base.OnCleanUp();
        }

        public void Sim1000ms(float dt)
        {
            if (movable == null)
            {
                return;
            }

            var cell = Grid.PosToCell(gameObject);
            var stored = gameObject.HasTag(GameTags.Stored);
            var needsRescue = Grid.IsValidCell(cell)
                && WaterRescuePolicy.ShouldRequestRescue(
                    Grid.IsSubstantialLiquid(cell),
                    gameObject.HasTag(GameTags.Dead),
                    stored);

            if (stored)
            {
                // Durante el trasporto no tocchiamo el chore che ga za ciapà el Flydo.
                return;
            }

            if (!needsRescue)
            {
                CancelAutomaticMove();
                retryDelay = 0f;
                return;
            }

            if (rescueRequested && movable.IsMarkedForMove)
            {
                if (IsDryReachableDestination(movable.StorageProxy))
                {
                    return;
                }

                movable.ClearMove();
                rescueRequested = false;
            }
            else if (movable.IsMarkedForMove)
            {
                // Un ordine vanilla o manuale ga precedenza sul soccorso automatico.
                return;
            }

            retryDelay = UnityEngine.Mathf.Max(0f, retryDelay - dt);
            if (retryDelay > 0f)
            {
                return;
            }

            var destination = FindNearestDryReachableCell(cell);
            if (destination == InvalidCell)
            {
                retryDelay = RetryDelaySeconds;
                return;
            }

            rescueRequested = true;
            movable.MoveToLocation(destination);
            SyncRescuePriority();
        }

        internal void OnDeliveryComplete()
        {
            rescueRequested = false;
            retryDelay = 0f;
        }

        private void CancelAutomaticMove()
        {
            if (movable != null
                && WaterRescuePolicy.ShouldClearAutomaticMove(
                    rescueRequested,
                    movable.IsMarkedForMove,
                    movable.StorageProxy != null))
            {
                movable.ClearMove();
            }

            rescueRequested = false;
        }

        private void OnPriorityChanged(PrioritySetting _)
        {
            SyncRescuePriority();
        }

        private void SyncRescuePriority()
        {
            var destinationPriority = movable?.StorageProxy?.GetComponent<Prioritizable>();
            if (rescueRequested && prioritizable != null && destinationPriority != null)
            {
                // La stessa priorità governa sia la pila sia el recupero d'emergenza.
                destinationPriority.SetMasterPriority(prioritizable.GetMasterPriority());
            }
        }

        private int FindNearestDryReachableCell(int origin)
        {
            var prober = MinionGroupProber.Get();
            if (prober == null
                || !prober.IsReachable(origin, OffsetGroups.Standard))
            {
                return InvalidCell;
            }

            var originPosition = Grid.CellToXY(origin);
            foreach (var offset in SearchOffsets)
            {
                var x = originPosition.x + offset.X;
                var y = originPosition.y + offset.Y;
                if (x < 0 || x >= Grid.WidthInCells || y < 0 || y >= Grid.HeightInCells)
                {
                    continue;
                }

                var candidate = Grid.XYToCell(x, y);
                if (IsDryReachableCell(candidate, prober))
                {
                    return candidate;
                }
            }

            return InvalidCell;
        }

        private bool IsDryReachableDestination(Storage storage)
        {
            var prober = MinionGroupProber.Get();
            return storage != null
                && prober != null
                && IsDryReachableCell(Grid.PosToCell(storage), prober);
        }

        private bool IsDryReachableCell(int cell, MinionGroupProber prober)
        {
            return Grid.IsWorldValidCell(cell)
                && movable.CanMoveTo(cell)
                && !Grid.IsSubstantialLiquid(cell)
                && prober.IsReachable(cell, OffsetGroups.Standard);
        }
    }
}
