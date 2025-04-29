using System.Linq;
using Content.Server.Actions;                          // ActionsSystem
using Content.Server.AlertLevel;                       // AlertLevelSystem
using Content.Server.Station.Systems;                  // StationSystem
using Content.Server.Chat.Systems;                     // ChatSystem
using Content.Shared._LateStation.Vampires.Components; // VampireComponent, VampireMatriarchComponent
using Robust.Server.Player;                            // IPlayerManager
using Robust.Shared.GameStates;                        // EntitySystem, ComponentInit, ComponentShutdown
using Robust.Shared.IoC;                               // [Dependency]
using Robust.Shared.GameObjects;                       // EntityQuery

namespace Content.Server._LateStation.Vampires.Systems
{
    /// <summary>
    /// Manages bite-action granting and Silver-alert triggering based on VampireComponent.
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

            // Give / remove bite when VampireComponent is added/removed.
            SubscribeLocalEvent<VampireComponent, ComponentInit>(OnVampireInit);
            SubscribeLocalEvent<VampireComponent, ComponentShutdown>(OnVampireShutdown);

            // Reserve Matriarch hooks (empty handlers with correct signature).
            SubscribeLocalEvent<VampireMatriarchComponent, ComponentInit>(
                (uid, comp, args) => { /* future leader abilities */ });
            SubscribeLocalEvent<VampireMatriarchComponent, ComponentShutdown>(
                (uid, comp, args) => { /* cleanup */ });
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

            // Raise to Silver with announcement & sound
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
                station.Value,                   // source console/station
                station.Value,                   // channel/station again for broadcast
                msg,                             // the announcement text
                "Central Command Supernatural Affairs", // title/sender name
                true);                           // playDefaultSound
        }
    }
}
