using Robust.Client.GameObjects;
using Robust.Shared.GameStates;
using Content.Shared._LateStation.Vampires.Components;

namespace Content.Client._LateStation.Vampires.Systems
{
    public sealed class VampireClientSystem : EntitySystem
    {
        public override void Initialize()
        {
            SubscribeLocalEvent<VampireInfectionComponent, ComponentGetState>(OnGetState);
        }

        private void OnGetState(EntityUid uid, VampireInfectionComponent comp, ref ComponentGetState args)
        {
            // Client-side vision HUD effects go here.
        }
    }
}
