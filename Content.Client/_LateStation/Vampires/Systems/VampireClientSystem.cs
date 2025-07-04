using Content.Shared._LateStation.Vampires.Components;
using Content.Shared.StatusIcon;
using Content.Shared.StatusIcon.Components;    // GetStatusIconsEvent
using Robust.Client.GameObjects;              // EntityUid extensions
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;              // ProtoId<T>
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Client._LateStation.Vampires.Systems
{
    /// <summary>
    /// Shows vampire and matriarch status icons on the client.
    /// </summary>
    public sealed class VampireClientSystem : EntitySystem
    {
        [Dependency] private readonly IPrototypeManager _prototype = default!;

        public override void Initialize()
        {
            base.Initialize();
            // First show regular vampire icon if not a matriarch
            SubscribeLocalEvent<SharedVampireComponent, GetStatusIconsEvent>(OnVampireIcon);
            // Then show matriarch icon (overrides vampire icon)
            SubscribeLocalEvent<SharedVampireMatriarchComponent, GetStatusIconsEvent>(OnMatriarchIcon);
        }

        private void OnVampireIcon(EntityUid uid, SharedVampireComponent comp, ref GetStatusIconsEvent args)
        {
            // Matriarchs will get handled by OnMatriarchIcon instead
            if (HasComp<SharedVampireMatriarchComponent>(uid))
                return;

            if (_prototype.TryIndex<FactionIconPrototype>(comp.StatusIcon, out var icon))
                args.StatusIcons.Add(icon);
        }

        private void OnMatriarchIcon(EntityUid uid, SharedVampireMatriarchComponent _, ref GetStatusIconsEvent args)
        {
            // SharedVampireMatriarchComponent is only a marker; pull icon from SharedVampireComponent
            if (!EntityManager.TryGetComponent(uid, out SharedVampireComponent vampComp))
                return;

            if (_prototype.TryIndex<FactionIconPrototype>(vampComp.StatusIcon, out var icon))
                args.StatusIcons.Add(icon);
        }
    }
}
