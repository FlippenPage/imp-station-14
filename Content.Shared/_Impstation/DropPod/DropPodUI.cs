using Robust.Shared.Serialization;

namespace Content.Shared._Impstation.DropPod;

[Serializable, NetSerializable]
public enum DropPodLocationsUiKey : byte
{
    Key
}

/// <summary>
/// Sends message to request the drop pod be FTL'd to a certain beacon
/// </summary>
[Serializable, NetSerializable]
public sealed class DropPodDestinationMessage(NetEntity netEnt, string pointName) : BoundUserInterfaceMessage
{
    public NetEntity NetEnt = netEnt;
    public string PointName = pointName;
}
