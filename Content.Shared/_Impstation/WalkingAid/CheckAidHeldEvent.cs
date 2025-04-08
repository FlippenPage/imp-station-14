using Content.Shared.Inventory;

namespace Content.Shared._Impstation.WalkingAid
{
    /// <summary>
    /// Checks if a walking aid is being held.
    /// </summary>
    public sealed class CheckAidHeldEvent : HandledEntityEventArgs, IInventoryRelayEvent
    {
        public SlotFlags TargetSlots => SlotFlags.WITHOUT_POCKET;
    }
}