using Content.Shared.Actions;
using Robust.Shared.GameStates;
using Robust.Shared.GameObjects;
using Content.Shared._LateStation.Vampires.Components;

namespace Content.Shared._LateStation.Vampires.Systems
{
    /// <summary>
    /// Shared stub so that [Access(typeof(SharedVampireSystem))] resolves
    /// and so that both client and server wire up the bite action toggle.
    /// </summary>
    public sealed class SharedVampireSystem : EntitySystem
    {
        [Dependency] private readonly SharedActionsSystem _actions = default!;

        public override void Initialize()
        {
            base.Initialize();
            SubscribeLocalEvent<SharedVampireComponent, ComponentInit>(OnVampireInit);
            SubscribeLocalEvent<SharedVampireComponent, ComponentShutdown>(OnVampireShutdown);
        }

        private void OnVampireInit(EntityUid uid, SharedVampireComponent comp, ComponentInit args)
        {
            _actions.AddAction(uid, ref comp.BiteActionEntity, comp.BiteActionPrototype);
        }

        private void OnVampireShutdown(EntityUid uid, SharedVampireComponent comp, ComponentShutdown args)
        {
            _actions.RemoveAction(uid, comp.BiteActionEntity);
        }
    }
}