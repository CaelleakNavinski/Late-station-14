using Content.Shared.Actions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared.Vampire.Events
{
    /// <summary>
    /// Fired when a vampire with the Bite action armed clicks on a target.
    /// </summary>
    [Prototype("VampireBiteAction")]
    public sealed partial class VampireBiteActionEvent : EntityTargetActionEvent
    {
        /// <summary>
        /// The red-flavor popup text that everyone nearby sees when the bite lands.
        /// Use {Victim} to interpolate the target’s name.
        /// </summary>
        [DataField("popupText")]
        public string PopupText { get; private set; } = "A crimson spray bursts from {Victim} as fangs sink in!";

    }
}
