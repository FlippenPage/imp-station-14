namespace Content.Server._EinsteinEngines.BloodCult.Runes.Empower;

[RegisterComponent]
public sealed partial class CultRuneEmpowerComponent : Component
{
    [DataField]
    public string ComponentToGive = "BloodCultEmpowered";
}
