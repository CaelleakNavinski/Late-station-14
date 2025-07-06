using System.Collections.Generic;
using Content.Shared.StatusEffect;    // StatusEffectsComponent, StatusEffectState
using Robust.Shared.GameObjects;

namespace Content.Shared._LateStation.StatusEffects
{
    /// <summary>
    /// Ensures that StatusEffectsComponent.ActiveEffects and AllowedEffects
    /// are never null, preventing null‐collection crashes in OnGetState.
    /// </summary>
    public sealed class StatusEffectsInitSystem : EntitySystem
    {
        public override void Initialize()
        {
            base.Initialize();
            SubscribeLocalEvent<StatusEffectsComponent, ComponentInit>(OnInit);
        }

        private void OnInit(EntityUid uid, StatusEffectsComponent comp, ComponentInit args)
        {
            // Initialize the dictionaries/lists if they came in null
            comp.ActiveEffects  ??= new Dictionary<string, StatusEffectState>();
            comp.AllowedEffects ??= new List<string>();
        }
    }
}
