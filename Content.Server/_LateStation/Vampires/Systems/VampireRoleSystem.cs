using Content.Server.Actions;
using Content.Shared.Mind;
using Content.Shared.Vampire.Components;
using Robust.Server.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;

namespace Content.Server._LateStation.Vampires.Systems
{
    /// <summary>
    /// Handles assigning vampire roles and their marker components/actions
    /// when a Mind is added or removed.
    /// </summary>
    public sealed class VampireRoleSystem : EntitySystem
    {
        [Dependency] private readonly ActionSystem _actionSystem = default!;

        public override void Initialize()
        {
            base.Initialize();

            // When a vampire mind is assigned at round start (or conversion),
            // give them the VampireComponent + bite action.
            SubscribeLocalEvent<MindAddedMessage>(OnMindAdded);

            // When the mind loses its vampire role (cloning, borging, etc.),
            // remove components & actions.
            SubscribeLocalEvent<MindRemovedMessage>(OnMindRemoved);
        }

        private void OnMindAdded(EntityUid uid, MindAddedMessage msg)
        {
            switch (msg.MindRole)
            {
                case "MindRoleVampire":
                    // Grant the base VampireComponent and bite ability
                    if (!EntityManager.HasComponent<VampireComponent>(uid))
                        EntityManager.AddComponent<VampireComponent>(uid);

                    _actionSystem.AddAction(uid, "ActionVampireBite");
                    break;

                case "MindRoleVampireMatriarch":
                    // Tag as Matriarch for future exclusive powers
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
                    if (EntityManager.HasComponent<VampireComponent>(uid))
                        EntityManager.RemoveComponent<VampireComponent>(uid);

                    _actionSystem.RemoveAction(uid, "ActionVampireBite");
                    break;

                case "MindRoleVampireMatriarch":
                    if (EntityManager.HasComponent<VampireMatriarchComponent>(uid))
                        EntityManager.RemoveComponent<VampireMatriarchComponent>(uid);
                    break;
            }
        }
    }
}
