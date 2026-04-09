using Content.Server.Body.Components;

namespace Content.Server._Impstation.Mummy.BreathingImmune;

public sealed class BreathingImmuneSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<BreathingImmuneComponent, ComponentInit>(OnComponentInit);
        SubscribeLocalEvent<BreathingImmuneComponent, ComponentRemove>(OnComponentRemove);
    }

    private void OnComponentInit(EntityUid uid, BreathingImmuneComponent comp, ComponentInit args)
    {
        if (TryComp<RespiratorComponent>(uid, out var respirator))
        {
            respirator.HasImmunity = true;
        }
    }

    private void OnComponentRemove(EntityUid uid, BreathingImmuneComponent comp, ComponentRemove args)
    {
        if (TryComp<RespiratorComponent>(uid, out var respirator))
        {
            respirator.HasImmunity = false;
        }
    }
}