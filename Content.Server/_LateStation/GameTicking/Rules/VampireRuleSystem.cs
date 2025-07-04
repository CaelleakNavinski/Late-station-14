using System;
using System.Collections.Generic;
using System.Linq;
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
using Content.Shared.Mind.Components;
using Content.Shared.Mobs.Components;
using Content.Shared._LateStation.Vampires.Components;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Random;
using Robust.Shared.Localization;

namespace Content.Server._LateStation.GameTicking.Rules
{
    /// <summary>
    /// Controls the Vampire matriarch assignment and win/loss based on converting ≥2/5 of the crew.
    /// Mirrors other GameRuleSystem implementations.
    /// </summary>
    public sealed class VampireRuleSystem : GameRuleSystem<VampireRuleComponent>
    {
        [Dependency] private readonly IRobustRandom _random = default!;
        [Dependency] private readonly ChatSystem _chat = default!;
        [Dependency] private readonly ISharedPlayerManager _players = default!;
        [Dependency] private readonly RoundEndSystem _roundEnd = default!;
        [Dependency] private readonly StationSystem _station = default!;
        [Dependency] private readonly EmergencyShuttleSystem _shuttle = default!;

        public override void Initialize()
        {
            base.Initialize();
            SubscribeLocalEvent<VampireRuleComponent, RoundStartEvent>(OnRoundStart);
            SubscribeLocalEvent<VampireRuleComponent, RoundEndTextAppendEvent>(OnAppendRoundEndText);
            SubscribeLocalEvent<VampireRoleComponent, GetBriefingEvent>(OnGetBriefing);
        }

        private void OnGetBriefing(EntityUid uid, VampireRoleComponent comp, ref GetBriefingEvent args)
        {
            var ent = args.Mind.Comp.OwnedEntity;
            var isMatriarch = HasComp<VampireMatriarchComponent>(ent);
            args.Append(Loc.GetString(isMatriarch ? "vamp-mat-briefing" : "vamp-briefing"));
        }

        private void OnRoundStart(EntityUid uid, VampireRuleComponent comp, RoundStartEvent args)
        {
            var allPlayers = _players.Sessions
                .Select(s => s.AttachedEntity)
                .Where(e => e != null)
                .Cast<EntityUid>()
                .ToList();

            if (allPlayers.Count == 0)
                return;

            var shuffled = allPlayers.OrderBy(_ => _random.Next()).ToList();

            // Assign exactly comp.MatriarchCount matriarchs
            for (var i = 0; i < comp.MatriarchCount && i < shuffled.Count; i++)
            {
                var target = shuffled[i];
                EnsureComp<VampireMatriarchComponent>(target);
                _chat.DispatchServerAnnouncement(
                    Loc.GetString("vamp-matriarch-assigned", ("player", Identity.Entity(target))),
                    playDefaultSound: true);
            }
        }

        protected override void AppendRoundEndText(EntityUid uid, VampireRuleComponent comp, GameRuleComponent gameRule, ref RoundEndTextAppendEvent args)
        {
            base.AppendRoundEndText(uid, comp, gameRule, ref args);

            // Gather matriarchs
            var matriarchs = AllEntityQuery<VampireMatriarchComponent>()
                .Select(x => x.Item1)
                .ToList();

            // If no matriarchs ever assigned or all died before start
            if (matriarchs.Count == 0)
            {
                args.AddLine(Loc.GetString("vamp-no-matriarch"));
                return;
            }

            // Check if any matriarch is alive
            var aliveMatriarchs = matriarchs.Count(m =>
                TryComp<MobStateComponent>(m, out var state) && state.CurrentState != MobState.Dead);

            // Count converted vampires (excluding matriarchs)
            var convertedCount = AllEntityQuery<SharedVampireComponent>()
                .Count(x => !HasComp<VampireMatriarchComponent>(x.Item1));

            // Total players at round start
            var totalPlayers = _players.Sessions.Count();
            var required = (int)Math.Floor(totalPlayers * 0.4f);

            string outcome;
            if (aliveMatriarchs > 0 && convertedCount >= required)
                outcome = "vamp-won";
            else if (aliveMatriarchs == 0)
                outcome = "vamp-lost";
            else
                outcome = "vamp-stalemate";

            args.AddLine(Loc.GetString(outcome));

            // List matriarch conversion counts
            args.AddLine(Loc.GetString("vamp-mat-count"));
            var antags = _players.Sessions
                .Select(s => s.AttachedEntity)
                .Where(e => e != null)
                .Cast<EntityUid>()
                .Where(e => HasComp<VampireMatriarchComponent>(e));

            foreach (var mat in antags)
            {
                if (_roles.MindHasRole<VampireRoleComponent>(mat, out var role))
                {
                    var count = role.Value.Comp2.ConvertedCount;
                    args.AddLine(Loc.GetString("vamp-mat-name-user",
                        ("name", Identity.Entity(mat)),
                        ("username", role.Value.Comp2.Owner)),
                        ("count", count));
                }
            }
        }
    }
}
