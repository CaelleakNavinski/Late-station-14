using Content.Shared.StatusEffect;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.GameStates; // Needed for ComponentGetStateAttemptEvent

namespace Content.Server._LateStation.StatusEffects
{
    /// <summary>
    /// Prevents crashes in StatusEffectsSystem.OnGetState by cancelling the state request
    /// if the component was constructed without initializing its list fields.
    /// </summary>
    public sealed class StatusEffectStateSuppressorSystem : EntitySystem
    {
        public override void Initialize()
        {
            base.Initialize();
            SubscribeLocalEvent<StatusEffectsComponent, ComponentGetStateAttemptEvent>(OnGetStateAttempt);
        }

        private void OnGetStateAttempt(EntityUid uid, StatusEffectsComponent comp, ref ComponentGetStateAttemptEvent args)
        {
            if (comp.AllowedEffects == null || comp.ActiveEffects == null)
            {
                args.Cancelled = true;
            }
        }
    }
}
