using System;
using System.Linq;
using Content.Server.Actions;                            // ActionsSystem
using Content.Server.AlertLevel;                         // AlertLevelSystem
using Content.Server.Station.Systems;                    // StationSystem
using Content.Server.Chat.Systems;                       // ChatSystem
using Content.Shared.Mind.Components;                     // MindContainerComponent
using Content.Shared.Mind;                               // MindAddedMessage, MindRemovedMessage
using Content.Shared._LateStation.Vampires.Components;   // VampireComponent, VampireMatriarchComponent
using Robust.Server.Player;                              // IPlayerManager
using Robust.Shared.GameStates;                          // EntitySystem
using Robust.Shared.IoC;                                 // [Dependency]
using Robust.Shared.GameObjects;                         // EntityQuery<T>

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
            // Subscribe on the actual mind‐container entities
            SubscribeLocalEvent<MindContainerComponent, MindAddedMessage>(OnMindAdded);
            SubscribeLocalEvent<MindContainerComponent, MindRemovedMessage>(OnMindRemoved);
        }

        private void OnMindAdded(EntityUid uid, MindContainerComponent comp, MindAddedMessage args)
        {
            // args.MindRole isn't a thing—check the Mind's active role components instead:
            if (comp.Mind?.TryGetRole<Content.Shared._LateStation.Vampires.Components.VampireComponent>(out _) ?? false)
            {
                GrantVampire(uid);
                TryTriggerVioletAlert(uid);
            }
            else if (comp.Mind?.TryGetRole<Content.Shared._LateStation.Vampires.Components.VampireMatriarchComponent>(out _) ?? false)
            {
                if (!EntityManager.HasComponent<VampireMatriarchComponent>(uid))
                    EntityManager.AddComponent<VampireMatriarchComponent>(uid);
            }
        }

        private void OnMindRemoved(EntityUid uid, MindContainerComponent comp, MindRemovedMessage args)
        {
            // When the Mind loses its Vampire role component:
            if (!comp.Mind?.HasRole<VampireComponent>() ?? true)
                RemoveVampire(uid);

            if (!comp.Mind?.HasRole<VampireMatriarchComponent>() ?? true)
                EntityManager.RemoveComponent<VampireMatriarchComponent>(uid);
        }

        private void GrantVampire(EntityUid uid)
        {
            if (!EntityManager.HasComponent<VampireComponent>(uid))
                EntityManager.AddComponent<VampireComponent>(uid);

            _actions.AddAction(uid, "ActionVampireBite");
        }

        private void RemoveVampire(EntityUid uid)
        {
            if (EntityManager.HasComponent<VampireComponent>(uid))
                EntityManager.RemoveComponent<VampireComponent>(uid);

            _actions.RemoveAction(uid, "ActionVampireBite");
        }

        private void TryTriggerVioletAlert(EntityUid uid)
        {
            // Use LINQ Count() now that we imported System.Linq
            var totalVamps = EntityQuery<VampireComponent>().Count();
            var cap = Math.Max(3, (int)Math.Ceiling(_playerManager.PlayerCount * 0.2f));
            if (totalVamps != cap)
                return;

            var station = _stationSystem.GetOwningStation(uid);
            if (station == null)
                return;

            _alertLevelSystem.SetLevel(
                station.Value,
                "Violet",
                playSound: true,
                announce: false,
                force: true);

            const string warning =
                "Central Command Supernatural Affairs: We are detecting a growing vampiric threat aboard the station. Alert level has been raised to VIOLET.";

            // Parameters are: (source, message, title, playDefaultSound)
            _chatSystem.DispatchStationAnnouncement(
                station.Value,
                warning,
                "Central Command Supernatural Affairs",
                playDefaultSound: true);
        }
    }

    // Extension methods for checking MindRole components on the Mind
    public static class MindRoleExtensions
    {
        public static bool HasRole<T>(this MindComponent mind)
            where T : IComponent
        {
            return mind.OwnedEntity != null && 
                   EntityManager.HasComponent<T>(mind.OwnedEntity.Value);
        }

        public static bool TryGetRole<T>(this MindComponent mind, out T comp)
            where T : IComponent
        {
            if (mind.OwnedEntity != null && 
                EntityManager.TryGetComponent(mind.OwnedEntity.Value, out comp))
                return true;

            comp = default!;
            return false;
        }
    }
}
