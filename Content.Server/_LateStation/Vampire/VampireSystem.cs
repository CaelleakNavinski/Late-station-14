using System;
using Content.Server.Actions;
using Content.Server.Antag;
using Content.Server.Mind;
using Content.Server.Polymorph.Systems;
using Content.Server.Roles;
using Content.Shared.Alert;
using Content.Shared.Body.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.DoAfter;
using Content.Shared.FixedPoint;
using Content.Shared.Humanoid;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Movement.Systems;
using Content.Shared.Popups;
using Content.Shared.Radio;
using Content.Shared.Radio.Components;
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
/// - Bloodsprint action
/// - Mist Form action
/// - timed turning state
/// - blood resource / decay groundwork
/// - completion into Vampire / Exarch-capable vampire
/// </summary>
public sealed class VampireSystem : EntitySystem
{
    private const string BiteActionId = "ActionVampireBite";
    private const string BloodAlertId = "VampireBloodMeter";
    private const string BloodSprintActionId = "ActionVampireBloodSprint";
    private const string FeedActionId = "ActionVampireFeed";
    private const string MindRoleVampire = "MindRoleVampire";
    private const string MistFormActionId = "ActionVampireMistForm";

    [Dependency] private readonly ActionsSystem _actions = default!;
    [Dependency] private readonly AlertsSystem _alerts = default!;
    [Dependency] private readonly AntagSelectionSystem _antag = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly MindSystem _mind = default!;
    [Dependency] private readonly MobStateSystem _mobState = default!;
    [Dependency] private readonly MovementSpeedModifierSystem _movementSpeed = default!;
    [Dependency] private readonly PolymorphSystem _polymorph = default!;
    [Dependency] private readonly RoleSystem _role = default!;
    [Dependency] private readonly SharedDoAfterSystem _doAfter = default!;
    [Dependency] private readonly SharedSolutionContainerSystem _solutionContainer = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<VampireComponent, ComponentStartup>(OnVampireStartup);
        SubscribeLocalEvent<VampireComponent, ComponentShutdown>(OnVampireShutdown);
        SubscribeLocalEvent<VampireComponent, GetDefaultRadioChannelEvent>(OnGetDefaultRadioChannel);
        SubscribeLocalEvent<VampireComponent, RefreshMovementSpeedModifiersEvent>(OnRefreshMovementSpeedModifiers);
        SubscribeLocalEvent<VampireComponent, VampireBiteActionEvent>(OnBiteAction);
        SubscribeLocalEvent<VampireComponent, VampireBiteDoAfterEvent>(OnBiteDoAfter);
        SubscribeLocalEvent<VampireComponent, VampireBloodSprintActionEvent>(OnBloodSprintAction);
        SubscribeLocalEvent<VampireComponent, VampireMistFormActionEvent>(OnMistFormAction);
        SubscribeLocalEvent<VampireComponent, VampireFeedActionEvent>(OnFeedAction);
        SubscribeLocalEvent<VampireComponent, VampireFeedDoAfterEvent>(OnFeedDoAfter);
    }

    private void OnVampireStartup(Entity<VampireComponent> ent, ref ComponentStartup args)
    {
        SyncActions(ent);
        SyncIntrinsicRadio(ent);
    }

    private void OnVampireShutdown(Entity<VampireComponent> ent, ref ComponentShutdown args)
    {
        _actions.RemoveAction(ent.Owner, ent.Comp.BiteAction);
        _actions.RemoveAction(ent.Owner, ent.Comp.FeedAction);
        _actions.RemoveAction(ent.Owner, ent.Comp.BloodSprintAction);
        _actions.RemoveAction(ent.Owner, ent.Comp.MistFormAction);

        RemoveIntrinsicRadio(ent);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var curTime = _timing.CurTime;

        var vampireQuery = EntityQueryEnumerator<VampireComponent>();
        while (vampireQuery.MoveNext(out var uid, out var vampire))
        {
            SyncActions((uid, vampire));
            ProcessBloodDecay((uid, vampire));
            UpdateBloodAlert(uid, vampire);

            if (vampire.BloodSprintEndTime != TimeSpan.Zero && vampire.BloodSprintEndTime <= curTime)
            {
                vampire.BloodSprintEndTime = TimeSpan.Zero;
                Dirty(uid, vampire);
                _movementSpeed.RefreshMovementSpeedModifiers(uid);
            }
        }

        var turningQuery = EntityQueryEnumerator<VampireTurningComponent>();
        while (turningQuery.MoveNext(out var uid, out var turning))
        {
            if (turning.NextTick > curTime)
                continue;

            turning.NextTick = curTime + TimeSpan.FromSeconds(1);
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

        if (HasSireAbilities(ent.Owner, ent.Comp))
        {
            _actions.AddAction(ent.Owner, ref ent.Comp.BiteAction, BiteActionId);
            _actions.AddAction(ent.Owner, ref ent.Comp.BloodSprintAction, BloodSprintActionId);
            _actions.AddAction(ent.Owner, ref ent.Comp.MistFormAction, MistFormActionId);
            return;
        }

        if (ent.Comp.BiteAction != null)
        {
            _actions.RemoveAction(ent.Owner, ent.Comp.BiteAction);
            ent.Comp.BiteAction = null;
        }

        if (ent.Comp.BloodSprintAction != null)
        {
            _actions.RemoveAction(ent.Owner, ent.Comp.BloodSprintAction);
            ent.Comp.BloodSprintAction = null;
        }

        if (ent.Comp.MistFormAction != null)
        {
            _actions.RemoveAction(ent.Owner, ent.Comp.MistFormAction);
            ent.Comp.MistFormAction = null;
        }
    }

    private bool HasSireAbilities(EntityUid uid, VampireComponent comp)
    {
        if (comp.IsExarch)
            return true;

        if (_mind.TryGetMind(uid, out var mindId, out _) &&
            _role.MindHasRole<VampireMatriarchRoleComponent>(mindId, out _))
        {
            return true;
        }

        return false;
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

    private void UpdateBloodAlert(EntityUid uid, VampireComponent comp)
    {
        var ratio = comp.Blood / comp.MaxBlood;

        short severity = ratio switch
        {
            <= 0.08f => 0,
            <= 0.22f => 1,
            <= 0.44f => 2,
            <= 0.66f => 3,
            <= 0.88f => 4,
            _ => 5
        };

        _alerts.ShowAlert(uid, BloodAlertId, severity);
    }

    private void OnRefreshMovementSpeedModifiers(Entity<VampireComponent> ent, ref RefreshMovementSpeedModifiersEvent args)
    {
        if (ent.Comp.BloodSprintEndTime <= _timing.CurTime)
            return;

        args.ModifySpeed(ent.Comp.BloodSprintWalkSpeedModifier, ent.Comp.BloodSprintSprintSpeedModifier);
    }

    private void OnBloodSprintAction(Entity<VampireComponent> ent, ref VampireBloodSprintActionEvent args)
    {
        if (args.Handled)
            return;

        if (!HasSireAbilities(ent.Owner, ent.Comp))
            return;

        if (ent.Comp.Blood < ent.Comp.BloodSprintCost)
            return;

        ent.Comp.Blood = MathF.Max(0f, ent.Comp.Blood - ent.Comp.BloodSprintCost);
        ent.Comp.BloodSprintEndTime = _timing.CurTime + ent.Comp.BloodSprintDuration;
        ent.Comp.LastFeedTime = _timing.CurTime;
        ent.Comp.NextBloodDecayTick =
            _timing.CurTime + ent.Comp.BloodDecayDelay + ent.Comp.BloodDecayInterval;

        Dirty(ent.Owner, ent.Comp);
        _movementSpeed.RefreshMovementSpeedModifiers(ent.Owner);

        args.Handled = true;
    }

    private void OnMistFormAction(Entity<VampireComponent> ent, ref VampireMistFormActionEvent args)
    {
        if (args.Handled)
            return;

        if (!HasSireAbilities(ent.Owner, ent.Comp))
            return;

        if (ent.Comp.Blood < ent.Comp.MistFormCost)
            return;

        if (_polymorph.PolymorphEntity(ent.Owner, "Jaunt") == null)
            return;

        ent.Comp.Blood = MathF.Max(0f, ent.Comp.Blood - ent.Comp.MistFormCost);
        ent.Comp.LastFeedTime = _timing.CurTime;
        ent.Comp.NextBloodDecayTick =
            _timing.CurTime + ent.Comp.BloodDecayDelay + ent.Comp.BloodDecayInterval;

        Dirty(ent.Owner, ent.Comp);
        args.Handled = true;
    }

    private void OnBiteAction(Entity<VampireComponent> ent, ref VampireBiteActionEvent args)
    {
        if (args.Handled || !HasSireAbilities(ent.Owner, ent.Comp))
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

        if (ent.Comp.Blood >= ent.Comp.MaxBlood)
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

        if (ent.Comp.Blood >= ent.Comp.MaxBlood)
            return;

        if (!TryDrainFeedBlood(target, ent.Comp, out var gainedBlood))
            return;

        ent.Comp.Blood = MathF.Min(ent.Comp.MaxBlood, ent.Comp.Blood + gainedBlood);
        ent.Comp.LastFeedTime = _timing.CurTime;
        ent.Comp.NextBloodDecayTick =
            _timing.CurTime + ent.Comp.BloodDecayDelay + ent.Comp.BloodDecayInterval;
        Dirty(ent.Owner, ent.Comp);

        if (ent.Comp.Blood < ent.Comp.MaxBlood && CanFeedTarget(ent.Owner, target))
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

        if (!_mobState.IsAlive(target, mobState) && !_mobState.IsCritical(target, mobState))
            return false;

        if (!TryComp<BloodstreamComponent>(target, out var bloodstream))
            return false;

        if (!TryComp<SolutionContainerManagerComponent>(target, out var solutionManager))
            return false;

        if (!_solutionContainer.ResolveSolution((target, solutionManager), bloodstream.BloodSolutionName, ref bloodstream.BloodSolution, out var bloodSolution))
            return false;

        return bloodSolution.Volume > FixedPoint2.Zero;
    }

    private bool TryDrainFeedBlood(EntityUid target, VampireComponent vampire, out float gainedBlood)
    {
        gainedBlood = 0f;

        if (!TryComp<BloodstreamComponent>(target, out var bloodstream))
            return false;

        if (!TryComp<SolutionContainerManagerComponent>(target, out var solutionManager))
            return false;

        if (!_solutionContainer.ResolveSolution((target, solutionManager), bloodstream.BloodSolutionName, ref bloodstream.BloodSolution, out var bloodSolution))
            return false;

        if (bloodSolution.Volume <= FixedPoint2.Zero)
            return false;

        var missingBlood = vampire.MaxBlood - vampire.Blood;
        if (missingBlood <= 0f)
            return false;

        if (vampire.FeedEfficiency <= 0f)
            return false;

        var desiredDrain = bloodSolution.Volume * vampire.FeedTargetBloodDrainFraction;
        var maxUsefulDrain = FixedPoint2.New(missingBlood / vampire.FeedEfficiency);
        var drainedBlood = FixedPoint2.Min(desiredDrain, bloodSolution.Volume, maxUsefulDrain);

        if (drainedBlood <= FixedPoint2.Zero)
            return false;

        _solutionContainer.SplitSolution(bloodstream.BloodSolution.Value, drainedBlood);

        gainedBlood = drainedBlood.Float() * vampire.FeedEfficiency;
        return gainedBlood > 0f;
    }

    private bool CanStartTurning(EntityUid vampire, EntityUid target)
    {
        if (vampire == target)
            return false;

        if (!TryComp<HumanoidAppearanceComponent>(target, out _))
            return false;

        if (HasComp<VampireComponent>(target) || HasComp<VampireMatriarchComponent>(target))
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

        if (!vampire.IsExarch && _random.Prob(0.10f))
            vampire.IsExarch = true;

        SyncActions((uid, vampire));

        if (!_mind.TryGetMind(uid, out var mindId, out _))
            return;

        if (_role.MindHasRole<VampireRoleComponent>(mindId, out _))
            return;

        if (_role.MindHasRole<VampireMatriarchRoleComponent>(mindId, out _))
            return;

        _role.MindAddRole(mindId, MindRoleVampire);

        _antag.SendBriefing(uid, Loc.GetString("vamp-role-greeting"), Color.Red, null);
    }

    private void OnGetDefaultRadioChannel(Entity<VampireComponent> ent, ref GetDefaultRadioChannelEvent args)
    {
        args.Channel = ent.Comp.RadioChannel;
    }

    private void SyncIntrinsicRadio(Entity<VampireComponent> ent)
    {
        var activeRadio = EnsureComp<ActiveRadioComponent>(ent.Owner);
        if (activeRadio.Channels.Add(ent.Comp.RadioChannel))
            ent.Comp.ActiveAddedChannels.Add(ent.Comp.RadioChannel);

        EnsureComp<IntrinsicRadioReceiverComponent>(ent.Owner);

        var transmitter = EnsureComp<IntrinsicRadioTransmitterComponent>(ent.Owner);
        if (transmitter.Channels.Add(ent.Comp.RadioChannel))
            ent.Comp.TransmitterAddedChannels.Add(ent.Comp.RadioChannel);
    }

    private void RemoveIntrinsicRadio(Entity<VampireComponent> ent)
    {
        if (TryComp<ActiveRadioComponent>(ent.Owner, out var activeRadio))
        {
            foreach (var channel in ent.Comp.ActiveAddedChannels)
            {
                activeRadio.Channels.Remove(channel);
            }

            ent.Comp.ActiveAddedChannels.Clear();

            if (activeRadio.Channels.Count == 0)
                RemCompDeferred<ActiveRadioComponent>(ent.Owner);
        }

        if (TryComp<IntrinsicRadioTransmitterComponent>(ent.Owner, out var transmitter))
        {
            foreach (var channel in ent.Comp.TransmitterAddedChannels)
            {
                transmitter.Channels.Remove(channel);
            }

            ent.Comp.TransmitterAddedChannels.Clear();

            if (transmitter.Channels.Count == 0)
                RemCompDeferred<IntrinsicRadioTransmitterComponent>(ent.Owner);
        }
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