using Content.Shared.Alert;
using Robust.Client.GameObjects;
using Robust.Shared.Utility;

namespace Content.Client.Alerts;

/// <summary>
/// Event raised on an entity with alerts in order to allow it to update visuals for the alert sprite entity.
/// </summary>
[ByRefEvent]
public record struct UpdateAlertSpriteEvent
{
    public Entity<SpriteComponent> SpriteViewEnt;

    public EntityUid ViewerEnt;

    public AlertPrototype Alert;

    public UpdateAlertSpriteEvent(Entity<SpriteComponent> spriteViewEnt, EntityUid viewerEnt, AlertPrototype alert)
    {
        SpriteViewEnt = spriteViewEnt;
        ViewerEnt = viewerEnt;
        Alert = alert;
    }
}

/// <summary>
/// Event raised on the local player to allow alerts to provide a custom tooltip description.
/// </summary>
[ByRefEvent]
public record struct GetAlertTooltipEvent(AlertPrototype Alert, FormattedMessage? Description = null)
{
    public bool Handled => Description != null;
}
