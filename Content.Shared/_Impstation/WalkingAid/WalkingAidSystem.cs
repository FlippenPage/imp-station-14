using Content.Shared.Body.Systems;
using Content.Shared.Hands;
using Content.Shared.Interaction.Events;
using Content.Shared.Traits.Assorted;
using Content.Shared.Movement.Systems;
using Content.Shared._Impstation.Traits.Assorted;
using Content.Shared._Goobstation.Standing;

namespace Content.Shared._Impstation.WalkingAid;

public sealed partial class WalkingAidSystem : EntitySystem
{
    [Dependency] private readonly SharedLayingDownSystem _layingDown = default!;
    [Dependency] private readonly SharedBodySystem _bodySystem = default!;
    [Dependency] private readonly MovementSpeedModifierSystem _movementSpeedModifierSystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<WalkingAidComponent, GotEquippedHandEvent>(OnPickup);
        SubscribeLocalEvent<WalkingAidComponent, DroppedEvent>(OnDropped);
    }


    /// <summary>
    /// If the player has LegWeaknessComp, increase their speed back to regular walking speed while holding.
    /// </summary>
    private void OnPickup(Entity<WalkingAidComponent> uid, ref GotEquippedHandEvent args)
    {
        if (!HasComp<LegWeaknessComponent>(args.User))
        {
            _bodySystem.UpdateMovementSpeed(args.User);
        }
    }

    /// <summary>
    /// If the player loses their walking aid, perform an action based on their condition.
    /// </summary>
    private void OnDropped(Entity<WalkingAidComponent> uid, ref DroppedEvent args)
    {
        if (!HasComp<LegsParalyzedComponent>(args.User))
        {
            _layingDown.TryLieDown(args.User, null, null, DropHeldItemsBehavior.AlwaysDrop);
        }

        if (!HasComp<LegWeaknessComponent>(args.User))
        {
            _movementSpeedModifierSystem.ChangeBaseSpeed(uid, 0, 2.5f, 20f);
        }
    }
}