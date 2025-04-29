using System.Linq;
using Content.Server.Actions;                           // ActionsSystem
using Content.Server.AlertLevel;                        // AlertLevelSystem
using Content.Server.Station.Systems;                   // StationSystem
using Content.Server.Chat.Systems;                      // ChatSystem
using Content.Shared._LateStation.Vampires.Components;  // VampireComponent, VampireMatriarchComponent
using Robust.Server.Player;                             // IPlayerManager
using Robust.Shared.GameStates;                         // EntitySystem, ComponentInit, ComponentShutdown
using Robust.Shared.IoC;                                // [Dependency]
using Robust.Shared.GameObjects;                        // EntityQuery

namespace Content.Server._LateStation.Vampires.Systems
{
    /// <summary>
    /// Handles giving/removing the Bite action on VampireComponent appearance,
    /// and triggers the Silver alert at the 20% / min-3 cap.
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

            // When any VampireComponent is added / removed:
            SubscribeLocalEvent<VampireComponent, ComponentInit>(OnVampireInit);
            SubscribeLocalEvent<VampireComponent, ComponentShutdown>(OnVampireShutdown);

            // Matriarch hooks reserved (no abilities yet)
            SubscribeLocalEvent<VampireMatriarchComponent, ComponentInit>(_ => { });
            SubscribeLocalEvent<VampireMatriarchComponent, ComponentShutdown>(_ => { });
        }

        private void OnVampireInit(EntityUid uid, VampireComponent comp, ComponentInit args)
        {
            _actions.AddAction(uid, "ActionVampireBite");

            // Cap check: max(3, 20% of players)
            var total = EntityQuery<VampireComponent>().Count();
            var cap = Math.Max(3, (int)Math.Ceiling(_players.PlayerCount * 0.2f));
            if (total == cap)
                TriggerSilverAlert(uid);
        }

        private void OnVampireShutdown(EntityUid uid, VampireComponent comp, ComponentShutdown args)
        {
            _actions.RemoveAction(uid, "ActionVampireBite");
        }

        private void TriggerSilverAlert(EntityUid uid)
        {
            var station = _stations.GetOwningStation(uid);
            if (station == null)
                return;

            // Raise to Silver, with announcement & sound
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

            // Must pass null for the 'target' parameter
            _chat.DispatchStationAnnouncement(
                station.Value,
                null,
                msg,
                "Central Command Supernatural Affairs",
                playDefaultSound: true);
        }
    }
}
