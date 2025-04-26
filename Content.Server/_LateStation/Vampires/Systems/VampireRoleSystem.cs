using Content.Server.Actions;
using Content.Server.Chat.Systems;
using Content.Server.Station.Systems;
using Content.Server._LateStation.Vampires.Components;
using Content.Shared.Mind;
using Content.Shared.Vampire.Components;
using Robust.Server.GameObjects;
using Robust.Server.Player;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;

namespace Content.Server._LateStation.Vampires.Systems
{
    /// <summary>
    /// Assigns and removes vampire roles (regular & Matriarch),
    /// enforces a cap on total vampires, and raises Violet alert when cap is reached.
    /// </summary>
    public sealed class VampireRoleSystem : EntitySystem
    {
        [Dependency] private readonly ActionSystem _actionSystem = default!;
        [Dependency] private readonly IPlayerManager _playerManager = default!;
        [Dependency] private readonly AlertLevelSystem _alertLevelSystem = default!;
        [Dependency] private readonly StationSystem _stationSystem = default!;
        [Dependency] private readonly ChatSystem _chatSystem = default!;

        public override void Initialize()
        {
            base.Initialize();

            // Listen for mind‐role assignments and removals
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
            {<=
                case "MindRoleVampire":
                    RemoveVampire(uid);
                    break;

                case "MindRoleVampireMatriarch":
                    if (EntityManager.HasComponent<VampireMatriarchComponent>(uid))
                        EntityManager.RemoveComponent<VampireMatriarchComponent>(uid);
                    break;
            }
        }

        /// <summary>
        /// Adds the VampireComponent and bite action to a new vampire.
        /// </summary>
        private void GrantVampire(EntityUid uid)
        {
            if (!EntityManager.HasComponent<VampireComponent>(uid))
                EntityManager.AddComponent<VampireComponent>(uid);

            _actionSystem.AddAction(uid, "ActionVampireBite");
        }

        /// <summary>
        /// Removes the VampireComponent and bite action from an entity.
        /// </summary>
        private void RemoveVampire(EntityUid uid)
        {
            if (EntityManager.HasComponent<VampireComponent>(uid))
                EntityManager.RemoveComponent<VampireComponent>(uid);

            _actionSystem.RemoveAction(uid, "ActionVampireBite");
        }

        /// <summary>
        /// When the vampire count reaches the cap (max(3, 20% of players)),
        /// raises station alert to Violet and announces.
        /// </summary>
        private void TryTriggerVioletAlert(EntityUid uid)
        {
            var totalVamps = EntityQuery<VampireComponent>().Count();
            var cap = Math.Max(3, (int) Math.Ceiling(_playerManager.PlayerCount * 0.2f));

            if (totalVamps <= cap)
                return;

            var station = _stationSystem.GetOwningStation(uid);
            if (station == null)
                return;

            // Force Violet without default broadcast
            _alertLevelSystem.SetLevel(station.Value, "Silver", playSound: true, announce: false, force: true);

            // Custom station announcement
            const string warning =
                "Scans have detected a significant escalation in vampiric activity aboard the station. Remain within your respective departments and report any suspicious behavior to Security or the Chaplain. Avoid isolated areas and travel in groups when possible.";
            _chatSystem.DispatchStationAnnouncement(
                station.Value,
                warning,
                playDefaultSound: true,
                sender: "Central Command Supernatural Affairs"
            );
        }
    }
}
