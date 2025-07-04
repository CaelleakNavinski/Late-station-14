using Content.Shared._LateStation.Vampires.Components;
using Content.Shared._LateStation.Vampires.Systems;
using Content.Shared.StatusIcon.Components;
using Robust.Shared.Prototypes;

namespace Content.Client._LateStation.Vampires.Systems;

/// <summary>
/// Used for the client to get status icons from other vamps.
/// </summary>
public sealed class VampireClientSystem : SharedVampireSystem
{
    [Dependency] private readonly IPrototypeManager _prototype = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<VampireComponent, GetStatusIconsEvent>(GetVampIcon);
        SubscribeLocalEvent<VampireMatriarchComponent, GetStatusIconsEvent>(GetVampMatIcon);
    }

    private void GetVampIcon(Entity<VampireComponent> ent, ref GetStatusIconsEvent args)
    {
        if (HasComp<VampireMatriarchComponent>(ent))
            return;

        if (_prototype.TryIndex(ent.Comp.StatusIcon, out var iconPrototype))
            args.StatusIcons.Add(iconPrototype);
    }

    private void GetVampMatIcon(Entity<VampireMatriarchComponent> ent, ref GetStatusIconsEvent args)
    {
        if (_prototype.TryIndex(ent.Comp.StatusIcon, out var iconPrototype))
            args.StatusIcons.Add(iconPrototype);
    }
}
