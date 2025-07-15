using Content.Server.StationEvents.Events;

namespace Content.Server.StationEvents.Components;

[RegisterComponent, Access(typeof(RadiationStormRule))]
public sealed partial class RadiationStormRuleComponent : Component
{
    public float TimeUntilPulse;
    public float MinPulseDelay = 0.2f;
    public float MaxPulseDelay = 0.8f;
    public EntityUid Target = EntityUid.Invalid;
}
