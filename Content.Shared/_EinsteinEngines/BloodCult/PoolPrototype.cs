using Robust.Shared.Prototypes;

namespace Content.Shared._EinsteinEngines.BloodCult;

[Prototype("powerPool")]
public sealed partial class PowerPoolPrototype : IPrototype
{
    [ViewVariables]
    [IdDataField]
    public string ID { get; private set; } = default!;

    [ViewVariables]
    [DataField]
    public List<string> Powers = new();
}
