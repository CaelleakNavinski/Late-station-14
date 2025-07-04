using System;
using System.Collections.Generic;
using Content.Server.Administration.Logs;
using Content.Server.Antag;
using Content.Server.Chat.Systems;
using Content.Server.GameTicking.Rules;
using Content.Server.GameTicking.Rules.Components;
using Content.Server.Mind;
using Content.Server.Roles;
using Content.Server.RoundEnd;
using Content.Server.Shuttles.Systems;
using Content.Server.Station.Systems;
using Content.Shared.GameTicking.Events;
using Content.Shared.IdentityManagement;
using Content.Shared.Mind.Components;
using Content.Shared.Mobs.Components;
using Content.Shared._LateStation.Vampires.Components;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Player;
using Robust.Shared.Random;

namespace Content.Server._LateStation.GameTicking.Rules
{
    public sealed class VampireRuleSystem : GameRuleSystem<VampireRuleComponent>
    {
        [Dependency] private readonly AntagSelectionSystem _antag = default!;
        [Dependency] private readonly IAdminLogManager _adminLog = default!;
        [Dependency] private readonly IGameTiming _timing = default!;
        [Dependency] private readonly ISharedPlayerManager _players = default!;
        [Dependency] private readonly MindSystem _mind = default!;
        [Dependency] private readonly RoleSystem _role = default!;
        [Dependency] private readonly RoundEndSystem _roundEnd = default!;
        [Dependency] private readonly StationSystem _station = default!;
        [Dependency] private readonly EmergencyShuttleSystem _shuttle = default!;
        [Dependency] private readonly IRobustRandom _random = default!;

        public override void Initialize()
        {
            base.Initialize();

            SubscribeLocalEvent<VampireRoleComponent, GetBriefingEvent>(OnGetBriefing);
            SubscribeLocalEvent<VampireRuleComponent, RoundEndTextAppendEvent>(OnAppendRoundEndText);
        }

        private void OnGetBriefing(EntityUid uid, VampireRoleComponent comp, ref GetBriefingEvent args)
        {
            var ent = args.Mind.Comp.OwnedEntity;
            var isMatriarch = HasComp<VampireMatriarchComponent>(ent);
            args.Append(Loc.GetString(isMatriarch ? "vamp-mat-briefing" : "vamp-briefing"));
        }

        private void OnAppendRoundEndText(EntityUid uid, VampireRuleComponent comp, RoundEndTextAppendEvent args)
        {
            // Determine outcomes
            var matriarchs = AllEntityQuery<VampireMatriarchComponent>();
            var aliveMat = 0;
            foreach (var (ent, _) in matriarchs)
            {
                if (TryComp<MobStateComponent>(ent, out var state) && state.CurrentState != MobState.Dead)
                    aliveMat++;
            }

            var totalPlayers = _players.Sessions.Count;
            var converted = AllEntityQuery<SharedVampireComponent>()
                .Count(e => !HasComp<VampireMatriarchComponent>(e.Item1));
            var required = (int)Math.Floor(totalPlayers * 0.4f);

            string outcome;
            if (aliveMat > 0 && converted >= required)
                outcome = "vamp-won";
            else if (aliveMat <= 0)
                outcome = "vamp-lost";
            else
                outcome = "vamp-stalemate";

            args.AddLine(Loc.GetString(outcome));
            args.AddLine(Loc.GetString("vamp-mat-count"));

            var sessionData = _antag.GetAntagIdentifiers(uid);
            foreach (var (mind, data, name) in sessionData)
            {
                if (_role.MindHasRole<VampireRoleComponent>(mind, out var role))
                {
                    var count = role.Value.Comp2.ConvertedCount;
                    args.AddLine(Loc.GetString("vamp-mat-name-user",
                        ("name", name),
                        ("username", data.UserName),
                        ("count", count)));
                }
            }
        }
    }
}
