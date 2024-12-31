namespace Content.Server._EinsteinEngines.BloodCult.RendingRunePlacement;

[RegisterComponent]
public sealed partial class RendingRunePlacementMarkerComponent : Component
{
    [DataField]
    public bool IsActive;

    [DataField]
    public float DrawingRange = 10;
}
