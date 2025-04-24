using Robust.Shared.GameStates;

namespace Content.Shared.Vampire.Components
{
  /// <summary>
  /// Marker component for fully converted Vampires. Basic foundation for Vampire abilities.
  [RegisterComponent, NetworkedComponent]
  public sealed partial class VampireComponent : Component
  {
    // No more data is needed here. Vampire status is there.
  }
}
