using System;
using Content.Shared.Actions;
using Content.Shared.Antag;                        // for ShowAntagIconsComponent
using Content.Shared._LateStation.Vampires.Components;
using Content.Shared.Popups;
using Robust.Shared.GameStates;
using Robust.Shared.GameObjects;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using Robust.Shared.IoC;

namespace Content.Shared._LateStation.Vampires.Systems
{
    /// <summary>
    /// Shared logic for vampire components: action hookup, state synchronization,
    /// and client visibility for vampire status icons via GetStatusIconsEvent.
    /// </summary>
    public abstract class SharedVampireSystem : EntitySystem
    {
        [Dependency] private readonly SharedActionsSystem _actions = default!;
        [Dependency] private readonly SharedPopupSystem _popupSystem = default!;

        public override void Initialize()
        {
            base.Initialize();

            // Hook up the bite action on init/shutdown
            SubscribeLocalEvent<SharedVampireComponent, ComponentInit>(OnVampireInit);
            SubscribeLocalEvent<SharedVampireComponent, ComponentShutdown>(OnVampireShutdown);

            // Control replication of vampire state to clients
            SubscribeLocalEvent<SharedVampireComponent, ComponentGetStateAttemptEvent>(OnVampCompGetStateAttempt);
            SubscribeLocalEvent<SharedVampireMatriarchComponent, ComponentGetStateAttemptEvent>(OnVampCompGetStateAttempt);

            // Ensure late‑joining clients get up‑to‑date vampire info
            SubscribeLocalEvent<SharedVampireComponent, ComponentStartup>(DirtyVampComps);
            SubscribeLocalEvent<SharedVampireMatriarchComponent, ComponentStartup>(DirtyVampComps);
        }

        private void OnVampireInit(EntityUid uid, SharedVampireComponent comp, ComponentInit args)
        {
            _actions.AddAction(uid, ref comp.BiteActionEntity, comp.BiteActionPrototype);
        }

        private void OnVampireShutdown(EntityUid uid, SharedVampireComponent comp, ComponentShutdown args)
        {
            _actions.RemoveAction(uid, comp.BiteActionEntity);
        }

        private void OnVampCompGetStateAttempt(EntityUid uid, IComponent comp, ref ComponentGetStateAttemptEvent args)
        {
            if (CanGetState(args.Player))
                return;

            args.Cancelled = true;
        }

        private bool CanGetState(ICommonSession? player)
        {
            if (player?.AttachedEntity is not { } ent)
                return true;

            // Vampires and Matriarchs always see their own state
            if (HasComp<SharedVampireComponent>(ent) || HasComp<SharedVampireMatriarchComponent>(ent))
                return true;

            // Other players see vampire icons only if they have the ShowAntagIconsComponent
            return HasComp<ShowAntagIconsComponent>(ent);
        }

        private void DirtyVampComps<T>(EntityUid uid, T _, ComponentStartup args) where T : IComponent
        {
            var vampQuery = AllEntityQuery<SharedVampireComponent>();
            while (vampQuery.MoveNext(out var id, out var vampComp))
                Dirty(id, vampComp);

            var matQuery = AllEntityQuery<SharedVampireMatriarchComponent>();
            while (matQuery.MoveNext(out var id2, out var matComp))
                Dirty(id2, matComp);
        }
    }
}
