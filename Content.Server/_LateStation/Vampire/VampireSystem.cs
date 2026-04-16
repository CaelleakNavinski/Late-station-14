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
/// - 1 second bite DoAfter
/// - timed turning state
/// - completion into Vampire / Exarch-capable vampire
/// </summary>
public sealed class VampireSystem : EntitySystem
{
    private const string BiteActionId = "ActionVampireBite";
    private const string MindRoleVampire = "MindRoleVamp            CompleteTurning(uid, turning);
        }
    }

    private void EnsureBiteAction(Entity<VampireComponent> ent)
    {
        if (!ent.Comp.CanConvert)
            return;

        _actions.AddAction(ent.Owner, ref ent.Comp.BiteAction, BiteActionId);
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

        _popup.PopupEntity(Loc.GetString("vamp-bite-popup", ("victim", target)), target, target);

        args.Handled = true;
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
    }
