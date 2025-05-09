using Content.Shared._EE.Supermatter.Components;
using Content.Shared._Impstation.Supermatter.CascadeCrystal.Components;
using Content.Shared.DoAfter;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Robust.Shared.Containers;

namespace Content.Shared._Impstation.Supermatter.CascadeCrystal;

public sealed class CascadeCrystalSystem : EntitySystem
{
    [Dependency] private readonly SharedDoAfterSystem _doAfterSystem = default!;
    [Dependency] private readonly SharedContainerSystem _containerSystem = default!;
    [Dependency] private readonly SharedTransformSystem _transformSystem = default!;
    [Dependency] private readonly SharedPopupSystem _popupSystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<CascadeCrystalComponent, AfterInteractEvent>(OnAfterInteract);
        SubscribeLocalEvent<CascadeCrystalComponent, EvilCrystalDoAfterEvent>(OnDoAfter);
    }

    private void OnAfterInteract(EntityUid uid, CascadeCrystalComponent component, AfterInteractEvent args)
    {
        if (args.Handled || args.Target is null || !args.CanReach)
            return;

        if (TryStartEvilCrystalDoafter(uid, args.Target.Value, args.User))
            args.Handled = true;
    }

    private bool TryStartEvilCrystalDoafter(Entity<SupermatterComponent?> supermatter, EntityUid crystal, EntityUid user)
    {
        var doAfter =
            new DoAfterArgs(EntityManager, user, 5f, new EvilCrystalDoAfterEvent(), crystal, target: supermatter, used: crystal)
            {
                BreakOnDamage = true,
                BreakOnMove = true,
            };
        _doAfterSystem.TryStartDoAfter(doAfter);
        return true;
    }

    private void OnDoAfter(EntityUid uid, CascadeCrystalComponent component, DoAfterEvent args)
    {
        if (args.Handled || args.Args.Target == null)
            return;

        if (_containerSystem.IsEntityInContainer(args.Args.Target.Value))
        {
            args.Handled = true;
            return;
        }

        var coords = _transformSystem.GetMapCoordinates(args.Args.Target.Value);
        EntityUid popupEnt = default!;

        _popupSystem.PopupEntity(Loc.GetString("cascade-crystal-success", ("target", args.Args.Target.Value), ("crystal", uid)),
            popupEnt, args.Args.User);

        QueueDel(uid);
        args.Handled = true;
    }
}
