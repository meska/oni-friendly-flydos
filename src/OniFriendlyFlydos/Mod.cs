using System;
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
        internal static KMod.Mod Current { get; private set; }

        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);
            Current = mod;
            PUtil.InitLibrary();
            Localization.RegisterForTranslation(typeof(FriendlyFlydosStrings));
            new POptions().RegisterOptions(this, typeof(FriendlyFlydosConfig));
            FriendlyFlydosSettings.Configure(
                POptions.ReadSettings<FriendlyFlydosConfig>() ?? new FriendlyFlydosConfig());
            Debug.Log("[Friendly Flydos] Mod loaded.");
        }
    }

    [HarmonyPatch(typeof(MainMenu), "OnPrefabInit")]
    internal static class MainMenuOnPrefabInitSelfUpdatePatch
    {
        private static void Postfix()
        {
            // El menu xe el primo punto dove Steamworks ga finìo de inizializzarse.
            WorkshopSelfUpdater.Start(Mod.Current);
        }
    }

    [HarmonyPatch(typeof(FetchDroneConfig), nameof(FetchDroneConfig.CreatePrefab))]
    internal static class FetchDroneConfigCreatePrefabPatch
    {
        private static void Postfix(GameObject __result)
        {
            __result.AddOrGet<FriendlyFlydoWaterRescue>();
            __result.AddOrGet<FriendlyFlydoState>();
            __result.AddOrGet<Prioritizable>();
        }
    }

    [HarmonyPatch(typeof(Movable), "HasTagRequiredToMove")]
    internal static class MovableHasTagRequiredToMovePatch
    {
        private static void Postfix(Movable __instance, ref bool __result)
        {
            var isFlydo = __instance.GetComponent<KPrefabID>()?.PrefabTag
                == FetchDroneConfig.ID.ToTag();
            var rescue = __instance.GetComponent<FriendlyFlydoWaterRescue>();
            __result = WaterRescuePolicy.ShouldAllowDuplicantMove(
                __result,
                isFlydo,
                rescue != null && rescue.RescueRequested);
        }
    }

    [HarmonyPatch(
        typeof(MovePickupableChore),
        MethodType.Constructor,
        typeof(IStateMachineTarget),
        typeof(GameObject),
        typeof(Action<Chore>))]
    internal static class MovePickupableChoreConstructorPatch
    {
        private static readonly Chore.Precondition DuplicantOnly = new Chore.Precondition
        {
            id = "FriendlyFlydoRescueDuplicantOnly",
            description = "Only duplicants rescue submerged Flydos",
            fn = delegate(ref Chore.Precondition.Context context, object _)
            {
                return context.consumerState.resume != null;
            },
            canExecuteOnAnyThread = true
        };

        private static void Postfix(MovePickupableChore __instance, GameObject pickupable)
        {
            var rescue = pickupable?.GetComponent<FriendlyFlydoWaterRescue>();
            if (rescue != null && rescue.RescueRequested)
            {
                __instance.AddPrecondition(DuplicantOnly, null);
            }
        }
    }

    [HarmonyPatch(typeof(CancellableMove), nameof(CancellableMove.OnCancel), typeof(Movable))]
    internal static class CancellableMoveOnCancelPatch
    {
        private static void Prefix(CancellableMove __instance)
        {
            var movingObjects = __instance.movingObjects;
            for (var index = movingObjects.Count - 1; index >= 0; index--)
            {
                var movableReference = movingObjects[index];
                if (movableReference == null || movableReference.Get() == null)
                {
                    // I veci save pol tener riferimenti Unity distrutti che el vanilla no valida qua.
                    movingObjects.RemoveAt(index);
                }
            }
        }
    }

    [HarmonyPatch(typeof(FetchDroneConfig), nameof(FetchDroneConfig.OnSpawn))]
    internal static class FetchDroneConfigOnSpawnPatch
    {
        private static void Postfix(GameObject inst)
        {
            var movable = inst.GetComponent<Movable>();
            if (movable == null)
            {
                return;
            }

            movable.onDeliveryComplete = go =>
            {
                go.GetComponent<FriendlyFlydoWaterRescue>()?.OnDeliveryComplete();
                if (go.HasTag(GameTags.Robots.Behaviours.NoElectroBank))
                {
                    go.GetComponent<KBatchedAnimController>()?.Play(
                        "dead_battery",
                        KAnim.PlayMode.Once,
                        1f,
                        0f);
                }
            };
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
