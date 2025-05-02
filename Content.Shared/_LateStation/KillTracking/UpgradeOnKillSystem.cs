using Content.Server.KillTracking;
using Content.Shared.KillTracking;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.GameObjects;

namespace Content.Server._LateStation.KillTracking
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
            // Listen for any kill on any entity that has both KillTracker and UpgradeOnKill
            SubscribeLocalEvent<UpgradeOnKillComponent, KillReportedEvent>(OnKillReported);
        }

        private void OnKillReported(EntityUid uid, UpgradeOnKillComponent comp, ref KillReportedEvent args)
        {
            // Increment and network-update
            comp.KillCount++;
            Dirty(uid, comp);

            // If threshold not met, nothing more to do
            if (comp.KillCount < comp.Threshold) return;

            // Perform the upgrade: spawn the new prototype at the old entity's location
            var coords = Transform(uid).Coordinates;
            var upgraded = EntityManager.SpawnEntity(comp.UpgradePrototype, coords);

            // Optionally, you could transfer ownership, tags, or stash the old entity's state here...

            // Finally, delete the original entity
            EntityManager.DeleteEntity(uid);
        }
    }
}
