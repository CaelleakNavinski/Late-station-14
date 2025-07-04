using System;
using Content.Shared.Actions;
using Content.Shared.Popups;
using Content.Shared.StatusIcon.Components;
using Content.Shared._LateStation.Vampires.Components;
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
    public sealed class SharedVampireSystem : EntitySystem
    {
        [Dependency] private readonly SharedActionsSystem _actions = default!;
        [Dependency] private readonly SharedPopupSystem _popupSystem = default!;

        public override void Initialize()
        {
            base.Initialize();

            // Hook up the bite action
            SubscribeLocalEvent<SharedVampireComponent, ComponentInit>(OnVampireInit);
            SubscribeLocalEvent<SharedVampireComponent, ComponentShutdown>(OnVampireShutdown);

            // Control replication of vampire state to clients
            SubscribeLocalEvent<SharedVampireComponent, ComponentGetStateAttemptEvent>(OnVampCompGetStateAttempt);
            SubscribeLocalEvent<SharedVampireMatriarchComponent, ComponentGetStateAttemptEvent>(OnVampCompGetStateAttempt);

            // Ensure new clients get up-to-date vampire info
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

        private void OnVampCompGetStateAttempt(EntityUid uid, Component comp, ref ComponentGetStateAttemptEvent args)
        {
            if (CanGetState(args.Player))
                return;

            args.Cancelled = true;
        }

        private bool CanGetState(ICommonSession? player)
        {
            if (player?.AttachedEntity is not {} ent)
                return true;

            // Vampires and Matriarchs always see their own state
            if (HasComp<SharedVampireComponent>(ent) || HasComp<SharedVampireMatriarchComponent>(ent))
                return true;

            // Other players see vampire icons only if allowed
            return HasComp<ShowAntagIconsComponent>(ent);
        }

        private void DirtyVampComps<T>(EntityUid uid, T comp, ComponentStartup args)
        {
            // Force resend of all vampire and matriarch components
            var vampQuery = AllEntityQuery<SharedVampireComponent>();
            while (vampQuery.MoveNext(out var id, out var _))
                Dirty(id);

            var matQuery = AllEntityQuery<SharedVampireMatriarchComponent>();
            while (matQuery.MoveNext(out var id2, out var _))
                Dirty(id2);
        }
    }
}
