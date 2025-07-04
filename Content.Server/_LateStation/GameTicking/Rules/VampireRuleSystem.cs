using System;
using System.Linq;
using Content.Server.Antag;
using Content.Server._LateStation.Roles;
using Content.Server.GameTicking;
using Content.Server.Roles;
using Content.Shared.Mind.Components;
using Content.Shared.Mobs.Components;
using Content.Shared._LateStation.Vampires.Components;
using Content.Server._LateStation.Vampires.Components;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Player;
using Robust.Shared.Random;
using Robust.Shared.GameObjects;
using Content.Server._LateStation.GameTicking.Rules.Components;

namespace Content.Server._LateStation.GameTicking.Rules
{
    /// <summary>
    /// Evaluates vampire win/loss conditions at round end.
    /// </summary>
    public sealed class VampireRuleSystem : EntitySystem
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
            // Count alive matriarchs
            var aliveMat = 0;
            var matriarchs = AllEntityQuery<VampireMatriarchComponent>();
            while (matriarchs.MoveNext(out var ent, out _))
            {
                if (TryComp<MobStateComponent>(ent, out var mob) && mob.CurrentState != MobState.Dead)
                    aliveMat++;
            }

            var converted = 0;
            var vamps = AllEntityQuery<SharedVampireComponent>();
            while (vamps.MoveNext(out var ent, out _))
            {
                if (!HasComp<VampireMatriarchComponent>(ent))
                    converted++;
            }

            var required = (int)Math.Floor(_players.Sessions.Count * 0.4f);

            string outcome = (aliveMat > 0 && converted >= required)
                ? "vamp-won"
                : aliveMat == 0
                    ? "vamp-lost"
                    : "vamp-stalemate";

            args.AddLine(Loc.GetString(outcome));
            args.AddLine(Loc.GetString("vamp-mat-count"));

            foreach (var (mind, data, name) in _antag.GetAntagIdentifiers(uid))
            {
                if (_role.MindHasRole<VampireRoleComponent>(mind, out var role))
                {
                    args.AddLine(Loc.GetString("vamp-mat-name-user",
                        ("name", name),
                        ("username", data.UserName),
                        ("count", role.Value.Comp2.ConvertedCount)));
                }
            }
        }
    }
}
