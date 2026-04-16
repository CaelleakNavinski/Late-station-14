using Content.Shared.Examine;
using Content.Shared._LateStation.Vampire.Components;

namespace Content.Shared._LateStation.Vampire;

/// <summary>
/// Exposes visible bite-mark clues on turning victims.
/// </summary>
public sealed class VampireExamineSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<VampireTurningComponent, ExaminedEvent>(OnExamined);
    }

    private void OnExamined(EntityUid uid, VampireTurningComponent comp, ExaminedEvent args)
    {
        if (!args.IsInDetailsRange)
            return;

        if (!comp.HasBiteMarks)
            return;

        args.PushText(Loc.GetString("vamp-bite-marks-examine"));
    }
}
