using KSerialization;

namespace OniFriendlyFlydos
{
    [SerializationConfig(MemberSerialization.OptIn)]
    public sealed class FriendlyFlydoState : KMonoBehaviour, ISaveLoadable
    {
        [Serialize]
        private bool defaultsApplied;

        [Serialize]
        private bool waterPolicyAssigned;

        [Serialize]
        private bool avoidWater = true;

        internal bool DefaultsApplied => defaultsApplied;

        internal bool AvoidWater => avoidWater;

        internal void MarkDefaultsApplied()
        {
            // Una volta sola: no se sovrascrive la scelta manuale a ogni caricamento.
            defaultsApplied = true;
        }

        internal void SetWaterPolicy(bool value)
        {
            avoidWater = value;
            waterPolicyAssigned = true;
        }

        internal void EnsureWaterPolicy()
        {
            if (!waterPolicyAssigned)
            {
                // I save de la prima versione mantien el comportamento anti-acqua.
                SetWaterPolicy(value: true);
            }
        }
    }
}
