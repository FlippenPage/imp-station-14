namespace Content.Server._EinsteinEngines.BloodCult.Runes.Revive;

[RegisterComponent]
public sealed partial class ReviveRuneChargesProviderComponent : Component
{
    [DataField]
    public int Charges = 3;
}
