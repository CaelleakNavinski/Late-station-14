using Content.Server.GameTicking.Rules.Components;
using Content.Server.Roles;
using Content.Shared.GameTicking.Components;
using Content.Shared.Roles.Components;

namespace Content.Server.GameTicking.Rules;

/// <summary>
/// Baseline Vampire round-rule scaffold.
/// This pass only wires role briefings and establishes the rule type.
/// Turning, reversion, stake logic, and round-end evaluation will be added later.
/// </summary>
public sealed class VampireRuleSystem : GameRuleSystem<VampireRuleComponent>
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<VampireMatriarchRoleComponent, GetBriefingEvent>(OnGetMatriarchBriefing);
        SubscribeLocalEvent<VampireRoleComponent, GetBriefingEvent>(OnGetVampireBriefing);
    }

    private void OnGetMatriarchBriefing(Entity<VampireMatriarchRoleComponent> ent, ref GetBriefingEvent args)
    {
        args.Append(Loc.GetString("vamp-mat-briefing"));
    }

    private void OnGetVampireBriefing(Entity<VampireRoleComponent> ent, ref GetBriefingEvent args)
    {
        args.Append(Loc.GetString("vamp-briefing"));
    }

    protected override void Started(EntityUid uid, VampireRuleComponent component, GameRuleComponent gameRule, GameRuleStartedEvent args)
    {
        base.Started(uid, component, gameRule, args);
    }
}
