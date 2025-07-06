using Content.Shared.StatusEffect;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server._LateStation.StatusEffects
{
    /// <summary>
    /// Prevents crashes caused by uninitialized AllowedEffects/ActiveEffects.
    /// Ensures that any StatusEffectsComponent has non-null lists.
    /// </summary>
    public sealed class StatusEffectStateSuppressorSystem : EntitySystem
    {
        public override void Initialize()
        {
            base.Initialize();
            SubscribeLocalEvent<StatusEffectsComponent, ComponentInit>(OnInit);
        }

        private void OnInit(EntityUid uid, StatusEffectsComponent comp, ComponentInit args)
        {
            comp.AllowedEffects ??= new();
            comp.ActiveEffects ??= new();
        }
    }
}
