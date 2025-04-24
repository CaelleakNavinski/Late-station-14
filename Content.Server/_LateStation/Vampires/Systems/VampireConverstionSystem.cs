using Content.Shared.Vampire.Components;
using Content.Shared.StatusEffect;
using Content.Server.StatusEffects;
using Content.Shared.Popups;
using Robust.Shared.GameStates;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using Robust.Shared.GameObjects;
using Robust.Shared.Interfaces.GameObjects;
using Content.Shared.Faction;
using Content.Server.Faction;

namespace Content.Server.Vampire.Systems
{
    /// <summary>
    /// Ticks down VampireInfectionComponent timers, applies blur flavor, and flips to full vampire.
    /// </summary>
    public sealed class VampireInfectionSystem : EntitySystem
    {
        [Dependency] private readonly StatusEffectsSystem _statusSys = default!;
        [Dependency] private readonly SharedPopupSystem _popupSys = default!;
        [Dependency] private readonly FactionSystem _factionSys = default!;

        public override void Update(float frameTime)
        {
            foreach (var comp in EntityQuery<VampireInfectionComponent>())
            {
                // Decrement timer
                comp.TimeLeft = MathF.Max(0f, comp.TimeLeft - frameTime);

                // Under 45s: blur edges & flavor text
                if (comp.TimeLeft <= 45f)
                {
                    _statusSys.TryAddStatusEffect(comp.Owner, "TemporaryBlindness", 1f);
                    if (comp.TimeLeft % 10f < frameTime)
                        _popupSys.PopupEntity("You feel a primal thirst growing…", comp.Owner, PopupType.SmallCaution);
                }

                // Conversion complete
                if (comp.TimeLeft <= 0f)
                {
                    EntityManager.RemoveComponent<VampireInfectionComponent>(comp.Owner);
                    EntityManager.AddComponent<VampireComponent>(comp.Owner);
                    _factionSys.SetFaction(comp.Owner, "VampireFaction");
                }
            }
        }
    }
}
