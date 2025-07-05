using Content.Shared._LateStation.Vampires.Components;
using Content.Shared._LateStation.Vampires.Systems;
using Content.Shared.StatusIcon;                  // FactionIconPrototype
using Content.Shared.StatusIcon.Components;       // GetStatusIconsEvent
using Robust.Shared.Prototypes;                  // IPrototypeManager
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Client._LateStation.Vampires.Systems
{
    /// <summary>
    /// Client‐side: adds the vampire and matriarch status icons to the HUD.
    /// </summary>
    public sealed class VampireClientSystem : SharedVampireSystem
    {
        [Dependency] private readonly IPrototypeManager _prototype = default!;

        public override void Initialize()
        {
            base.Initialize();

            SubscribeLocalEvent<SharedVampireComponent, GetStatusIconsEvent>(OnVampireIcon);
            SubscribeLocalEvent<SharedVampireMatriarchComponent, GetStatusIconsEvent>(OnMatriarchIcon);
        }

        private void OnVampireIcon(EntityUid uid, SharedVampireComponent comp, ref GetStatusIconsEvent args)
        {
            if (HasComp<SharedVampireMatriarchComponent>(uid))
                return;

            if (_prototype.TryIndex(comp.StatusIcon, out FactionIconPrototype icon) && icon != null)
                args.StatusIcons.Add(icon);
        }

        private void OnMatriarchIcon(EntityUid uid, SharedVampireMatriarchComponent matComp, ref GetStatusIconsEvent args)
        {

            if (_prototype.TryIndex(matComp.StatusIcon, out FactionIconPrototype icon) && icon != null)
                args.StatusIcons.Add(icon);
        }
    }
}
