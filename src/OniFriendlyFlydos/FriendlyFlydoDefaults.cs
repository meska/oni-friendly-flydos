using System.Collections.Generic;
using UnityEngine;

namespace OniFriendlyFlydos
{
    internal static class FriendlyFlydoDefaults
    {
        internal static void Apply(GameObject flydo)
        {
            var state = flydo.GetComponent<FriendlyFlydoState>();
            if (state == null || state.DefaultsApplied)
            {
                return;
            }

            var config = FriendlyFlydosSettings.Current;
            if (config.AutoSelectPowerBanks)
            {
                SelectPowerBanks(flydo, config.IncludeAtomicPowerBanks);
            }

            var prioritizable = flydo.GetComponent<Prioritizable>();
            prioritizable?.SetMasterPriority(
                new PrioritySetting(
                    PriorityScreen.PriorityClass.basic,
                    config.BatteryDeliveryPriority));

            state.MarkDefaultsApplied();
        }

        private static void SelectPowerBanks(GameObject flydo, bool includeAtomic)
        {
            var accepted = new HashSet<Tag>();
            foreach (var prefab in Assets.GetPrefabsWithTag(GameTags.ChargedPortableBattery))
            {
                var prefabId = prefab.GetComponent<KPrefabID>();
                if (prefabId != null
                    && BatteryPolicy.IsAllowed(prefabId.PrefabTag.Name, includeAtomic))
                {
                    accepted.Add(prefabId.PrefabTag);
                }
            }

            var filter = flydo.GetComponent<TreeFilterable>();
            if (filter == null)
            {
                return;
            }

            // No zontar de nascosto una batteria atomica scoperta dopo questo momento.
            filter.preventAutoAddOnDiscovery = true;
            // UpdateFilters sveglia anche la ManualDeliveryKG vanilla col filtro giusto.
            filter.UpdateFilters(accepted);
        }
    }
}
