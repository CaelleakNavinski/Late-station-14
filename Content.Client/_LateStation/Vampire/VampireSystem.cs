using System;
using Content.Client.Alerts;
using Content.Shared._LateStation.Vampire.Components;
using Content.Shared.StatusIcon.Components;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Client._LateStation.Vampire;

/// <summary>
/// Client-side vampire status icon handling.
/// Mirrors the Revolutionary/Zombie pattern by resolving the component's
/// faction icon prototype during GetStatusIconsEvent.
/// </summary>
public sealed class VampireSystem : EntitySystem
{
    private const string BloodAlertId = "VampireBloodMeter";

    [Dependency] private readonly IPrototypeManager _prototype = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<VampireComponent, GetStatusIconsEvent>(GetVampireIcon);
        SubscribeLocalEvent<VampireMatriarchComponent, GetStatusIconsEvent>(GetMatriarchIcon);
        SubscribeLocalEvent<VampireComponent, GetAlertTooltipEvent>(OnGetAlertTooltip);
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

    private void OnGetAlertTooltip(Entity<VampireComponent> ent, ref GetAlertTooltipEvent args)
    {
        if (args.Handled || args.Alert.ID != BloodAlertId)
            return;

        var msg = new FormattedMessage();
        msg.AddText(Loc.GetString("alerts-vampire-blood-desc"));
        msg.PushNewline();
        msg.AddText(Loc.GetString(
            "alerts-vampire-blood-current",
            ("blood", (int) MathF.Round(ent.Comp.Blood, MidpointRounding.AwayFromZero)),
            ("max", (int) MathF.Round(ent.Comp.MaxBlood, MidpointRounding.AwayFromZero))));

        args.Description = msg;
    }
}
