using Robust.Shared.GameStates;

namespace Content.Shared._Impstation.Traits.Assorted;

/// <summary>
/// Sets player to become slower without a walking aid.
/// </summary>
[RegisterComponent, NetworkedComponent, Access(typeof(LegWeaknessSystem))]
public sealed partial class LegWeaknessComponent : Component
{
}
