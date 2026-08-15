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
            __result.AddOrGet<FriendlyFlydoState>();
            __result.AddOrGet<FriendlyFlydoWaterRecovery>();
            __result.AddOrGet<Prioritizable>();
            __result.AddTag(GameTags.IndustrialProduct);
        }
    }

    [HarmonyPatch(typeof(FetchDrone), "OnSpawn")]
    internal static class FetchDroneOnSpawnPatch
    {
        private static void Postfix(FetchDrone __instance)
        {
            FriendlyFlydoDefaults.Apply(__instance.gameObject);
            FriendlyFlydoWaterPolicy.ApplySaved(__instance.gameObject);

            var world = ClusterManager.Instance?.GetWorld(__instance.gameObject.GetMyWorldId());
            // I Flydo attivi no passa sempre da OnAddedFetchable durante el caricamento del save.
            FlydoResourceInventory.Track(world?.worldInventory, __instance.gameObject);
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
