using System;
using System.Linq;
using Content.Server.Administration.Logs;
using Content.Server.Antag;
using Content.Server.GameTicking.Rules;
using Content.Server.Roles;
using Content.Server._LateStation.GameTicking.Rules.Components;
using Content.Server.GameTicking
using Content.Shared.Mind.Components;
using Content.Shared.Mobs.Components;
using Content.Shared._LateStation.Vampires.Components;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Player;
using Robust.Shared.Random;

namespace Content.Server._LateStation.GameTicking.Rules
{
    /// <summary>
    /// Controls Vampire matriarch assignment and win/loss based on converting ≥2/5 of the crew.
    /// </summary>
    public sealed class VampireRuleSystem : GameRuleSystem<VampireRuleComponent>
    {
        [Dependency] private readonly AntagSelectionSystem _antag = default!;
        [Dependency] private readonly ISharedPlayerManager _players = default!;
        [Dependency] private readonly RoleSystem _role = default!;

        public override void Initialize()
        {
            base.Initialize();
            SubscribeLocalEvent<VampireRuleComponent, RoundEndTextAppendEvent>(OnAppendRoundEndText);
            SubscribeLocalEvent<VampireRoleComponent, GetBriefingEvent>(OnGetBriefing);
        }

        private void OnGetBriefing(EntityUid uid, VampireRoleComponent comp, ref GetBriefingEvent args)
        {
            var ent = args.Mind.Comp.OwnedEntity;
            var isMatriarch = HasComp<VampireMatriarchComponent>(ent);
            args.Append(Loc.GetString(isMatriarch ? "vamp-mat-briefing" : "vamp-briefing"));
        }

        private void OnAppendRoundEndText(EntityUid uid, VampireRuleComponent comp, RoundEndTextAppendEvent args)
        {
            var matriarchs = AllEntityQuery<VampireMatriarchComponent>();
            var aliveMat = matriarchs.Count(m => TryComp<MobStateComponent>(m.Item1, out var state) && state.CurrentState != MobState.Dead);

            var totalPlayers = _players.Sessions.Count;
            var converted = AllEntityQuery<SharedVampireComponent>().Count(e => !HasComp<VampireMatriarchComponent>(e.Item1));
            var required = (int)Math.Floor(totalPlayers * 0.4f);

            string outcome;
            if (aliveMat > 0 && converted >= required)
                outcome = "vamp-won";
            else if (aliveMat == 0)
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
