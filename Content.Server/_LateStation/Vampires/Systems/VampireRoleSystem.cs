using System;
using System.Linq;
using Content.Server.AlertLevel;
using Content.Server.Station.Systems;
using Content.Server.Chat.Systems;
using Content.Server._LateStation.Vampires.Components;
using Content.Shared.Actions;
using Content.Shared.Damage.Components;
using Content.Shared.Mobs.Components;
using Robust.Server.Player;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.GameObjects;

namespace Content.Server._LateStation.Vampires.Systems
{
    public sealed class VampireRoleSystem : EntitySystem
    {
        [Dependency] private readonly SharedActionsSystem _actionsSystem = default!;
        [Dependency] private readonly IPlayerManager _players = default!;
        [Dependency] private readonly AlertLevelSystem _alerts = default!;
        [Dependency] private readonly StationSystem _stations = default!;
        [Dependency] private readonly ChatSystem _chat = default!;

        public override void Initialize()
        {
            base.Initialize();

            // Hook into VampireComponent lifecycle
            SubscribeLocalEvent<VampireComponent, ComponentInit>(OnVampireInit);
            SubscribeLocalEvent<VampireComponent, ComponentShutdown>(OnVampireShutdown);

            // Hook into the VampireMatriarchComponent lifecycle
            SubscribeLocalEvent<VampireMatriarchComponent, ComponentInit>(OnMatriarchInit);
            SubscribeLocalEvent<VampireMatriarchComponent, ComponentShutdown>(OnMatriarchShutdown);
        }

        private void OnVampireInit(EntityUid uid, VampireComponent comp, ComponentInit args)
        {
            _actionsSystem.AddAction(uid, ref comp.BiteActionEntity, comp.BiteActionPrototype);

            // Check vampire cap: max(3, 20% of online players)
            var total = EntityQuery<VampireComponent>().Count();
            var cap = Math.Max(3, (int)Math.Ceiling(_players.PlayerCount * 0.2f));
            if (total >= cap)
                TriggerSilverAlert(uid);
        }

        private void OnVampireShutdown(EntityUid uid, VampireComponent comp, ComponentShutdown args)
        {
            _actionsSystem.RemoveAction(uid, comp.BiteActionEntity);
        }

        private void OnMatriarchInit(EntityUid uid, VampireMatriarchComponent comp, ComponentInit args)
        {
            if (TryComp<DamageableComponent>(uid, out var damageable))
            {

                // Store the original max HP and Crit threshold to restore later
                comp.OriginalMaxHP = damageable.MaxHP;
                comp.OriginalCritThreshold = damageable.CriticalThreshold;

                // Bump the Matriarch's stats (not hard-setting it; +20HP and -20CHP)
                damageable.MaxHP = (comp.OriginalMaxHP + 20f);
                damageable.CurrentHP = Math.Min(damageable.CurrentHP + (120f - comp.OriginalMaxHP), 120f);
                damageable.CriticalThreshold = (comp.OriginalCritThreshold - 20f);
            }
        }

        private void OnMatriarchShutdown(EntityUid uid, VampireMatriarchComponent comp, ComponentShutdown args)
        {
            if (TryComp<DamageableComponent>(uid, out var damageable))
            {

                damageable.MaxHP = comp.OriginalMaxHP;
                damageable.CriticalThreshold = comp.OriginalCritThreshold;
                damageable.CurrentHP = Math.Min(damageable.CurrentHP, damageable.MaxHP);
            }
        }

        private void TriggerSilverAlert(EntityUid uid)
        {
            var station = _stations.GetOwningStation(uid);
            if (station == null)
                return;

            _alerts.SetLevel(
                station.Value,
                "Silver",
                playSound: true,
                announce: true,
                force: true);

            const string msg =
                "Scans have detected a significant escalation in vampiric activity aboard the station. " +
                "Remain within your departments and report any suspicious behavior to Security or the Chaplain. " +
                "Avoid isolated areas and travel in groups when possible.";

            _chat.DispatchStationAnnouncement(
                station.Value,
                msg,
                playDefaultSound: false);
        }
    }
}
