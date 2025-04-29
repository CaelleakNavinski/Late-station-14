using Content.Shared.Popups;
using Content.Shared._LateStation.Vampires.Components;
using Content.Shared._LateStation.Vampires.Events;
using Content.Shared.Humanoid;            // for HumanoidComponent
using Content.Shared.Actions;             // for ActionsComponent (if needed)
using Robust.Shared.GameStates;           // EntitySystem
using Robust.Server.GameObjects;          // EntityManager, SharedPopupSystem
using Robust.Shared.IoC;                  // [Dependency]

namespace Content.Server._LateStation.Vampires.Systems
{
    public sealed class VampireBiteSystem : EntitySystem
    {
        [Dependency] private readonly SharedPopupSystem _popup = default!;

        public override void Initialize()
        {
            SubscribeLocalEvent<VampireBiteActionEvent>(OnBite);
        }

        private void OnBite(VampireBiteActionEvent ev)
        {
            var user = ev.Performer;
            var target = ev.Target;

            // Only humanoids
            if (!EntityManager.HadComponent<HumanoidAppearanceComponent>(uid))
                return;

            // Already infected or already a vampire?
            if (EntityManager.HasComponent<VampireInfectionComponent>(target) ||
                EntityManager.HasComponent<VampireComponent>(target))
                return;

            // Start infection
            EntityManager.AddComponent<VampireInfectionComponent>(target);

            // Flavor popup
            var name = EntityManager.GetComponent<MetaDataComponent>(target).EntityName;
            _popup.PopupEntity(
                ev.PopupText.Replace("{Victim}", name),
                target,
                PopupType.LargeCaution);

            // Optionally, you could also remove the toggle from the user here
            // But bite removal is handled by VampireComponent shutdown in VampireRoleSystem
        }
    }
}
