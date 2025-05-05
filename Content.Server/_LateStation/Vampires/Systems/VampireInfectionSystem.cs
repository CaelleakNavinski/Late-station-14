using System;
using Content.Shared.Popups;
using Content.Server._LateStation.Vampires.Components;   // VampireInfectionComponent
using Robust.Shared.GameStates;                         // EntitySystem
using Robust.Shared.Timing;                             // frameTime
using Robust.Shared.GameObjects;                        // EntityQuery
using Robust.Shared.IoC;                                // [Dependency]
using Robust.Shared.Random;                             // IRobustRandom

namespace Content.Server._LateStation.Vampires.Systems
{
    /// <summary>
    /// Ticks down infection timers, shows flavor popups, and converts targets when time runs out.
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

        private static readonly float[] FinalThresholds = { 10f, 8f, 6f, 4f, 2f };
        private static readonly string[] FinalMessages =
        {
            "Your final heartbeat...",
            "You feel the last of your humanity slipping away...",
            "You cannot remember why you fought it...",
            "You feel peace like you've never known...",
            "You feel...",
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
            // Initialize our tracked fields
            comp.PopupAccumulator = 0f;
            comp.PreviousTimeLeft = comp.TimeLeft;
            comp.FinalStage = 0;
        }

        public override void Update(float frameTime)
        {
            foreach (var comp in EntityQuery<VampireInfectionComponent>(true))
            {
                // Countdown
                comp.TimeLeft = MathF.Max(0f, comp.TimeLeft - frameTime);

                // Random whispers between 45s and 10s
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

                // Final staged messages at thresholds
                while (comp.FinalStage < FinalThresholds.Length
                       && comp.PreviousTimeLeft > FinalThresholds[comp.FinalStage]
                       && comp.TimeLeft <= FinalThresholds[comp.FinalStage])
                {
                    var finalMsg = FinalMessages[comp.FinalStage];
                    _popup.PopupEntity(finalMsg, comp.Owner, PopupType.MediumCaution);
                    comp.FinalStage++;
                }

                comp.PreviousTimeLeft = comp.TimeLeft;

                // Conversion!
                if (comp.TimeLeft <= 0f)
                {
                    _popup.PopupEntity("THIRSTY.", comp.Owner, PopupType.LargeCaution);
                    EntityManager.RemoveComponent<VampireInfectionComponent>(comp.Owner);
                    EntityManager.AddComponent<VampireComponent>(comp.Owner);
                }
            }
        }
    }
}
