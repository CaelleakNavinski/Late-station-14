using System;
using System.Linq;                                            // Enumerable extensions
using Content.Server.Antag;
using Content.Server.Roles;
using Content.Server._LateStation.GameTicking.Rules.Components;
using Content.Shared.GameTicking.Components;                   // RoundEndTextAppendEvent, RoundStartEvent
using Content.Shared.Mind.Components;
using Content.Shared.Mobs.Components;                         // MobStateComponent
using Content.Shared._LateStation.Vampires.Components;        // VampireMatriarchComponent, SharedVampireComponent
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Player;

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
            // Count alive matriarchs
            var aliveMat = AllEntityQuery<VampireMatriarchComponent>()
                .Select(q => q.Item1)
                .Count(e => TryComp<MobStateComponent>(e, out var st) && st.CurrentState != MobState.Dead);

            var totalPlayers = _players.Sessions.Count;
            var converted = AllEntityQuery<SharedVampireComponent>()
                .Select(q => q.Item1)
                .Count(e => !HasComp<VampireMatriarchComponent>(e));

            var required = (int)Math.Floor(totalPlayers * 0.4f);

            var outcome = aliveMat > 0 && converted >= required
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
