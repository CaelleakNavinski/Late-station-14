using Content.Server.GameTicking.Rules.Components;
using Content.Server.Mind;
using Content.Server.Roles;
using Content.Shared.GameTicking.Components;
using Content.Shared._LateStation.Roles.Components;
using Content.Shared._LateStation.Vampire.Components;

namespace Content.Server.GameTicking.Rules;

/// <summary>
/// Baseline Vampire round-rule scaffold.
/// This pass wires role briefings and activates the Matriarch body-state.
/// </summary>
public sealed class VampireRuleSystem : GameRuleSystem<VampireRuleComponent>
{
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
}}
