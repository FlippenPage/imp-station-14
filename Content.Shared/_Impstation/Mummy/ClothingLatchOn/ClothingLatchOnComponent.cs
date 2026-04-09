using Robust.Shared.GameStates;

namespace Content.Shared._Impstation.Mummy.ClothingLatchOn;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class ClothingLatchOnComponent : Component
{
    /// <summary>
    /// Name of the slot the item will attempt to latch on to.
    /// </summary>
    [DataField, AutoNetworkedField]
    public string TargetSlot = "head";

    /// <summary>
    /// Whether or not the item will remove
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool ForceEquip = true;

    /// <summary>
    /// Clothing will attempt to "latch on" to its target slot when someone attempts to drag it.
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool LatchOnPulling = true;


    [DataField]
    public LocId LatchOnPopup = "mummy-crown-latched-on-success";

    public EntityUid? LatchedEnt = null;
}
