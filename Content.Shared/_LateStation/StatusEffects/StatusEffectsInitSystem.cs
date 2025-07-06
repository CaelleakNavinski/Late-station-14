// File: Content.Shared/_LateStation/StatusEffects/StatusEffectsInitSystem.cs

using System.Collections.Generic;
using Content.Shared.StatusEffect;             // StatusEffectsComponent, StatusEffectEntry
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared._LateStation.StatusEffects
{
    /// <summary>
    /// Ensures that every StatusEffectsComponent has a non-null Effects list
    /// so that StatusEffectsSystem.OnGetState never sees a null collection.
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
            if (comp.Effects == null)
                comp.Effects = new List<StatusEffectEntry>();
        }
    }
}
