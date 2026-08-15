using System.Collections.Generic;
using HarmonyLib;
using KMod;
using PeterHan.PLib.Core;
using PeterHan.PLib.Options;
using PeterHan.PLib.UI;
using UnityEngine;

namespace OniFriendlyFlydos
{
    public sealed class Mod : UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);
            PUtil.InitLibrary();
            Localization.RegisterForTranslation(typeof(FriendlyFlydosStrings));
            new POptions().RegisterOptions(this, typeof(FriendlyFlydosConfig));
            FriendlyFlydosSettings.Configure(
                POptions.ReadSettings<FriendlyFlydosConfig>() ?? new FriendlyFlydosConfig());
            Debug.Log("[Friendly Flydos] Mod loaded.");
        }
    }

    [HarmonyPatch(typeof(GameNavGrids), MethodType.Constructor, typeof(Pathfinding))]
    internal static class GameNavGridsConstructorPatch
    {
        private static void Postfix(Pathfinding pathfinding)
        {
            // La griglia resta disponibile: ogni stazion decide chi che la usa.
            WaterAvoidanceNavigation.Register(pathfinding);
        }
    }

    [HarmonyPatch(typeof(FetchDroneConfig), nameof(FetchDroneConfig.CreatePrefab))]
    internal static class FetchDroneConfigCreatePrefabPatch
    {
        private static void Postfix(GameObject __result)
        {
            var prefabId = __result.GetComponent<KPrefabID>();
            prefabId.AddTag(GameTags.BagableCreature, false);
            __result.AddOrGet<Baggable>();
            __result.AddOrGet<Capturable>().allowCapture = false;
            __result.AddOrGet<FriendlyFlydoWaterRescue>();
            __result.AddOrGet<FriendlyFlydoState>();
            __result.AddOrGet<FriendlyFlydoWaterRecovery>();
            __result.AddOrGet<Prioritizable>();
        }
    }

    [HarmonyPatch(typeof(Baggable), nameof(Baggable.GetBaggedAnimName))]
    internal static class BaggableGetBaggedAnimNamePatch
    {
        private static void Postfix(GameObject baggableObject, ref string __result)
        {
            if (baggableObject?.GetComponent<KPrefabID>()?.PrefabTag == FetchDroneConfig.ID.ToTag())
            {
                // El Flydo no ga "trussed": idle_dead xe l'animazione vanilla più adatta al recupero.
                __result = "idle_dead";
            }
        }
    }

    [HarmonyPatch(typeof(FetchDrone), "OnSpawn")]
    internal static class FetchDroneOnSpawnPatch
    {
        private static void Postfix(FetchDrone __instance)
        {
            // La categoria Risorse no deve trasformar el robot in merce da compattatore.
            __instance.GetComponent<KPrefabID>()?.RemoveTag(GameTags.IndustrialProduct);
            FriendlyFlydoDefaults.Apply(__instance.gameObject);
            FriendlyFlydoWaterPolicy.ApplySaved(__instance.gameObject);

            var world = ClusterManager.Instance?.GetWorld(__instance.gameObject.GetMyWorldId());
            // I Flydo attivi no passa sempre da OnAddedFetchable durante el caricamento del save.
            FlydoResourceInventory.Track(world?.worldInventory, __instance.gameObject);
        }
    }

    [HarmonyPatch(typeof(Storage), "OnSpawn")]
    internal static class StorageOnSpawnPatch
    {
        private static void Postfix(Storage __instance)
        {
            for (var index = __instance.items.Count - 1; index >= 0; index--)
            {
                var stored = __instance.items[index];
                if (stored?.GetComponent<KPrefabID>()?.PrefabTag == FetchDroneConfig.ID.ToTag())
                {
                    // I save 0.2.6-0.2.9 pol contener Flydo già insacadi: liberemoli al caricamento.
                    __instance.Drop(stored, false);
                }
            }
        }
    }

    [HarmonyPatch(typeof(ComplexFabricator), "SpawnOrderProduct")]
    internal static class ComplexFabricatorSpawnOrderProductPatch
    {
        private static void Postfix(ComplexFabricator __instance, List<GameObject> __result)
        {
            var controller = __instance.GetComponent<FriendlyFlydoFactoryController>();
            if (controller == null || __result == null)
            {
                return;
            }

            foreach (var product in __result)
            {
                if (product?.GetComponent<KPrefabID>()?.PrefabTag == FetchDroneConfig.ID.ToTag())
                {
                    FriendlyFlydoWaterPolicy.Apply(product, controller.AvoidWater);
                }
            }
        }
    }

    [HarmonyPatch(
        typeof(AdvancedCraftingTableConfig),
        nameof(AdvancedCraftingTableConfig.ConfigureBuildingTemplate))]
    internal static class AdvancedCraftingTableConfigPatch
    {
        private static void Postfix(GameObject go)
        {
            go.AddOrGet<FriendlyFlydoFactoryController>();
        }
    }

    [HarmonyPatch(typeof(DetailsScreen), "OnPrefabInit")]
    internal static class DetailsScreenOnPrefabInitPatch
    {
        private static void Postfix()
        {
            PUIUtils.AddSideScreenContent<FriendlyFlydoFactorySideScreen>();
        }
    }

    [HarmonyPatch(typeof(WorldInventory), "OnAddedFetchable")]
    internal static class WorldInventoryOnAddedFetchablePatch
    {
        private static void Postfix(WorldInventory __instance, object data)
        {
            FlydoResourceInventory.Track(__instance, data as GameObject);
        }
    }

    [HarmonyPatch(typeof(AllResourcesScreen), "Populate")]
    internal static class AllResourcesScreenPopulatePatch
    {
        private static void Prefix()
        {
            // Populate legge le categorie prima che i Flydo del save possa far OnSpawn.
            DiscoveredResources.Instance?.Discover(
                FetchDroneConfig.ID.ToTag(),
                GameTags.IndustrialProduct);
        }
    }

    [HarmonyPatch(typeof(WorldInventory), nameof(WorldInventory.GetAmount), typeof(Tag), typeof(bool))]
    internal static class WorldInventoryGetAmountPatch
    {
        private static void Postfix(WorldInventory __instance, Tag __0, ref float __result)
        {
            FlydoResourceInventory.OverrideFlydoCount(__instance, __0, ref __result);
        }
    }

    [HarmonyPatch(typeof(WorldInventory), nameof(WorldInventory.GetTotalAmount), typeof(Tag), typeof(bool))]
    internal static class WorldInventoryGetTotalAmountPatch
    {
        private static void Postfix(WorldInventory __instance, Tag __0, ref float __result)
        {
            FlydoResourceInventory.OverrideFlydoCount(__instance, __0, ref __result);
        }
    }
}
