using Robust.Shared.GameStates;
using Robust.Shared.Utility;

namespace Content.Shared._EinsteinEngines.BloodCult.Components;

[NetworkedComponent, RegisterComponent]
public sealed partial class PentagramComponent : Component
{
    public ResPath RsiPath = new("/Textures/_EinsteinEngines/BloodCult/Effects/pentagram.rsi");

    public readonly string[] States =
    [
        "halo1",
        "halo2",
        "halo3",
        "halo4",
        "halo5",
        "halo6"
    ];
}
