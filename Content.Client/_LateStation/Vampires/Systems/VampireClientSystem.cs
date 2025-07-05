using Content.Shared._LateStation.Vampires.Components;
using Content.Shared._LateStation.Vampires.Systems;
using Content.Shared.StatusIcon.Components;
using Robust.Shared.Prototypes;

namespace Content.Client._LateStation.Vampires.Systems
{
    /// <summary>
    /// Shows vampire and matriarch status icons on the client.
    /// </summary>
    public sealed class VampireClientSystem : SharedVampireSystem
    {
        [Dependency] private readonly IPrototypeManager _prototype = default!;

        public override void Initialize()
        {
            base.Initialize();

            SubscribeLocalEvent<SharedVampireComponent, GetStatusIconsEvent>(GetVampIcon);
            SubscribeLocalEvent<SharedVampireMatriarchComponent, GetStatusIconsEvent>(GetVampMatIcon);
        }

        private void GetVampIcon(Entity<SharedVampireComponent> ent, ref GetStatusIconsEvent args)
        {
            if (HasComp<SharedVampireMatriarchComponent>(ent))
                return;

            if (_prototype.TryIndex(ent.Comp.StatusIcon, out var iconPrototype))
                args.StatusIcons.Add(iconPrototype);
        }
    
        private void GetVampMatIcon(Entity<SharedVampireMatriarchComponent> ent, ref GetStatusIconsEvent args)
        {
            if (_prototype.TryIndex(ent.Comp.StatusIcon, out var iconPrototype))
                args.StatusIcons.Add(iconPrototype);
        }
    }
}
