using Content.Shared.Mech.Components;
using Robust.Shared.Physics;

namespace Content.Shared.Mech.EntitySystems;

public abstract partial class SharedMechSystem
{
    // This class handles actions and hooks n shit for SPECIFIC mechs so as not to muck up upstream's shit.
    private void InitializeSpecific()
    {
        SubscribeLocalEvent<MechComponent, MechTogglePhazonPhaseEvent>(OnTogglePhasingAction);
    }

    private void OnTogglePhasingAction(EntityUid uid, MechComponent component, MechTogglePhazonPhaseEvent args)
    {
        if (args.Handled)
            return;
        args.Handled = true;

        if (!args.Action.Comp.Toggled)
        {
            IsPhasing(uid, true, component);
            _actions.SetToggled(args.Action, true);
        }
        else
        {
            IsPhasing(uid, false, component);
            _actions.SetToggled(args.Action, false);
        }
    }
    public void IsPhasing(EntityUid uid, bool isPhasing, MechComponent component)
    {
        if (!TryComp<FixturesComponent>(uid, out var fixtures))
            return;

        if (isPhasing == true && fixtures != null)
        {
            foreach (var fixture in fixtures.Fixtures.Values)
                _physics.SetHard(uid, fixture, false);
            component.Phasing = true;
            UpdateAppearance(uid, component);
        }
        else
        {
            if (isPhasing == false && fixtures != null)
                foreach (var fixture in fixtures.Fixtures.Values)
                    _physics.SetHard(uid, fixture, true);
            component.Phasing = false;
            UpdateAppearance(uid, component);
        }
    }
    
}