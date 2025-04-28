using System;
using Content.Server.Actions;                          // ActionSystem
using Content.Server.AlertLevel;                       // AlertLevelSystem
using Content.Server.Station.Systems;                  // StationSystem
using Content.Server.Chat.Systems;                     // ChatSystem
using Content.Shared.Mind;                              // MindAddedMessage/MindRemovedMessage
using Content.Shared._LateStation.Vampires.Components; // VampireComponent, VampireMatriarchComponent
using Robust.Server.Player;                            // IPlayerManager
using Robust.Shared.GameStates;                        // EntitySystem
using Robust.Shared.IoC;                               // [Dependency]
using Robust.Shared.GameObjects;                       // EntityQuery<T>

namespace Content.Server._LateStation.Vampires.Systems
{
    public sealed class VampireRoleSystem : EntitySystem
    {
        [Dependency] private readonly ActionSystem _actionSystem = default!;
        [Dependency] private readonly IPlayerManager _playerManager = default!;
        [Dependency] private readonly AlertLevelSystem _alertLevelSystem = default!;
        [Dependency] private readonly StationSystem _stationSystem = default!;
        [Dependency] private readonly ChatSystem _chatSystem = default!;

        public override void Initialize()
        {
            SubscribeLocalEvent<MindAddedMessage>(OnMindAdded);
            SubscribeLocalEvent<MindRemovedMessage>(OnMindRemoved);
        }

        private void OnMindAdded(EntityUid uid, MindAddedMessage msg)
        {
            switch (msg.MindRole)
            {
                case "MindRoleVampire":
                    GrantVampire(uid);
                    TryTriggerVioletAlert(uid);
                    break;
                case "MindRoleVampireMatriarch":
                    if (!EntityManager.HasComponent<VampireMatriarchComponent>(uid))
                        EntityManager.AddComponent<VampireMatriarchComponent>(uid);
                    break;
            }
        }

        private void OnMindRemoved(EntityUid uid, MindRemovedMessage msg)
        {
            switch (msg.MindRole)
            {
                case "MindRoleVampire":
                    RemoveVampire(uid);
                    break;
                case "MindRoleVampireMatriarch":
                    if (EntityManager.HasComponent<VampireMatriarchComponent>(uid))
                        EntityManager.RemoveComponent<VampireMatriarchComponent>(uid);
                    break;
            }
        }

        private void GrantVampire(EntityUid uid)
        {
            if (!EntityManager.HasComponent<VampireComponent>(uid))
                EntityManager.AddComponent<VampireComponent>(uid);
            _actionSystem.AddAction(uid, "ActionVampireBite");
        }

        private void RemoveVampire(EntityUid uid)
        {
            if (EntityManager.HasComponent<VampireComponent>(uid))
                EntityManager.RemoveComponent<VampireComponent>(uid);
            _actionSystem.RemoveAction(uid, "ActionVampireBite");
        }

        private void TryTriggerSilverAlert(EntityUid uid)
        {
            var total = EntityQuery<VampireComponent>().Count();
            var cap = Math.Max(3, (int)Math.Ceiling(_playerManager.PlayerCount * 0.2f));
            if (total != cap) return;

            var station = _stationSystem.GetOwningStation(uid);
            if (station == null) return;

            _alertLevelSystem.SetLevel(station.Value, "Silver", playSound: true, announce: true, force: true);

            // Custom station announcement
            const string warning =
                "Scans have detected a significant escalation in vampiric activity aboard the station. Remain within your respective departments and report any suspicious behavior to Security or the Chaplain. Avoid isolated areas and travel in groups when possible.";
            _chatSystem.DispatchStationAnnouncement(
                station.Value,
                warning,
                playDefaultSound: true,
                sender: "Central Command Supernatural Affairs"
        }
    }
}
