using Content.Server.Actions;
using Content.Shared.Actions;
using Content.Shared.Vampire.Components;
using Robust.Shared.GameStates;
using Robust.Server.GameObjects;
using Robust.Shared.Timing;

namespace Content.Server.Vampire.Systems
{
  /// <summary>
  /// Gives the Vampires their Bite action based on VampireComponent presence
  /// </summary>
  public sealed class VampireRoleSystem : EntitySystem
  {
    [Dependency] private readonly ActionSystem _actionSystem = default!;

    public override void Initialize()
    {
      base.Initialize();
      SubscribeLocalEvent<VampireComponent, ComponentInit>(OnVampireInit);
      SubscribeLocalEvent<VampireComponent, ComponentShutdown>(OnVampireShutdown);
    }

    private void OnVampireInit(EntityUid uid, VampireComponent comp, ComponentInit args)
    {
      /// Adds the Bite action so the Vampire can... well... bite people
      _actionSystem.AddAction(uid, "ActionVampireBite");
    }

    private void OnVampireShutdown(EntityUid uid, VampireComponent comp, ComponentShutdown args)
    {
      // Removes the Bite action if the VampireComponent is ever taken away
      _actionSystem.RemoveAction(uid, "ActionVampireBite");
    }
  }
}
