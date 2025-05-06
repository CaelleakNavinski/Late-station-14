using System;
using Content.Shared.Popups;
using Content.Server._LateStation.Vampires.Components;
using Robust.Shared.GameStates;
using Robust.Shared.Timing;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Random;

namespace Content.Server._LateStation.Vampires.Systems
{
    /// <summary>
    /// Handles the countdown from bite to full vampirism,
    /// issues intermittent and final flavor popups,
    /// and converts the entity when the timer reaches zero.
    /// </summary>
    public sealed class VampireInfectionSystem : EntitySystem
    {
        private static readonly string[] TurningMessages =
        {
            "Feed...",
            "Their blood calls...",
            "Embrace the darkness...",
            "The hunger wins soon...",
            "Tick... tock...",
            "Their pulse is your lullaby...",
            "Their blood... it sings.",
            "The void in your veins grows."
        };

        // At these remaining-times, show one fixed “final” message in order.
        private static readonly float[] FinalThresholds = { 10f, 8f, 6f, 4f, 2f };
        private static readonly string[] FinalMessages =
        {
            "Your final heartbeat...",
            "You feel the last of your humanity slipping away...",
            "You cannot remember why you fought it...",
            "You feel peace like you've never known...",
            "You feel..."
        };

        [Dependency] private readonly SharedPopupSystem _popup = default!;
        [Dependency] private readonly IRobustRandom _random = default!;

        public override void Initialize()
        {
            base.Initialize();
            SubscribeLocalEvent<VampireInfectionComponent, ComponentInit>(OnInit);
        }

        private void OnInit(EntityUid uid, VampireInfectionComponent comp, ComponentInit args)
        {
            // Reset tracking fields on new infection
            comp.PopupAccumulator = 0f;
            comp.PreviousTimeLeft = comp.TimeLeft;
            comp.FinalStage = 0;
        }

        public override void Update(float frameTime)
        {
            base.Update(frameTime);

            // Iterate all infected but not yet fully converted
            foreach (var comp in EntityQuery<VampireInfectionComponent>(true))
            {
                // Decrease the timer
                comp.TimeLeft = MathF.Max(0f, comp.TimeLeft - frameTime);

                // Between 45s and 10s, every ~3s show a random whisper
                if (comp.TimeLeft <= 45f && comp.TimeLeft >= 10f)
                {
                    comp.PopupAccumulator += frameTime;
                    if (comp.PopupAccumulator >= 3f)
                    {
                        comp.PopupAccumulator -= 3f;
                        if (_random.Prob(0.33f))
                        {
                            var msg = _random.Pick(TurningMessages);
                            _popup.PopupEntity(msg, comp.Owner, PopupType.Medium);
                        }
                    }
                }

                // At each final threshold crossing, show the corresponding final message
                while (comp.FinalStage < FinalThresholds.Length
                       && comp.PreviousTimeLeft > FinalThresholds[comp.FinalStage]
                       && comp.TimeLeft <= FinalThresholds[comp.FinalStage])
                {
                    var finalMsg = FinalMessages[comp.FinalStage];
                    _popup.PopupEntity(finalMsg, comp.Owner, PopupType.MediumCaution);
                    comp.FinalStage++;
                }

                comp.PreviousTimeLeft = comp.TimeLeft;

                // When timer hits zero, convert to full Vampire
                if (comp.TimeLeft <= 0f)
                {
                    _popup.PopupEntity("THIRSTY.", comp.Owner, PopupType.LargeCaution);
                    // Remove infection marker
                    EntityManager.RemoveComponent<VampireInfectionComponent>(comp.Owner);
                    // Add actual Vampire role/component
                    EntityManager.AddComponent<VampireComponent>(comp.Owner);
                }
            }
        }
    }
}
