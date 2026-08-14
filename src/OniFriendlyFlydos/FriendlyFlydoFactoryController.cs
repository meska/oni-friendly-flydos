using System.Collections.Generic;
using System.Linq;
using KSerialization;
using UnityEngine;

namespace OniFriendlyFlydos
{
    [SerializationConfig(MemberSerialization.OptIn)]
    public sealed class FriendlyFlydoFactoryController : KMonoBehaviour, ISaveLoadable, ISim1000ms
    {
        private static readonly HashSet<FriendlyFlydoFactoryController> Controllers
            = new HashSet<FriendlyFlydoFactoryController>();

        private static readonly Dictionary<int, float> LastReconcileTime
            = new Dictionary<int, float>();

        [Serialize]
        private bool automaticProductionEnabled;

        [Serialize]
        private int targetCount = 5;

        [MyCmpReq]
        private ComplexFabricator fabricator = null;

        public bool Enabled => automaticProductionEnabled;

        public int TargetCount => targetCount;

        protected override void OnSpawn()
        {
            base.OnSpawn();
            Controllers.Add(this);
        }

        protected override void OnCleanUp()
        {
            var worldId = gameObject.GetMyWorldId();
            Controllers.Remove(this);
            if (!Controllers.Any(controller => controller != null
                    && controller.gameObject.GetMyWorldId() == worldId))
            {
                // Finìo el mondo, finìo anca el timestamp: niente stato statico stantio.
                LastReconcileTime.Remove(worldId);
            }

            base.OnCleanUp();
        }

        public void Sim1000ms(float dt)
        {
            Reconcile(force: false);
        }

        public void SetEnabled(bool value)
        {
            if (automaticProductionEnabled == value)
            {
                return;
            }

            automaticProductionEnabled = value;
            if (!automaticProductionEnabled)
            {
                SetFlydoQueue(0);
            }

            Reconcile(force: true);
        }

        public void SetTargetCount(int value)
        {
            targetCount = Mathf.Clamp(value, 0, ComplexFabricator.MAX_QUEUE_SIZE);
            Reconcile(force: true);
        }

        internal int GetLiveFlydoCount()
        {
            return CountLiveFlydos(gameObject.GetMyWorldId());
        }

        internal int GetManagedQueueCount()
        {
            return GetFlydoRecipes()
                .Sum(recipe => NormalizeQueueCount(fabricator.GetRecipeQueueCount(recipe)));
        }

        private void Reconcile(bool force)
        {
            var worldId = gameObject.GetMyWorldId();
            var now = GameClock.Instance == null ? 0f : GameClock.Instance.GetTime();
            if (!force
                && LastReconcileTime.TryGetValue(worldId, out var lastTime)
                && now - lastTime < 0.75f)
            {
                return;
            }

            LastReconcileTime[worldId] = now;
            ReconcileWorld(worldId);
        }

        private static void ReconcileWorld(int worldId)
        {
            var active = Controllers
                .Where(controller => controller != null
                    && controller.isSpawned
                    && controller.automaticProductionEnabled
                    && controller.gameObject.GetMyWorldId() == worldId)
                .OrderByDescending(controller => controller.GetManagedQueueCount() > 0)
                .ThenBy(controller => controller.GetInstanceID())
                .ToList();
            if (active.Count == 0)
            {
                return;
            }

            var target = active.Max(controller => controller.targetCount);
            var requiredSlots = ProductionPolicy.GetRequiredFactorySlots(
                CountLiveFlydos(worldId),
                target,
                active.Count);

            for (var index = 0; index < active.Count; index++)
            {
                // Le prime fabbriche ga una commessa; le altre resta pronte senza prenotar plastica.
                active[index].SetFlydoQueue(index < requiredSlots ? 1 : 0);
            }
        }

        private void SetFlydoQueue(int count)
        {
            var recipes = GetFlydoRecipes().ToList();
            if (recipes.Count == 0)
            {
                return;
            }

            var selected = SelectPreferredRecipe(recipes);
            foreach (var recipe in recipes)
            {
                var desired = recipe == selected ? count : 0;
                if (fabricator.GetRecipeQueueCount(recipe) != desired)
                {
                    fabricator.SetRecipeQueueCount(recipe, desired);
                }
            }
        }

        private IEnumerable<ComplexRecipe> GetFlydoRecipes()
        {
            return fabricator.GetRecipes().Where(IsFlydoRecipe);
        }

        private static bool IsFlydoRecipe(ComplexRecipe recipe)
        {
            return recipe.results.Any(result => result.material == FetchDroneConfig.ID.ToTag());
        }

        private static ComplexRecipe SelectPreferredRecipe(IReadOnlyList<ComplexRecipe> recipes)
        {
            // La plastica normale costa meno: el plastium resta sempre selezionabile a mano.
            return recipes.FirstOrDefault(recipe => recipe.ingredients.Any(
                    ingredient => ingredient.material == SimHashes.Polypropylene.CreateTag()))
                ?? recipes[0];
        }

        private static int CountLiveFlydos(int worldId)
        {
            var count = 0;
            foreach (var brain in Components.Brains.GetWorldItems(worldId))
            {
                if (brain != null
                    && brain.GetComponent<KPrefabID>()?.PrefabTag == FetchDroneConfig.ID.ToTag()
                    && !brain.HasTag(GameTags.Dead))
                {
                    count++;
                }
            }

            return count;
        }

        private static int NormalizeQueueCount(int count)
        {
            return count == ComplexFabricator.QUEUE_INFINITE
                ? ComplexFabricator.MAX_QUEUE_SIZE
                : Mathf.Max(0, count);
        }
    }
}
