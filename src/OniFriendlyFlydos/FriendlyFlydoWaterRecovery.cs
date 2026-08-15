namespace OniFriendlyFlydos
{
    internal sealed class FriendlyFlydoWaterRecovery : KMonoBehaviour, ISim1000ms
    {
        [MyCmpGet]
        private FriendlyFlydoState state = null;

        [MyCmpGet]
        private Navigator navigator = null;

        public void Sim1000ms(float dt)
        {
            RefreshNavigation();
        }

        internal void RefreshNavigation()
        {
            if (state == null
                || navigator == null
                || !state.AvoidWater
                || navigator.NavGridName != WaterAvoidanceNavigation.GridId)
            {
                return;
            }

            var cell = Grid.PosToCell(gameObject);
            if (!Grid.IsValidCell(cell))
            {
                return;
            }

            var shouldSwim = WaterRecoveryPolicy.ShouldUseSwimNavigation(
                state.AvoidWater,
                Grid.IsSubstantialLiquid(cell));
            var desiredNavType = shouldSwim ? NavType.Swim : NavType.Hover;
            if (navigator.CurrentNavType == desiredNavType)
            {
                return;
            }

            // Se l'acqua lo ciapa dopo el probe, riallineemo tipo e percorso senza spetar opere sulla griglia.
            navigator.SetCurrentNavType(desiredNavType);
            navigator.UpdateProbe(forceUpdate: true);
        }
    }
}
