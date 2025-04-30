using System.Linq;
using Content.Server.AlertLevel;                         // AlertLevelSystem
using Content.Server.Station.Systems;                    // StationSystem
using Content.Server.Chat.Systems;                       // ChatSystem
using Content.Shared._LateStation.Vampires.Components;   // VampireComponent, VampireMatriarchComponent
using Content.Shared.Actions;                            // SharedActionsSystem, ActionsComponent
using Robust.Server.Player;                              // IPlayerManager
using Robust.Shared.GameStates;                          // EntitySystem, ComponentInit, ComponentShutdown
using Robust.Shared.IoC;                                 // [Dependency]
using Robust.Shared.GameObjects;                         // SubscribeLocalEvent, EntityQuery

namespace Content.Server._LateStation.Vampires.Systems
{
    /// <summary>
    /// Manages giving/removing the bite action (PAI style) and triggers the Silver alert cap.
    /// </summary>
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

            // Stub in Matriarch for future expansion
            SubscribeLocalEvent<VampireMatriarchComponent, ComponentInit>((_, _, _) => { });
            SubscribeLocalEvent<VampireMatriarchComponent, ComponentShutdown>((_, _, _) => { });
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
            if (comp.BiteActionEntity != null)
            {
                _actionsSystem.RemoveAction(uid, comp.BiteActionEntity.Value);
                comp.BiteActionEntity = null;
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
                station.Value,                   // source console/station
                msg,                             // the announcement text
                playDefaultSound: false);
        }
    }
}
