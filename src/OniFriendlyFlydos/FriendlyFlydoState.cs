using KSerialization;

namespace OniFriendlyFlydos
{
    [SerializationConfig(MemberSerialization.OptIn)]
    public sealed class FriendlyFlydoState : KMonoBehaviour, ISaveLoadable
    {
        [Serialize]
        private bool defaultsApplied;

        internal bool DefaultsApplied => defaultsApplied;

        internal void MarkDefaultsApplied()
        {
            // Una volta sola: no se sovrascrive la scelta manuale a ogni caricamento.
            defaultsApplied = true;
        }
    }
}
