using System;
using Content.Server.Actions;
using Content.Server.Mind;
using Content.Server.Roles;
using Content.Shared.DoAfter;
using Content.Shared.Humanoid;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared._LateStation.Roles.Components;
using Content.Shared._LateStation.Vampire;
using Content.Shared._LateStation.Vampire.Components;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Server._LateStation.Vampire;

/// <summary>
/// First implementation slice for vampires:
/// - Converting Bite action
/// - Feed action
/// - timed turning state
/// - blood resource / decay groundwork
/// - completion into Vampire / Exarch-capable vampire
/// </summary>
public sealed class VampireSystem : EntitySystem
{
    private const string BiteActionId = "ActionVampireBite";
    private const string FeedActionId = "ActionVampireFeed";
    private const string MindRoleVampire = "MindRoleVampire";

    [Dependency] private readonly ActionsSystem _actions = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly MindSystem _mind = default!;
    [Dependency] private readonly MobStateSystem _mobState = default!;
    [Dependency] private readonly RoleSystem _role = default!;
    [Dependency] private readonly SharedDoAfterSystem _doAfter = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<VampireComponent, ComponentStartup>(OnVampireStartup);
        SubscribeLocalEvent<VampireComponent, ComponentShutdown>(OnVampireShutdown);
        SubscribeLocalEvent<VampireComponent, VampireBiteActionEvent>(OnBiteAction);
        SubscribeLocalEvent<VampireComponent, VampireBiteDoAfterEvent>(OnBiteDoAfter);
        SubscribeLocalEvent<VampireComponent, VampireFeedActionEvent>(OnFeedAction);
        SubscribeLocalEvent<VampireComponent, VampireFeedDoAfterEvent>(OnFeedDoAfter);
    }

    private void OnVampireStartup(Entity<VampireComponent> ent, ref ComponentStartup args)
    {
        SyncActions(ent);
    }

    private void OnVampireShutdown(Entity<VampireComponent> ent, ref ComponentShutdown args)
    {
        _actions.RemoveAction(ent.Owner, ent.Comp.BiteAction);
        _actions.RemoveAction(ent.Owner, ent.Comp.FeedAction);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var vampireQuery = EntityQueryEnumerator<VampireComponent>();
        while (vampireQuery.MoveNext(out var uid, out var vampire))
        {
            SyncActions((uid, vampire));
            ProcessBloodDecay((uid, vampire));
        }

        var turningQuery = EntityQueryEnumerator<VampireTurningComponent>();
        while (turningQuery.MoveNext(out var uid, out var turning))
        {
            if (turning.NextTick > _timing.CurTime)
                continue;

            turning.NextTick = _timing.CurTime + TimeSpan.FromSeconds(1);
            turning.Remaining -= TimeSpan.FromSeconds(1);

            HandleTurningMessages(uid, turning);

            if (turning.Remaining > TimeSpan.Zero)
                continue;

            CompleteTurning(uid, turning);
        }
    }

    private void SyncActions(Entity<VampireComponent> ent)
    {
        _actions.AddAction(ent.Owner, ref ent.Comp.FeedAction, FeedActionId);

        if (ent.Comp.CanConvert)
        {
            _actions.AddAction(ent.Owner, ref ent.Comp.BiteAction, BiteActionId);
            return;
        }

        if (ent.Comp.BiteAction != null)
        {
            _actions.RemoveAction(ent.Owner, ent.Comp.BiteAction);
            ent.Comp.BiteAction = null;
        }
    }

    private void ProcessBloodDecay(Entity<VampireComponent> ent)
    {
        if (ent.Comp.Blood <= 0f)
            return;

        if (ent.Comp.NextBloodDecayTick == TimeSpan.Zero)
            ent.Comp.NextBloodDecayTick =
                _timing.CurTime + ent.Comp.BloodDecayDelay + ent.Comp.BloodDecayInterval;

        if (ent.Comp.NextBloodDecayTick > _timing.CurTime)
            return;

        ent.Comp.Blood = MathF.Max(0f, ent.Comp.Blood - 1f);
        ent.Comp.NextBloodDecayTick = _timing.CurTime + ent.Comp.BloodDecayInterval;
        Dirty(ent.Owner, ent.Comp);
    }

    private void OnBiteAction(Entity<VampireComponent> ent, ref VampireBiteActionEvent args)
    {
        if (args.Handled || !ent.Comp.CanConvert)
            return;

        if (!CanStartTurning(ent.Owner, args.Target))
            return;

        var doAfter = new DoAfterArgs(
            EntityManager,
            args.Performer,
            TimeSpan.FromSeconds(1),
            new VampireBiteDoAfterEvent(),
            target: args.Target,
            used: ent.Owner,
            eventTarget: ent.Owner)
        {
            BreakOnDamage = true,
            BreakOnMove = true,
            MovementThreshold = 0.5f,
            CancelDuplicate = false
        };

        _doAfter.TryStartDoAfter(doAfter);
        args.Handled = true;
    }

    private void OnBiteDoAfter(Entity<VampireComponent> ent, ref VampireBiteDoAfterEvent args)
    {
        if (args.Cancelled || args.Handled || args.Target == null)
            return;

        var target = args.Target.Value;

        if (!CanStartTurning(ent.Owner, target))
            return;

        var turning = EnsureComp<VampireTurningComponent>(target);
        turning.Source = ent.Owner;
        turning.Remaining = TimeSpan.FromMinutes(2);
        turning.FinalWarningStage = 0;
        turning.NextTick = _timing.CurTime + TimeSpan.FromSeconds(1);
        turning.HasBiteMarks = true;

        _popup.PopupEntity(Loc.GetString("vamp-bite-popup", ("victim", target)), target, target);

        args.Handled = true;
    }

    private void OnFeedAction(Entity<VampireComponent> ent, ref VampireFeedActionEvent args)
    {
        if (args.Handled)
            return;

        if (!CanFeedTarget(ent.Owner, args.Target))
            return;

        StartFeedDoAfter(ent.Owner, args.Target);
        args.Handled = true;
    }

    private void StartFeedDoAfter(EntityUid vampire, EntityUid target)
    {
        var doAfter = new DoAfterArgs(
            EntityManager,
            vampire,
            TimeSpan.FromSeconds(2.5f),
            new VampireFeedDoAfterEvent(),
            target: target,
            used: vampire,
            eventTarget: vampire)
        {
            BreakOnDamage = true,
            BreakOnMove = true,
            MovementThreshold = 0.5f,
            CancelDuplicate = false
        };

        _doAfter.TryStartDoAfter(doAfter);
    }

    private void OnFeedDoAfter(Entity<VampireComponent> ent, ref VampireFeedDoAfterEvent args)
    {
        if (args.Cancelled || args.Handled || args.Target == null)
            return;

        var target = args.Target.Value;

        if (!CanFeedTarget(ent.Owner, target))
            return;

        ent.Comp.Blood = MathF.Min(ent.Comp.MaxBlood, ent.Comp.Blood + 5f);
        ent.Comp.LastFeedTime = _timing.CurTime;
        ent.Comp.NextBloodDecayTick =
            _timing.CurTime + ent.Comp.BloodDecayDelay + ent.Comp.BloodDecayInterval;
        Dirty(ent.Owner, ent.Comp);

        StartFeedDoAfter(ent.Owner, target);

        args.Handled = true;
    }

    private bool CanFeedTarget(EntityUid vampire, EntityUid target)
    {
        if (vampire == target)
            return false;

        if (HasComp<VampireComponent>(target))
            return false;

        if (!TryComp<HumanoidAppearanceComponent>(target, out _))
            return false;

        if (!TryComp<MobStateComponent>(target, out var mobState))
            return false;

        return _mobState.IsAlive(target, mobState) || _mobState.IsCritical(target, mobState);
    }

    private bool CanStartTurning(EntityUid vampire, EntityUid target)
    {
        if (vampire == target)
            return false;

        if (!TryComp<HumanoidAppearanceComponent>(target, out _))
            return false;

        if (HasComp<VampireComponent>(target))
        {
            _popup.PopupEntity(
                Loc.GetString("vamp-target-immune-other-vamp-popup", ("victim", target)),
                vampire,
                vampire);
            return false;
        }

        if (HasComp<VampireTurningComponent>(target))
        {
            _popup.PopupEntity(
                Loc.GetString("vamp-target-immune-misc-popup", ("victim", target)),
                vampire,
                vampire);
            return false;
        }

        if (!TryComp<MobStateComponent>(target, out var mobState))
            return false;

        return _mobState.IsAlive(target, mobState) || _mobState.IsCritical(target, mobState);
    }

    private void HandleTurningMessages(EntityUid uid, VampireTurningComponent comp)
    {
        var remaining = (int)Math.Ceiling(comp.Remaining.TotalSeconds);

        if (remaining <= 0)
        {
            _popup.PopupEntity(Loc.GetString("vamp-final-msg-6"), uid, uid);
            return;
        }

        if (remaining <= 10)
        {
            var stage = remaining switch
            {
                10 => 1,
                8 => 2,
                6 => 3,
                4 => 4,
                2 => 5,
                _ => 0
            };

            if (stage > comp.FinalWarningStage)
            {
                comp.FinalWarningStage = stage;
                _popup.PopupEntity(Loc.GetString($"vamp-final-msg-{stage}"), uid, uid);
            }

            return;
        }

        if (remaining <= 45 && remaining % 3 == 0 && _random.Prob(1f / 3f))
        {
            var msg = _random.Next(1, 8);
            _popup.PopupEntity(Loc.GetString($"vamp-turn-msg-{msg}"), uid, uid);
        }
    }

    private void CompleteTurning(EntityUid uid, VampireTurningComponent comp)
    {
        RemCompDeferred<VampireTurningComponent>(uid);

        var vampire = EnsureComp<VampireComponent>(uid);
        vampire.Matriarch = ResolveMatriarch(comp.Source);

        if (!vampire.CanConvert && _random.Prob(0.10f))
            vampire.CanConvert = true;

        SyncActions((uid, vampire));

        if (!_mind.TryGetMind(uid, out var mindId, out _))
            return;

        if (_role.MindHasRole<VampireRoleComponent>(mindId, out _))
            return;

        if (_role.MindHasRole<VampireMatriarchRoleComponent>(mindId, out _))
            return;

        _role.MindAddRole(mindId, MindRoleVampire);

        _popup.PopupEntity(Loc.GetString("vamp-role-greeting"), uid, uid);
    }

    private EntityUid? ResolveMatriarch(EntityUid? source)
    {
        if (source == null)
            return null;

        if (TryComp<VampireComponent>(source.Value, out var sourceVamp))
        {
            if (sourceVamp.Matriarch != null)
                return sourceVamp.Matriarch;

            if (_mind.TryGetMind(source.Value, out var mindId, out _) &&
                _role.MindHasRole<VampireMatriarchRoleComponent>(mindId, out _))
            {
                return source.Value;
            }
        }
        else if (_mind.TryGetMind(source.Value, out var mindId, out _) &&
                 _role.MindHasRole<VampireMatriarchRoleComponent>(mindId, out _))
        {
            return source.Value;
        }

        return null;
    }
}