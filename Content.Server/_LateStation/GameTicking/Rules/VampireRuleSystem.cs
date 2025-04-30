using Content.Server.GameTicking.Rules.Components;
using Content.Server.RoundEnd;
using Content.Server.Station.Systems;
using Content.Shared.GameTicking.Components;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.StatusIcon;
using Content.Shared.Actions;
using Content.Shared._LateStation.Vampires.Components;
using Content.Shared._LateStation.Vampires.Systems;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;

namespace Content.Server.GameTicking.Rules
{
    public sealed class VampireRuleSystem : GameRuleSystem<VampireRuleComponent>
    {
        [Dependency] private readonly RoundEndSystem _roundEnd = default!;
        [Dependency] private readonly StationSystem _stationSystem = default!;
        [Dependency] private readonly IGameTiming _timing = default!;
        [Dependency] private readonly SharedActionsSystem _actionsSystem = default!;
        [Dependency] private readonly SharedVampireSystem _vampireSystem = default!;

        public override void Initialize()
        {
            // base.Initialize();
            // SubscribeLocalEvent<VampireComponent, MobStateChangedEvent>(OnVampireMobStateChanged);
        }

        protected override void Started(EntityUid uid, VampireRuleComponent component, GameRuleComponent gameRule, GameRuleStartedEvent args)
        {
            // base.Started(uid, component, gameRule, args);
            // component.NextCheck = _timing.CurTime + component.CheckInterval;
        }

        protected override void ActiveTick(EntityUid uid, VampireRuleComponent component, GameRuleComponent gameRule, float frameTime)
        {
            // base.ActiveTick(uid, component, gameRule, frameTime);

            // if (_timing.CurTime >= component.NextCheck)
            // {
                // component.NextCheck = _timing.CurTime + component.CheckInterval;

                // if (CheckVampireWinCondition())
                // {
                    // _roundEnd.DoRoundEndBehavior(RoundEndBehavior.ShuttleCall, component.ShuttleCallTime);
                    // GameTicker.EndGameRule(uid, gameRule);
                // }
            // }
        }

        private void OnVampireMobStateChanged(EntityUid uid, VampireComponent comp, MobStateChangedEvent args)
        {
            // if (args.NewMobState == MobState.Dead || args.NewMobState == MobState.Invalid)
            // {
                // Maybe vampire death logic here
            // }
        }

        private bool CheckVampireWinCondition()
        {
            // Implement win condition logic here
            // return false;
        }

        protected override void AppendRoundEndText(EntityUid uid, VampireRuleComponent component, GameRuleComponent gameRule, ref RoundEndTextAppendEvent args)
        {
            // base.AppendRoundEndText(uid, component, gameRule, ref args);
            // args.AddLine("The vampiric threat was abolished.");
        }
    }
}
