using System.Linq;
using Content.Server.Actions;                             // ← ActionsSystem
using Content.Server.AlertLevel;                          // ← AlertLevelSystem
using Content.Server.Station.Systems;                     // ← StationSystem
using Content.Server.Chat.Systems;                        // ← ChatSystem
using Content.Shared.Mind;                                // ← MindAddedMessage / MindRemovedMessage
using Content.Shared.Mind.Components;                      // ← MindContainerComponent
using Content.Shared._LateStation.Vampires.Components;
using Robust.Server.Player;                               // ← IPlayerManager
using Robust.Shared.GameStates;                           // ← EntitySystem
using Robust.Shared.IoC;                                  // ← [Dependency]
using Robust.Shared.GameObjects;                          // ← EntityQuery<T>

namespace Content.Server._LateStation.Vampires.Systems
{
    public sealed class VampireRoleSystem : EntitySystem
    {
        [Dependency] private readonly ActionsSystem _actions = default!;
        [Dependency] private readonly IPlayerManager _playerManager = default!;
        [Dependency] private readonly AlertLevelSystem _alertLevelSystem = default!;
        [Dependency] private readonly StationSystem _stationSystem = default!;
        [Dependency] private readonly ChatSystem _chatSystem = default!;

        public override void Initialize()
        {
            // Hook into when a Mind is added/removed on any body
            SubscribeLocalEvent<MindContainerComponent, MindAddedMessage>(OnMindAdded);
            SubscribeLocalEvent<MindContainerComponent, MindRemovedMessage>(OnMindRemoved);
        }

        private void OnMindAdded(EntityUid uid, MindContainerComponent comp, MindAddedMessage args)
        {
            // If the Mind just gained a VampireComponent on its OwnedEntity, equip them:
            if (comp.Mind?.OwnedEntity is { } owned && EntityManager.HasComponent<VampireComponent>(owned))
            {
                _actions.AddAction(owned, "ActionVampireBite");
                TryTriggerSilverAlert(owned);
            }

            // If they gained the Matriarch role component, tag it:
            if (comp.Mind?.OwnedEntity is { } own2 && EntityManager.HasComponent<VampireMatriarchComponent>(own2))
            {
                // marker only—no abilities yet
            }
        }

        private void OnMindRemoved(EntityUid uid, MindContainerComponent comp, MindRemovedMessage args)
        {
            // If they lost VampireComponent, strip bite
            if (comp.Mind?.OwnedEntity is { } owned && !EntityManager.HasComponent<VampireComponent>(owned))
            {
                _actions.RemoveAction(owned, "ActionVampireBite");
            }
        }

        private void TryTriggerSilverAlert(EntityUid uid)
        {
            var totalVamps = EntityQuery<VampireComponent>().Count();
            var cap = Math.Max(3, (int)Math.Ceiling(_playerManager.PlayerCount * 0.2f));
            if (totalVamps != cap) return;

            var station = _stationSystem.GetOwningStation(uid);
            if (station == null) return;

            _alertLevelSystem.SetLevel(
                station.Value,
                "Silver",
                playSound: true,
                announce: true,
                force: true);

            const string warning =
                "Scans have detected a significant escalation in vampiric activity aboard the station. " +
                "Remain within your respective departments and report any suspicious behavior to Security or the Chaplain. " +
                "Avoid isolated areas and travel in groups when possible.";

            _chatSystem.DispatchStationAnnouncement(
                station.Value,
                warning,
                "Central Command Supernatural Affairs",
                playDefaultSound: true);
        }
    }
}
