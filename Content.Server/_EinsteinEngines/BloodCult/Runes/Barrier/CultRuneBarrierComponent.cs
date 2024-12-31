using Robust.Shared.Prototypes;

namespace Content.Server._EinsteinEngines.BloodCult.Runes.Barrier;

[RegisterComponent]
public sealed partial class CultRuneBarrierComponent : Component
{
    [DataField]
    public EntProtoId SpawnPrototype = "BloodCultBarrier";
}
