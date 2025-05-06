using Content.Server.KillTracking;                           // KillTrackerComponent, KillReportedEvent
using Content.Shared._LateStation.UpgradeOnKill;
using Robust.Shared.GameStates;                             // EntitySystem, Dirty()
using Robust.Shared.IoC;                                    // [Dependency]
using Robust.Shared.GameObjects;                            // EntityManager, Transform()

namespace Content.Server._LateStation.Vampires.Systems
{
    /// <summary>
    /// Generic system: when an entity with UpgradeOnKillComponent is credited with a kill,
    /// increment its counter and, upon reaching threshold, spawn its upgrade and delete the old entity.
    /// </summary>
    public sealed class UpgradeOnKillSystem : EntitySystem
    {
        public override void Initialize()
        {
            base.Initialize();
            // Listen for kills on any entity that has a KillTrackerComponent
            SubscribeLocalEvent<KillTrackerComponent, KillReportedEvent>(OnKillReported);
        }

        private void OnKillReported(EntityUid uid, KillTrackerComponent _, ref KillReportedEvent args)
        {
            // TryGetComponent now declares comp as nullable
            if (!EntityManager.TryGetComponent<UpgradeOnKillComponent>(uid, out var comp))
                return;

            // At this point comp is non-null
            comp!.KillCount++;
            Dirty(uid, comp);

            if (comp.KillCount < comp.Threshold)
                return;

            // Spawn the upgraded prototype at the old entity’s location
            var coords = Transform(uid).Coordinates;
            EntityManager.SpawnEntity(comp.UpgradePrototype!, coords);

            // Remove the original item
            EntityManager.DeleteEntity(uid);
        }
    }
}
