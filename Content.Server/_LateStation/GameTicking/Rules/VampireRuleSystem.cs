using Content.Server.Antag;
using Content.Server.GameTicking.Rules.Components;
using Content.Server.Mind;
using Content.Server.Roles;
using Content.Server.RoundEnd;
using Content.Shared.GameTicking.Components;
using Content.Shared.Roles.Components;
using Content.Shared._LateStation.Roles.Components;
using Content.Shared._LateStation.Vampire.Components;

namespace Content.Server.GameTicking.Rules;

/// <summary>
/// Baseline Vampire round-rule scaffold.
/// This pass wires role briefings, activates the Matriarch body-state,
/// and reports brood counts at round end.
/// </summary>
public sealed class VampireRuleSystem : GameRuleSystem<VampireRuleComponent>
{
    [Dependency] private readonly AntagSelectionSystem _antag = default!;
    [Dependency] private readonly MindSystem _mind = default!;

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

        ActivateMatriarchBodies();
    }

    protected override void AppendRoundEndText(
        EntityUid uid,
        VampireRuleComponent component,
        GameRuleComponent gameRule,
        ref RoundEndTextAppendEvent args)
    {
        base.AppendRoundEndText(uid, component, gameRule, ref args);

        var sessionData = _antag.GetAntagIdentifiers(uid);

        args.AddLine(Loc.GetString("vamp-mat-count"));

        foreach (var (mind, data, name) in sessionData)
        {
            if (!_mind.TryGetMind(mind, out _, out var mindComp))
                continue;

            if (mindComp.OwnedEntity is not { } body)
                continue;

            var count = CountBroodForMatriarch(body);

            args.AddLine(Loc.GetString(
                "vamp-mat-name-user",
                ("name", name),
                ("username", data.UserName),
                ("count", count)));
        }

        args.AddLine(string.Empty);
    }

    private void ActivateMatriarchBodies()
    {
        var query = EntityQueryEnumerator<VampireMatriarchRoleComponent>();

        while (query.MoveNext(out var mindId, out _))
        {
            if (!_mind.TryGetMind(mindId, out _, out var mind))
                continue;

            if (mind.OwnedEntity is not { } body)
                continue;

            var vamp = EnsureComp<VampireComponent>(body);
            vamp.CanConvert = true;
            vamp.Matriarch = body;
        }
    }

    private int CountBroodForMatriarch(EntityUid matriarchBody)
    {
        var count = 0;
        var query = EntityQueryEnumerator<VampireComponent>();

        while (query.MoveNext(out var uid, out var vampire))
        {
            if (uid == matriarchBody)
                continue;

            if (vampire.Matriarch != matriarchBody)
                continue;

            count++;
        }

        return count;
    }
}
