using Content.Shared.Body.Systems;
using Content.Shared.Movement.Systems;

namespace Content.Shared._Impstation.Traits.Assorted;

public sealed class LegWeaknessSystem : EntitySystem
{
    [Dependency] private readonly MovementSpeedModifierSystem _movementSpeedModifierSystem = default!;
    [Dependency] private readonly SharedBodySystem _bodySystem = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<LegWeaknessComponent, ComponentStartup>(OnStartup);
        SubscribeLocalEvent<LegWeaknessComponent, ComponentShutdown>(OnShutdown);
    }

    private void OnStartup(EntityUid uid, LegWeaknessComponent component, ComponentStartup args)
    {
        _movementSpeedModifierSystem.ChangeBaseSpeed(uid, 0, 2.5f, 20f);
    }

    private void OnShutdown(EntityUid uid, LegWeaknessComponent component, ComponentShutdown args)
    {
        _bodySystem.UpdateMovementSpeed(uid);
    }

}