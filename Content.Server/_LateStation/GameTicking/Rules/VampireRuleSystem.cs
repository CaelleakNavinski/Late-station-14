using Content.Server.Administration.Logs;
using Content.Server.Antag;
using Content.Server.EUI;
using Content.Server._LateStation.GameTicking.Rules.Components;
using Content.Server.Mind;
using Content.Server.Roles;
using Content.Server.RoundEnd;
using Content.Server.Shuttles.Systems;
using Content.Server.Station.Systems;
using Content.Shared.Antag;
using Content.Shared.Database;
using Content.Shared.Mind.Components;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared._LateStation.Vampires.Components;
using Content.Shared._LateStation.Vampires.Systems;
using Robust.Shared.Player;

namespace Content.Server.GameTicking.Rules;

public sealed class VampireRuleSystem : GameRuleSystem<VampireRuleComponent>
{
    [Dependency] private readonly AntagSelectionSystem _antag = default!;
    [Dependency] private readonly EuiManager _eui = default!;
    [Dependency] private readonly MindSystem _mind = default!;
    [Dependency] private readonly RoleSystem _roles = default!;
    [Dependency] private readonly IAdminLogManager _logs = default!;
    [Dependency] private readonly RoundEndSystem _roundEnd = default!;
    [Dependency] private readonly StationSystem _station = default!;
    [Dependency] private readonly EmergencyShuttleSystem _shuttle = default!;
    [Dependency] private readonly ISharedPlayerManager _players = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<VampireRoleComponent, GetBriefingEvent>(OnGetBriefing);
    }

    private void OnGetBriefing(EntityUid uid, VampireRoleComponent comp, ref GetBriefingEvent args)
    {
        var ent = args.Mind.Comp.OwnedEntity;
        var mat = HasComp<VampireMatriarchComponent>(ent);
        args.Append(Loc.GetString(mat ? "vamp-mat-briefing" : "vamp-briefing"));
    }

    protected override void AppendRoundEndText(EntityUid uid, VampireRuleComponent comp, GameRuleComponent game, ref RoundEndTextAppendEvent args)
    {
        base.AppendRoundEndText(uid, comp, game, ref args);

        var matList = AllEntityQuery<VampireMatriarchComponent>();
        var matriarchs = new List<EntityUid>();

        while (matList.MoveNext(out var mat, out _))
            matriarchs.Add(mat);

        if (matriarchs.Count == 0)
        {
            args.AddLine(Loc.GetString("vamp-draw"));
            return;
        }

        var survived = matriarchs.Any(m => TryComp<MobStateComponent>(m, out var state) && state.CurrentState != MobState.Dead);

        var vampireCount = AllEntityQuery<SharedVampireComponent>().ToList().Count;
        var playerCount = _players.Sessions.Count();
        var convertedRatio = vampireCount / (float)Math.Max(1, playerCount);
        var requiredRatio = 2f / 5f; // 40%

        string outcome;
        if (survived && convertedRatio >= requiredRatio)
            outcome = "vamp-won";
        else if (!survived)
            outcome = "vamp-lost";
        else
            outcome = "vamp-stalemate";

        args.AddLine(Loc.GetString(outcome));

        args.AddLine(Loc.GetString("vamp-mat-count"));
        var sessionData = _antag.GetAntagIdentifiers(uid);
        foreach (var (mind, data, name) in sessionData)
        {
            if (_roles.MindHasRole<VampireRoleComponent>(mind, out var role))
            {
                var count = role.Value.Comp2.ConvertedCount;
                args.AddLine(Loc.GetString("vamp-mat-name-user", ("name", name), ("username", data.UserName), ("count", count)));
            }
        }
    }
}
