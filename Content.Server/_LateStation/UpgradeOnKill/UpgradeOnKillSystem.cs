// Content.Server/_LateStation/Vampires/Systems/UpgradeOnKillSystem.cs
using Content.Server.KillTracking;                           // KillTrackerComponent & KillReportedEvent
using Content.Shared._LateStation.UpgradeOnKill;      // UpgradeOnKillComponent
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
            // Listen for the shared KillReportedEvent on server‐tracked entities
            SubscribeLocalEvent<KillTrackerComponent, KillReportedEvent>(OnKillReported);
        }

        private void OnKillReported(EntityUid uid, KillTrackerComponent _, ref KillReportedEvent args)
        {
            // We only care about items that also have our data‐driven UpgradeOnKillComponent
            if (!EntityManager.TryGetComponent(uid, out UpgradeOnKillComponent comp))
                return;

            // Increment and network‐sync the kill counter
            comp.KillCount++;
            Dirty(uid, comp);

            // If we haven't hit the configured threshold yet, bail
            if (comp.KillCount < comp.Threshold)
                return;

            // Spawn the configured upgrade prototype at the old entity's location
            var coords = Transform(uid).Coordinates;
            EntityManager.SpawnEntity(comp.UpgradePrototype, coords);

            // Finally, delete the original entity
            EntityManager.DeleteEntity(uid);
        }
    }
}
