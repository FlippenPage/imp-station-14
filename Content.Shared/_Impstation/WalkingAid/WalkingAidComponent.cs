using Robust.Shared.GameStates;

namespace Content.Shared._Impstation.WalkingAid;

/// <summary>
/// If player has LegsParalyzed comp, this component handles items that allow them to walk again.
/// </summary>
[RegisterComponent, NetworkedComponent, Access(typeof(WalkingAidSystem))]
public sealed partial class WalkingAidComponent : Component
{
}