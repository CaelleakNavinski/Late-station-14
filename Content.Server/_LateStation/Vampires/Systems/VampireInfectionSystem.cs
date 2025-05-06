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
    /// converts the entity when the timer reaches zero.
    /// </summary>
    public sealed class VampireInfectionSystem : EntitySystem
    {
        private static readonly string[] TurningMessages =
        {
            "Feed...",                        // Loc.GetString("vamp-turn-msg-1")
            "Their blood calls...",           // Loc.GetString("vamp-turn-msg-2")
            "Embrace the darkness...",        // Loc.GetString("vamp-turn-msg-3")
            "The hunger wins soon...",        // Loc.GetString("vamp-turn-msg-4")
            "Tick... tock...",                // Loc.GetString("vamp-turn-msg-5")
            "Their pulse is your lullaby...", // Loc.GetString("vamp-turn-msg-6")
            "Their blood... it sings.",       // Loc.GetString("vamp-turn-msg-7")
            "The void in your veins grows."   // Loc.GetString("vamp-turn-msg-8")
        };

        // At these remaining-times, show one fixed “final” message in order.
        private static readonly float[] FinalThresholds = { 10f, 8f, 6f, 4f, 2f };
        private static readonly string[] FinalMessages =
        {
            "Your final heartbeat...",                              // Loc.GetString("vamp-final-msg-1")
            "You feel the last of your humanity slipping away...",  // Loc.GetString("vamp-final-msg-2")
            "You cannot remember why you fought it...",             // Loc.GetString("vamp-final-msg-3")
            "You feel peace like you've never known...",            // Loc.GetString("vamp-final-msg-4")
            "You feel..."                                           // Loc.GetString("vamp-final-msg-5")
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
                {   // Loc.GetString("vamp-final-msg-6")
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