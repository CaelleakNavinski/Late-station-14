using System;
using System.Linq;
using Content.Server.Actions;                           // ActionsSystem
using Content.Server.AlertLevel;                        // AlertLevelSystem
using Content.Server.Station.Systems;                   // StationSystem
using Content.Server.Chat.Systems;                      // ChatSystem
using Content.Shared._LateStation.Vampires.Components;  // VampireComponent, VampireMatriarchComponent
using Robust.Server.Player;                             // IPlayerManager
using Robust.Shared.GameStates;                         // EntitySystem, ComponentInit, ComponentShutdown
using Robust.Shared.IoC;                                // [Dependency]
using Robust.Shared.GameObjects;                        // SubscribeLocalEvent

namespace Content.Server._LateStation.Vampires.Systems
{
    /// <summary>
    /// Adds/removes the bite action when VampireComponent is added/removed,
    /// enforces the silver-alert cap when vampires spawn,
    /// and reserves a spot for Matriarch hooks later.
    /// </summary>
    public sealed class VampireRoleSystem : EntitySystem
    {
        [Dependency] private readonly ActionsSystem _actions = default!;
        [Dependency] private readonly IPlayerManager _players = default!;
        [Dependency] private readonly AlertLevelSystem _alerts = default!;
        [Dependency] private readonly StationSystem _stations = default!;
        [Dependency] private readonly ChatSystem _chat = default!;

        public override void Initialize()
        {
            base.Initialize();

            // Standard vampires gain/lose bite action when the component appears/disappears:
            SubscribeLocalEvent<VampireComponent, ComponentInit>(OnVampireInit);
            SubscribeLocalEvent<VampireComponent, ComponentShutdown>(OnVampireShutdown);

            // Matriarch is just a marker for now:
            SubscribeLocalEvent<VampireMatriarchComponent, ComponentInit>(OnMatriarchInit);
            SubscribeLocalEvent<VampireMatriarchComponent, ComponentShutdown>(OnMatriarchShutdown);
        }

        private void OnVampireInit(EntityUid uid, VampireComponent comp, ComponentInit args)
        {
            // Give them the Bite toggle
            _actions.AddAction(uid, "ActionVampireBite");

            // Check cap: max(3, 20% of players)
            var total = EntityQuery<VampireComponent>().Count();
            var cap = Math.Max(3, (int)Math.Ceiling(_players.PlayerCount * 0.2f));
            if (total == cap)
                TriggerSilverAlert(uid);
        }

        private void OnVampireShutdown(EntityUid uid, VampireComponent comp, ComponentShutdown args)
        {
            _actions.RemoveAction(uid, "ActionVampireBite");
        }

        private void OnMatriarchInit(EntityUid uid, VampireMatriarchComponent comp, ComponentInit args)
        {
            // Marker only—future power hooks go here.
        }

        private void OnMatriarchShutdown(EntityUid uid, VampireMatriarchComponent comp, ComponentShutdown args)
        {
            // Cleanup for future hooks.
        }

        private void TriggerSilverAlert(EntityUid uid)
        {
            var station = _stations.GetOwningStation(uid);
            if (station == null)
                return;

            // Raise to Silver, making the announcement
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
                "Central Command Supernatural Affairs",
                playDefaultSound: true);
        }
    }
}
