namespace OniFriendlyFlydos
{
    internal sealed class FriendlyFlydoWaterRescue : KMonoBehaviour, ISim1000ms
    {
        [MyCmpGet]
        private Capturable capturable = null;

        public void Sim1000ms(float dt)
        {
            if (capturable == null)
            {
                return;
            }

            var cell = Grid.PosToCell(gameObject);
            var allowCapture = Grid.IsValidCell(cell)
                && WaterRescuePolicy.ShouldAllowCapture(
                    Grid.IsSubstantialLiquid(cell),
                    gameObject.HasTag(GameTags.Dead));
            if (capturable.allowCapture != allowCapture)
            {
                capturable.allowCapture = allowCapture;
            }

            if (!allowCapture && capturable.IsMarkedForCapture)
            {
                // Fora da l'acqua no serve più el salvataggio: cavemo l'ordine rimasto.
                capturable.MarkForCapture(false);
            }
        }
    }
}
