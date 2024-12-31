using Robust.Shared.Audio;

namespace Content.Server._EinsteinEngines.BloodCult.Runes.Teleport;

[RegisterComponent]
public sealed partial class CultRuneTeleportComponent : Component
{
    [DataField]
    public float TeleportGatherRange = 0.65f;

    [DataField]
    public string Name = "";

    [DataField]
    public SoundPathSpecifier TeleportInSound = new("/Audio/_EinsteinEngines/BloodCult/veilin.ogg");

    [DataField]
    public SoundPathSpecifier TeleportOutSound = new("/Audio/_EinsteinEngines/BloodCult/veilout.ogg");
}
