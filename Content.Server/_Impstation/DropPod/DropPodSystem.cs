using Content.Server.Explosion.EntitySystems;
using Content.Server.Shuttles.Events;
using Content.Server.Shuttles.Systems;
using Content.Server.Shuttles.Components;
using Robust.Shared.Random;
using Content.Shared._Impstation.DropPod;

namespace Content.Server._Impstation.DropPod;

public sealed partial class DropPodSystem : SharedDropPodSystem
{
    [Dependency] private readonly ShuttleSystem _shuttle = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly ExplosionSystem _explosion = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;

    public override void Initialize()
    {
        base.Initialize();
    }

    //layout
    //need console with custom bui, single time use ftl to beacon button
    //override smimsh for massive damage

    public void OnDropPodDoThing(EntityUid uid, DropPodComponent component, EntityUid? beacon)
    {
        if (!TryComp<ShuttleComponent>(uid, out var shuttleComp))
            return;

        //get coords from ui checked beacon
        if (!TryComp<TransformComponent>(beacon, out var xform))
            return;

        var targetCoordinates = xform.Coordinates;

        //random ftl angle
        var randAngle = _random.NextAngle();
        _shuttle.FTLToCoordinates(uid, shuttleComp, targetCoordinates, randAngle);
    }

    private void HijackSmimsh(EntityUid uid, DropPodComponent component, ShuttleFlattenEvent args)
    {
        //check target grid for everything, cry if unfulfilled
        //was going to use shuttle collision but uhhhh its goobcode
        //explode shit instead
        //_explosion.QueueExplosion(_transform.ToMapCoordinates(targetCoordinates),
        //typeId: ExplosionSystem.DefaultExplosionPrototypeId,
        //totalIntensity: 30,
        //slope: 1,
        //maxTileIntensity: 2,
        //cause: uid);
    }
}