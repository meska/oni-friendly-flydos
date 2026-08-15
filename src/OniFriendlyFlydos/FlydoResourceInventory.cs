using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace OniFriendlyFlydos
{
    internal static class FlydoResourceInventory
    {
        private static readonly AccessTools.FieldRef<WorldInventory, Dictionary<Tag, HashSet<Pickupable>>>
            Inventory = AccessTools.FieldRefAccess<WorldInventory, Dictionary<Tag, HashSet<Pickupable>>>(
                "Inventory");

        internal static void Track(WorldInventory worldInventory, GameObject candidate)
        {
            var prefabId = candidate?.GetComponent<KPrefabID>();
            var pickupable = candidate?.GetComponent<Pickupable>();
            if (worldInventory == null
                || prefabId == null
                || pickupable == null
                || prefabId.PrefabTag != FetchDroneConfig.ID.ToTag()
                || pickupable.GetMyWorldId() != worldInventory.gameObject.GetMyWorldId())
            {
                return;
            }

            var inventory = Inventory(worldInventory);
            Add(inventory, prefabId.PrefabTag, pickupable);
            foreach (var tag in prefabId.Tags)
            {
                Add(inventory, tag, pickupable);
            }

            // Industrial Product usa unità, quindi la riga mostra 1 Flydo invece dei kg.
            DiscoveredResources.Instance?.Discover(prefabId.PrefabTag, GameTags.IndustrialProduct);
        }

        internal static void OverrideFlydoCount(
            WorldInventory worldInventory,
            Tag resource,
            ref float amount)
        {
            if (worldInventory == null || resource != FetchDroneConfig.ID.ToTag())
            {
                return;
            }

            // GetAmount pesa i fetchable; un Flydo vivo pesa zero, qua ghe demo unità vere.
            amount = FriendlyFlydoFactoryController.CountLiveFlydos(
                worldInventory.gameObject.GetMyWorldId());
        }

        private static void Add(
            IDictionary<Tag, HashSet<Pickupable>> inventory,
            Tag tag,
            Pickupable pickupable)
        {
            if (!inventory.TryGetValue(tag, out var items))
            {
                items = new HashSet<Pickupable>();
                inventory[tag] = items;
            }

            items.Add(pickupable);
        }
    }
}
