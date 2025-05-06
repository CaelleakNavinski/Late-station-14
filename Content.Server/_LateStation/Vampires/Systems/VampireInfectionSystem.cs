using System;
using Content.Shared.Popups;
using Content.Server._LateStation.Vampires.Components;
using Robust.Shared.GameStates;
using Robust.Shared.Timing;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Random;
using Robust.Shared.Localization;

namespace Content.Server._LateStation.Vampires.Systems
{
    public sealed class VampireInfectionSystem : EntitySystem
    {
        // FTL keys for random whispers:
        private static readonly string[] WhisperKeys =
        {
            "vamp-turn-msg-1",
            "vamp-turn-msg-2",
            "vamp-turn-msg-3",
            "vamp-turn-msg-4",
            "vamp-turn-msg-5",
            "vamp-turn-msg-6",
            "vamp-turn-msg-7"
        };

        // FTL keys for final threshold messages:
        private static readonly string[] FinalKeys =
        {
            "vamp-final-msg-1",
            "vamp-final-msg-2",
            "vamp-final-msg-3",
            "vamp-final-msg-4",
            "vamp-final-msg-5"
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
            comp.PopupAccumulator  = 0f;
            comp.PreviousTimeLeft  = comp.TimeLeft;
            comp.FinalStage        = 0;
        }

        public override void Update(float frameTime)
        {
            base.Update(frameTime);

            foreach (var comp in EntityQuery<VampireInfectionComponent>(true))
            {
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
                            var key = _random.Pick(WhisperKeys);
                            var msg = Loc.GetString(key);
                            _popup.PopupEntity(msg, comp.Owner, PopupType.Medium);
                        }
                    }
                }

                // Final fixed messages at thresholds
                while (comp.FinalStage < FinalKeys.Length
                       && comp.PreviousTimeLeft > comp.FinalThresholds[comp.FinalStage]
                       && comp.TimeLeft <= comp.FinalThresholds[comp.FinalStage])
                {
                    var key = FinalKeys[comp.FinalStage];
                    var finalMsg = Loc.GetString(key);
                    _popup.PopupEntity(finalMsg, comp.Owner, PopupType.MediumCaution);
                    comp.FinalStage++;
                }

                comp.PreviousTimeLeft = comp.TimeLeft;

                // Conversion complete
                if (comp.TimeLeft <= 0f)
                {
                    var thirstyText = Loc.GetString("vamp-final-msg-6");
                    _popup.PopupEntity(thirstyText, comp.Owner, PopupType.LargeCaution);

                    EntityManager.RemoveComponent<VampireInfectionComponent>(comp.Owner);
                    EntityManager.AddComponent<VampireComponent>(comp.Owner);
                }
            }
        }
    }
}
