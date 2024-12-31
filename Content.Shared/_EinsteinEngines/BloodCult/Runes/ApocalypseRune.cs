using Content.Shared.DoAfter;
using Robust.Shared.Serialization;

namespace Content.Shared._EinsteinEngines.BloodCult.Runes;

[Serializable, NetSerializable]
public sealed partial class ApocalypseRuneDoAfter : SimpleDoAfterEvent;

[Serializable, NetSerializable]
public enum ApocalypseRuneVisuals
{
    Used,
    Layer
}
