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
}
