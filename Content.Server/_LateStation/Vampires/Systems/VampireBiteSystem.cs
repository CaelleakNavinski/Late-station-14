using Content.Shared.Popups;
using Content.Shared.Actions;
using Content.Shared.Vampire.Components;
using Content.Shared.Vampire.Events;
using Content.Shared.Humanoid;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;
using Robust.Shared.Audio;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Timing;
using Robust.Shared.GameObjects;

namespace Content.Server.Vampire.Systems
{
    /// <summary>
    /// Handles the VampireBiteActionEvent: applies infection and shows red flavor text.
    /// </summary>
    public sealed class VampireBiteSystem : EntitySystem
    {
        [Dependency] private readonly SharedPopupSystem _popupSystem = default!;

        public override void Initialize()
        {
            base.Initialize();
            SubscribeLocalEvent<VampireBiteActionEvent>(OnVampireBite);
        }

        private void OnVampireBite(VampireBiteActionEvent ev)
        {
            var user = ev.Performer;
            var target = ev.Target;

            // Only humanoids can be bitten
            if (!EntityManager.HasComponent<HumanoidComponent>(target))
                return;

            // Already infected or a vampire?
            if (EntityManager.HasComponent<VampireInfectionComponent>(target) ||
                EntityManager.HasComponent<VampireComponent>(target))
                return;

            // Apply infection component
            EntityManager.AddComponent<VampireInfectionComponent>(target);

            // Broadcast red flavor popup to nearby players
            var text = Loc.GetString(ev.PopupText, ("Victim", Identity.Entity(target, EntityManager)));
            _popupSystem.PopupEntity(text, target, PopupType.LargeCaution);
        }
    }
}
