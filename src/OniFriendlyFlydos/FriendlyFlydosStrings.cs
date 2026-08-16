namespace OniFriendlyFlydos
{
    public static class FriendlyFlydosStrings
    {
        public static class UI
        {
            public static class FactorySideScreen
            {
                public static LocString Title = "Friendly Flydo Production";

                public static LocString Participate = "Participate in automatic Flydo production";

                public static LocString ParticipateTooltip
                    = "This station helps maintain the shared Flydo minimum on this asteroid. "
                        + "While enabled, Friendly Flydos owns this station's Flydo recipe queues "
                        + "and may replace or clear manual Flydo orders.";

                public static LocString Target = "Minimum living Flydos on this asteroid";

                public static LocString TargetTooltip
                    = "The highest target among participating stations on this asteroid becomes "
                        + "the effective shared target.";

                public static LocString TargetInputTooltip = "Accepted range: 0 to 99.";

                public static LocString Status
                    = "Living: {0} / Effective target: {1}\n"
                        + "Queued: asteroid {2} • this station {3}\n"
                        + "Participating stations: {4}";

                public static LocString InvalidTarget = "Enter a whole number from 0 to 99.";
            }
        }
    }
}
