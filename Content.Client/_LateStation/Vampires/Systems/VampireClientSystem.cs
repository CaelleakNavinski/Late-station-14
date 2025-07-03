using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Content.Shared._LateStation.Vampires.Components;
using Content.Shared.StatusIcon;
using Content.Shared.StatusIcon.Components;

namespace Content.Client._LateStation.Vampires.Systems
{
    /// <summary>
    /// Used for the client to get vampire status icons
    /// via the unified GetStatusIconsEvent pipeline, mirroring RevolutionarySystem.
    /// </summary>
    public sealed class VampireClientSystem : SharedVampireSystem
    {
        [Dependency] private readonly IPrototypeManager _prototype = default!;

        public override void Initialize()
        {
            base.Initialize();
            SubscribeLocalEvent<SharedVampireComponent, GetStatusIconsEvent>(OnGetVampireIcon);
            SubscribeLocalEvent<SharedVampireMatriarchComponent, GetStatusIconsEvent>(OnGetMatriarchIcon);
        }

        private void OnGetVampireIcon(EntityUid uid, SharedVampireComponent comp, ref GetStatusIconsEvent args)
        {
            if (_prototype.TryIndex(comp.StatusIcon, out FactionIconPrototype iconProto))
            {
                args.StatusIcons.Add(iconProto);
            }
        }

        private void OnGetMatriarchIcon(EntityUid uid, SharedVampireMatriarchComponent comp, ref GetStatusIconsEvent args)
        {
            // Matriarchs use the same icon by default, or override via SharedVampireMatriarchComponent
            var protoId = comp.StatusIconOverride ?? comp.StatusIcon;
            if (_prototype.TryIndex(protoId, out FactionIconPrototype iconProto))
            {
                args.StatusIcons.Add(iconProto);
            }
        }
    }
}
