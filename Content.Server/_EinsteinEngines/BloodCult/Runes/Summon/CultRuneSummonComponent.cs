using Robust.Shared.Audio;

namespace Content.Server._EinsteinEngines.BloodCult.Runes.Summon;

[RegisterComponent]
public sealed partial class CultRuneSummonComponent : Component
{
    [DataField]
    public SoundPathSpecifier TeleportSound = new("/Audio/_EinsteinEngines/BloodCult/veilin.ogg");
}
