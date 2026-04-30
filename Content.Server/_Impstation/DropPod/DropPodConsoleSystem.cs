using Content.Shared.Pinpointer;
using Content.Shared.UserInterface;
using Content.Shared.Whitelist;
using Content.Shared._Impstation.DropPod;

namespace Content.Server._Impstation.DropPod;


/// <summary>
/// its a ripoff of TeleportLocations with some different params, shoulda made yer shit generic Keron!!!
/// </summary>
public sealed partial class DropPodConsoleSystem : SharedDropPodConsoleSystem
{
    [Dependency] private readonly DropPodSystem _dropPod = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<DropPodConsoleComponent, MapInitEvent>(OnMapInit);
        SubscribeLocalEvent<DropPodConsoleComponent, BeforeActivatableUIOpenEvent>(OnBeforeUiOpen);
    }

    [Dependency] private readonly EntityWhitelistSystem _whitelist = default!;

    private void OnMapInit(Entity<DropPodConsoleComponent> ent, ref MapInitEvent args)
    {
        UpdateDroppableStationPoints(ent);
    }

    private void OnBeforeUiOpen(Entity<DropPodConsoleComponent> ent, ref BeforeActivatableUIOpenEvent args)
    {
        UpdateDroppableStationPoints(ent);
    }

    private void UpdateDroppableStationPoints(Entity<DropPodConsoleComponent> ent)
    {
        ent.Comp.AvailablePoints.Clear();

        var allEnts = AllEntityQuery<NavMapBeaconComponent>();

        while (allEnts.MoveNext(out var beaconEnt, out var dropPointComp))
        {
            if (_whitelist.IsWhitelistPassOrNull(ent.Comp.Blacklist, beaconEnt))
                continue;

            var beaconName = dropPointComp.Text;
            var beaconNet = GetNetEntity(beaconEnt);

            ent.Comp.AvailablePoints.Add(beaconNet, beaconName);
        }

        Dirty(ent);
    }

    private void OnDropPodDropRequest(Entity<DropPodConsoleComponent> ent, ref DropPodDestinationMessage args)
    {
        if (!TryGetEntity(args.NetEnt, out var beacon) || TerminatingOrDeleted(beacon))
            return;

        var uidXform = Transform(ent);
        var coordinates = uidXform.Coordinates;
        var gridUid = _transform.GetGrid(coordinates);

        if (gridUid == null)
            return;

        if (!TryComp<DropPodComponent>(ent, out var dropPodComp))
            return;

        _dropPod.OnDropPodDoThing(gridUid.Value, dropPodComp, beacon);
        //lock console
    }
}
