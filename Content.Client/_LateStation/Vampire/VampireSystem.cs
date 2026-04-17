using Content.Shared._LateStation.Vampire.Components;
using Content.Shared.StatusIcon.Components;
using Robust.Shared.Prototypes;

namespace Content.Client._LateStation.Vampire;

/// <summary>
/// Client-side vampire status icon handling.
/// Mirrors the Revolutionary/Zombie pattern by resolving the component's
/// faction icon prototype during GetStatusIconsEvent.
/// </summary>
public sealed class VampireSystem : EntitySystem
{
    [Dependency] private readonly IPrototypeManager _prototype = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<VampireComponent, GetStatusIconsEvent>(GetVampireIcon);
        SubscribeLocalEvent<VampireMatriarchComponent, GetStatusIconsEvent>(GetMatriarchIcon);
    }

    private void GetVampireIcon(Entity<VampireComponent> ent, ref GetStatusIconsEvent args)
    {
        if (HasComp<VampireMatriarchComponent>(ent))
            return;

        if (_prototype.Resolve(ent.Comp.StatusIcon, out var iconPrototype))
            args.StatusIcons.Add(iconPrototype);
    }

    private void GetMatriarchIcon(Entity<VampireMatriarchComponent> ent, ref GetStatusIconsEvent args)
    {
        if (_prototype.Resolve(ent.Comp.StatusIcon, out var iconPrototype))
            args.StatusIcons.Add(iconPrototype);
    }
}
