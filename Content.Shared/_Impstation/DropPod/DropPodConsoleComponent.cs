using Content.Shared.Whitelist;
using Content.Shared.Pinpointer;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Shared._Impstation.DropPod;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState(true)]
public sealed partial class DropPodConsoleComponent : Component
{
    /// <summary>
    /// List of available drop points
    /// </summary>
    [DataField, AutoNetworkedField]
    public Dictionary<NetEntity, string?> AvailablePoints = new();

    /// <summary>
    /// Name of the Drop Pod menu
    /// </summary>
    [DataField]
    public LocId Name;

    /// <summary>
    /// Blacklist for all banned travel beacons
    /// </summary>
    [DataField]
    public EntityWhitelist Blacklist;
}