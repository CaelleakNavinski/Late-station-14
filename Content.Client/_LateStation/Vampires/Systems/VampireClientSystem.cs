using Content.Shared._LateStation.Vampires.Components;
using Content.Shared.StatusIcon.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;

namespace Content.Client._LateStation.Vampires.Systems
{
    /// <summary>
    /// Used for the client to get status icons from other vamps.
    /// </summary>
    public sealed class VampireClientSystem : EntitySystem
    {
        [Dependency] private readonly IPrototypeManager _prototype = default!;

        public override void Initialize()
        {
            base.Initialize();

            SubscribeLocalEvent<SharedVampireComponent, GetStatusIconsEvent>(GetVampIcon);
            SubscribeLocalEvent<SharedVampireMatriarchComponent, GetStatusIconsEvent>(GetVampMatIcon);
        }

        private void GetVampIcon(EntityUid uid, SharedVampireComponent comp, ref GetStatusIconsEvent args)
        {
            if (HasComp<SharedVampireMatriarchComponent>(uid))
                return;

            if (_prototype.TryIndex<FactionIconPrototype>(comp.StatusIcon, out var icon))
            {
                args.StatusIcons.Add(icon);
            }
        }

        private void GetVampMatIcon(EntityUid uid, SharedVampireMatriarchComponent comp, ref GetStatusIconsEvent args)
        {
            if (_prototype.TryIndex<FactionIconPrototype>(comp.StatusIcon, out var icon))
            {
                args.StatusIcons.Add(icon);
            }
        }
    }
}
