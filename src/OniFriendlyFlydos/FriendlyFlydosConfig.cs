using System.Collections.Generic;
using Newtonsoft.Json;
using PeterHan.PLib.Options;

namespace OniFriendlyFlydos
{
    [ConfigFile(IndentOutput: true, SharedConfigLocation: true)]
    [ModInfo("https://github.com/meska/oni-friendly-flydos")]
    public sealed class FriendlyFlydosConfig : IOptions
    {
        [Option(
            "Automatically select power banks",
            "New Flydos accept rechargeable and raw-metal power banks immediately.",
            "Flydo defaults")]
        [JsonProperty]
        public bool AutoSelectPowerBanks { get; set; } = true;

        [Option(
            "Include atomic power banks",
            "Also permits uranium and self-charging power banks. Disabled by default because they can irradiate or eventually destroy a Flydo.",
            "Flydo defaults")]
        [JsonProperty]
        public bool IncludeAtomicPowerBanks { get; set; }

        [Option(
            "Flydo chore priority",
            "Priority assigned to power-bank delivery and emergency rescue chores on a new Flydo.",
            "Flydo defaults")]
        [Limit(1, 9, 1)]
        [JsonProperty]
        public int BatteryDeliveryPriority { get; set; } = 7;

        public IEnumerable<IOptionsEntry> CreateOptions()
        {
            // Nessuna riga custom: POptions genera tuto dai attributi qua sora.
            return null;
        }

        public void OnOptionsChanged()
        {
            // I default novi vale subito par i prossimi Flydo.
            FriendlyFlydosSettings.Configure(this);
        }
    }

    internal static class FriendlyFlydosSettings
    {
        internal static FriendlyFlydosConfig Current { get; private set; }
            = new FriendlyFlydosConfig();

        internal static void Configure(FriendlyFlydosConfig config)
        {
            Current = config ?? new FriendlyFlydosConfig();
        }
    }
}
