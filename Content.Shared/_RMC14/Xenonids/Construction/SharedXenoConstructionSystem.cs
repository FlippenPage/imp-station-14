﻿using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Content.Shared._RMC14.Xenonids.Construction.Events;
using Content.Shared._RMC14.Xenonids.Construction.Nest;
using Content.Shared._RMC14.Xenonids.Egg;
using Content.Shared._RMC14.Xenonids.Plasma;
using Content.Shared._RMC14.Xenonids.Weeds;
using Content.Shared._RMC14.Map;
using Content.Shared.Actions;
using Content.Shared.Actions.Events;
using Content.Shared.Administration.Logs;
using Content.Shared.Atmos;
using Content.Shared.Buckle.Components;
using Content.Shared.Coordinates;
using Content.Shared.Coordinates.Helpers;
using Content.Shared.Damage;
using Content.Shared.Database;
using Content.Shared.Destructible;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.FixedPoint;
using Content.Shared.Interaction;
using Content.Shared.Maps;
using Content.Shared.Popups;
using Content.Shared.Projectiles;
using Content.Shared.Prototypes;
using Content.Shared.Tag;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Network;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using static Content.Shared.Physics.CollisionGroup;

namespace Content.Shared._RMC14.Xenonids.Construction;

public sealed class SharedXenoConstructionSystem : EntitySystem
{
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly SharedActionsSystem _actions = default!;
    [Dependency] private readonly ISharedAdminLogManager _adminLogs = default!;
    [Dependency] private readonly IComponentFactory _compFactory = default!;
    [Dependency] private readonly DamageableSystem _damageable = default!;
    [Dependency] private readonly SharedDoAfterSystem _doAfter = default!;
    [Dependency] private readonly SharedInteractionSystem _interaction = default!;
    [Dependency] private readonly EntityLookupSystem _lookup = default!;
    [Dependency] private readonly IMapManager _map = default!;
    [Dependency] private readonly SharedMapSystem _mapSystem = default!;
    [Dependency] private readonly INetManager _net = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly IPrototypeManager _prototype = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;
    [Dependency] private readonly TagSystem _tags = default!;
    [Dependency] private readonly TurfSystem _turf = default!;
    [Dependency] private readonly SharedUserInterfaceSystem _ui = default!;
    [Dependency] private readonly XenoNestSystem _xenoNest = default!;
    [Dependency] private readonly XenoPlasmaSystem _xenoPlasma = default!;
    [Dependency] private readonly SharedXenoWeedsSystem _xenoWeeds = default!;
    [Dependency] private readonly RMCMapSystem _rmcMap = default!;

    private static readonly ImmutableArray<Direction> Directions = Enum.GetValues<Direction>()
        .Where(d => d != Direction.Invalid)
        .ToImmutableArray();

    private EntityQuery<BlockXenoConstructionComponent> _blockXenoConstructionQuery;
    private EntityQuery<XenoConstructionSupportComponent> _constructionSupportQuery;
    private EntityQuery<XenoConstructionRequiresSupportComponent> _constructionRequiresSupportQuery;
    private EntityQuery<TransformComponent> _transformQuery;
    private EntityQuery<XenoConstructComponent> _xenoConstructQuery;
    private EntityQuery<XenoEggComponent> _xenoEggQuery;

    private const string XenoStructuresAnimation = "RMCEffect";
    private const string XenoHiveCoreNodeId = "HiveCoreXenoConstructionNode";

    private static readonly ProtoId<TagPrototype> AirlockTag = "Airlock";
    private static readonly ProtoId<TagPrototype> StructureTag = "Structure";

    public override void Initialize()
    {
        _blockXenoConstructionQuery = GetEntityQuery<BlockXenoConstructionComponent>();
        _constructionSupportQuery = GetEntityQuery<XenoConstructionSupportComponent>();
        _constructionRequiresSupportQuery = GetEntityQuery<XenoConstructionRequiresSupportComponent>();
        _transformQuery = GetEntityQuery<TransformComponent>();
        _xenoConstructQuery = GetEntityQuery<XenoConstructComponent>();
        _xenoEggQuery = GetEntityQuery<XenoEggComponent>();

        SubscribeLocalEvent<XenoConstructionComponent, XenoPlantWeedsActionEvent>(OnXenoPlantWeedsAction);

        SubscribeLocalEvent<XenoConstructionComponent, XenoChooseStructureActionEvent>(OnXenoChooseStructureAction);

        SubscribeLocalEvent<XenoConstructionComponent, XenoSecreteStructureActionEvent>(OnXenoSecreteStructureAction);
        SubscribeLocalEvent<XenoConstructionComponent, XenoSecreteStructureDoAfterEvent>(OnXenoSecreteStructureDoAfter);

        SubscribeLocalEvent<XenoConstructionComponent, XenoOrderConstructionActionEvent>(OnXenoOrderConstructionAction);
        SubscribeLocalEvent<XenoConstructionComponent, XenoOrderConstructionDoAfterEvent>(OnXenoOrderConstructionDoAfter);

        SubscribeLocalEvent<XenoChooseConstructionActionComponent, XenoConstructionChosenEvent>(OnActionConstructionChosen);
        SubscribeLocalEvent<XenoConstructionActionComponent, ValidateActionWorldTargetEvent>(OnSecreteActionValidateTarget);

        SubscribeLocalEvent<XenoWeedsComponent, XenoStructureRepairedEvent>(OnWeedStructureRepair);

        SubscribeLocalEvent<XenoConstructionSupportComponent, ComponentRemove>(OnCheckAdjacentCollapse);
        SubscribeLocalEvent<XenoConstructionSupportComponent, EntityTerminatingEvent>(OnCheckAdjacentCollapse);

        SubscribeLocalEvent<DeleteXenoResinOnHitComponent, ProjectileHitEvent>(OnDeleteXenoResinHit);

        Subs.BuiEvents<XenoConstructionComponent>(XenoChooseStructureUI.Key,
            subs =>
            {
                subs.Event<XenoChooseStructureBuiMsg>(OnXenoChooseStructureBui);
            });

        Subs.BuiEvents<XenoConstructionComponent>(XenoOrderConstructionUI.Key,
            subs =>
            {
                subs.Event<XenoOrderConstructionBuiMsg>(OnXenoOrderConstructionBui);
            });

        UpdatesAfter.Add(typeof(SharedPhysicsSystem));
    }

    private void OnXenoPlantWeedsAction(Entity<XenoConstructionComponent> xeno, ref XenoPlantWeedsActionEvent args)
    {
        var coordinates = _transform.GetMoverCoordinates(xeno).SnapToGrid(EntityManager, _map);
        if (_transform.GetGrid(coordinates) is not { } gridUid ||
            !TryComp(gridUid, out MapGridComponent? gridComp))
        {
            return;
        }

        var grid = new Entity<MapGridComponent>(gridUid, gridComp);
        if (_xenoWeeds.IsOnWeeds(grid, coordinates, true))
        {
            _popup.PopupClient(Loc.GetString("cm-xeno-weeds-source-already-here"), xeno.Owner, xeno.Owner);
            return;
        }

        var tile = _mapSystem.CoordinatesToTile(gridUid, gridComp, coordinates);
        if (!_xenoWeeds.CanSpreadWeedsPopup(grid, tile, xeno, args.UseOnSemiWeedable, true))
            return;

        if (!_xenoWeeds.CanPlaceWeedsPopup(xeno, grid, coordinates, false))
            return;

        if (!_xenoPlasma.TryRemovePlasmaPopup(xeno.Owner, args.PlasmaCost))
            return;

        args.Handled = true;
        if (_net.IsServer)
        {
            var weeds = Spawn(args.Prototype, coordinates);
            _adminLogs.Add(LogType.RMCXenoPlantWeeds, $"Xeno {ToPrettyString(xeno):xeno} planted weeds {ToPrettyString(weeds):weeds} at {coordinates}");
        }
    }

    private void OnXenoChooseStructureAction(Entity<XenoConstructionComponent> xeno, ref XenoChooseStructureActionEvent args)
    {
        args.Handled = true;
        _ui.TryOpenUi(xeno.Owner, XenoChooseStructureUI.Key, xeno);
    }

    private void OnXenoChooseStructureBui(Entity<XenoConstructionComponent> xeno, ref XenoChooseStructureBuiMsg args)
    {
        if (!xeno.Comp.CanBuild.Contains(args.StructureId))
            return;

        xeno.Comp.BuildChoice = args.StructureId;
        Dirty(xeno);

        var ev = new XenoConstructionChosenEvent(args.StructureId);
        foreach (var (id, _) in _actions.GetActions(xeno))
        {
            RaiseLocalEvent(id, ref ev);
        }
    }

    private void OnXenoSecreteStructureAction(Entity<XenoConstructionComponent> xeno, ref XenoSecreteStructureActionEvent args)
    {
        var snapped = args.Target.SnapToGrid(EntityManager, _map);

        if (xeno.Comp.BuildChoice is not { } choice ||
            !CanSecreteOnTilePopup(xeno, choice, args.Target, true, true))
        {
            return;
        }

        var attempt = new XenoSecreteStructureAttemptEvent(args.Target);
        RaiseLocalEvent(xeno, ref attempt);

        if (attempt.Cancelled)
            return;

        var effectId = XenoStructuresAnimation + choice;
        var coordinates = GetNetCoordinates(args.Target);
        var entityCoords = GetCoordinates(coordinates);
        EntityUid? effect = null;

        if (_net.IsServer && _prototype.HasIndex(effectId))
        {
            effect = Spawn(effectId, entityCoords);
            RaiseNetworkEvent(new XenoConstructionAnimationStartEvent(GetNetEntity(effect.Value), GetNetEntity(xeno)), Filter.PvsExcept(effect.Value));
        }

        var buildMult = GetBuildSpeed(choice) ?? 1;

        var ev = new XenoSecreteStructureDoAfterEvent(coordinates, choice, GetNetEntity(effect));
        args.Handled = true;
        var doAfter = new DoAfterArgs(EntityManager, xeno, xeno.Comp.BuildDelay * buildMult, ev, xeno)
        {
            BreakOnMove = true
        };

        if (!_doAfter.TryStartDoAfter(doAfter))
        {
            if (effect != null && _net.IsServer)
                QueueDel(effect);
        }
    }

    private void OnXenoSecreteStructureDoAfter(Entity<XenoConstructionComponent> xeno, ref XenoSecreteStructureDoAfterEvent args)
    {
        if (_net.IsServer && args.Effect != null)
            QueueDel(EntityManager.GetEntity(args.Effect));

        if (args.Handled || args.Cancelled)
            return;

        var coordinates = GetCoordinates(args.Coordinates);
        if (!coordinates.IsValid(EntityManager) ||
            !xeno.Comp.CanBuild.Contains(args.StructureId) ||
            !CanSecreteOnTilePopup(xeno, args.StructureId, GetCoordinates(args.Coordinates), true, true))
        {
            return;
        }

        if (GetStructurePlasmaCost(args.StructureId) is { } cost &&
            !_xenoPlasma.TryRemovePlasmaPopup(xeno.Owner, cost))
        {
            return;
        }

        args.Handled = true;

        // TODO RMC14 stop collision for mobs until they move off
        if (_net.IsServer)
        {
            var structure = Spawn(args.StructureId, coordinates);
            _adminLogs.Add(LogType.RMCXenoConstruct, $"Xeno {ToPrettyString(xeno):xeno} constructed {ToPrettyString(structure):structure} at {coordinates}");
        }
    }

    private void OnXenoOrderConstructionAction(Entity<XenoConstructionComponent> xeno, ref XenoOrderConstructionActionEvent args)
    {
        if (!CanOrderConstructionPopup(xeno, args.Target, null))
            return;

        xeno.Comp.OrderingConstructionAt = args.Target;
        Dirty(xeno);

        args.Handled = true;
        _ui.TryOpenUi(xeno.Owner, XenoOrderConstructionUI.Key, xeno);
    }

    private void OnXenoOrderConstructionBui(Entity<XenoConstructionComponent> xeno, ref XenoOrderConstructionBuiMsg args)
    {
        _ui.CloseUi(xeno.Owner, XenoOrderConstructionUI.Key, xeno);
        if (xeno.Comp.OrderingConstructionAt is not { } target ||
            !xeno.Comp.CanOrderConstruction.Contains(args.StructureId) ||
            !CanOrderConstructionPopup(xeno, target, args.StructureId))
        {
            return;
        }

        if (!_prototype.TryIndex(args.StructureId, out var prototype))
            return;

        var ev = new XenoOrderConstructionDoAfterEvent(args.StructureId, GetNetCoordinates(target));
        var doAfter = new DoAfterArgs(EntityManager, xeno, xeno.Comp.OrderConstructionDelay, ev, xeno)
        {
            BreakOnMove = true,
        };

        _doAfter.TryStartDoAfter(doAfter);
    }

    private void OnXenoOrderConstructionDoAfter(Entity<XenoConstructionComponent> xeno, ref XenoOrderConstructionDoAfterEvent args)
    {
        if (args.Cancelled || args.Handled)
            return;

        args.Handled = true;
        var target = GetCoordinates(args.Coordinates);
        if (!xeno.Comp.CanOrderConstruction.Contains(args.StructureId) ||
            !CanOrderConstructionPopup(xeno, target, args.StructureId) ||
            !TryComp(xeno, out XenoPlasmaComponent? plasma))
        {
            return;
        }

        if (!_prototype.TryIndex(args.StructureId, out var prototype))
            return;

        args.Handled = true;

        if (_net.IsClient)
            return;

        var coordinates = target.SnapToGrid(EntityManager, _map);
        var structure = Spawn(args.StructureId, coordinates);

        _adminLogs.Add(LogType.RMCXenoOrderConstruction, $"Xeno {ToPrettyString(xeno):xeno} ordered construction of {ToPrettyString(structure):structure} at {coordinates}");

        if (!_prototype.TryIndex(args.StructureId, out var structureProto))
        {
            return;
        }

        string msg;
        msg = Loc.GetString("rmc-xeno-order-construction-structure-designated", ("construct", structureProto.Name));
    }

    private void OnActionConstructionChosen(Entity<XenoChooseConstructionActionComponent> xeno, ref XenoConstructionChosenEvent args)
    {
        if (_actions.TryGetActionData(xeno, out var action) &&
            _prototype.HasIndex(args.Choice))
        {
            action.Icon = new SpriteSpecifier.EntityPrototype(args.Choice);
            Dirty(xeno, action);
        }
    }

    private void OnSecreteActionValidateTarget(Entity<XenoConstructionActionComponent> ent, ref ValidateActionWorldTargetEvent args)
    {
        if (!TryComp(args.User, out XenoConstructionComponent? construction))
            return;

        var snapped = args.Target.SnapToGrid(EntityManager, _map);

        var adjustEv = new XenoSecreteStructureAdjustFields(snapped);
        RaiseLocalEvent(args.User, ref adjustEv);

        if (!CanSecreteOnTilePopup((args.User, construction), construction.BuildChoice, args.Target, ent.Comp.CheckStructureSelected, ent.Comp.CheckWeeds))
            args.Cancelled = true;
    }

    private void OnWeedStructureRepair(Entity<XenoWeedsComponent> weedsStructure, ref XenoStructureRepairedEvent args)
    {
        var (ent, comp) = weedsStructure;

        var spreaderComp = EnsureComp<XenoWeedsSpreadingComponent>(ent);
        spreaderComp.SpreadAt = _timing.CurTime;
        Dirty(ent, spreaderComp);

        foreach (var weed in comp.Spread)
        {
            spreaderComp = EnsureComp<XenoWeedsSpreadingComponent>(weed);
            spreaderComp.SpreadAt = _timing.CurTime;
            Dirty(weed, spreaderComp);
        }
    }

    private void OnCheckAdjacentCollapse<T>(Entity<XenoConstructionSupportComponent> ent, ref T args)
    {
        if (!_transformQuery.TryComp(ent, out var xform) ||
            _transform.GetGrid((ent, xform)) is not { Valid: true } gridId ||
            !TryComp(gridId, out MapGridComponent? grid))
        {
            return;
        }

        var coordinates = _transform.GetMapCoordinates((ent, xform));
        var indices = _mapSystem.TileIndicesFor(gridId, grid, coordinates);
        for (var i = 0; i < 4; i++)
        {
            var dir = (AtmosDirection) (1 << i);
            var pos = indices.Offset(dir);
            var anchored = _mapSystem.GetAnchoredEntitiesEnumerator(gridId, grid, pos);
            while (anchored.MoveNext(out var uid))
            {
                if (TerminatingOrDeleted(uid.Value) || EntityManager.IsQueuedForDeletion(uid.Value))
                    continue;

                if (!_constructionRequiresSupportQuery.HasComp(uid))
                    continue;

                if (!IsSupported((gridId, grid), pos))
                    QueueDel(uid);
            }
        }
    }

    private void OnDeleteXenoResinHit(Entity<DeleteXenoResinOnHitComponent> ent, ref ProjectileHitEvent args)
    {
        if (_net.IsServer && _xenoConstructQuery.HasComp(args.Target))
            QueueDel(args.Target);
    }


    public FixedPoint2? GetStructurePlasmaCost(EntProtoId prototype)
    {
        if (_prototype.TryIndex(prototype, out var buildChoice) &&
            buildChoice.TryGetComponent(out XenoConstructionPlasmaCostComponent? cost, _compFactory))
        {
            return cost.Plasma;
        }

        return null;
    }

    private float? GetBuildSpeed(EntProtoId prototype)
    {
        if (_prototype.TryIndex(prototype, out var buildChoice) &&
            buildChoice.TryGetComponent(out XenoConstructionBuildSpeedComponent? speed, _compFactory))
        {
            return speed.BuildTimeMult;
        }

        return null;
    }

    private FixedPoint2? GetStructurePlasmaCost(EntProtoId? building)
    {
        if (building is { } choice &&
            GetStructurePlasmaCost(choice) is { } cost)
        {
            return cost;
        }

        return null;
    }

    private bool TileSolidAndNotBlocked(EntityCoordinates target)
    {
        return target.GetTileRef(EntityManager, _map) is { } tile &&
               !tile.IsSpace() &&
               tile.GetContentTileDefinition().Sturdy &&
               !_turf.IsTileBlocked(tile, Impassable) &&
               !_xenoNest.HasAdjacentNestFacing(target);
    }

    private bool InRangePopup(EntityUid xeno, EntityCoordinates target, float range)
    {
        var origin = _transform.GetMoverCoordinates(xeno);
        target = target.SnapToGrid(EntityManager, _map);
        if (!_transform.InRange(origin, target, range))
        {
            _popup.PopupClient(Loc.GetString("cm-xeno-cant-reach-there"), target, xeno);
            return false;
        }

        if (_transform.InRange(origin, target, 0.75f))
        {
            _popup.PopupClient(Loc.GetString("cm-xeno-cant-build-in-self"), target, xeno);
            return false;
        }

        return true;
    }

    private bool CanSecreteOnTilePopup(Entity<XenoConstructionComponent> xeno, EntProtoId? buildChoice, EntityCoordinates target, bool checkStructureSelected, bool checkWeeds)
    {
        if (checkStructureSelected && buildChoice == null)
        {
            _popup.PopupClient(Loc.GetString("cm-xeno-construction-failed-select-structure"), target, xeno);
            return false;
        }

        if (_transform.GetGrid(target) is not { } gridId ||
            !TryComp(gridId, out MapGridComponent? grid))
        {
            _popup.PopupClient(Loc.GetString("cm-xeno-construction-failed-cant-build"), target, xeno);
            return false;
        }

        target = target.SnapToGrid(EntityManager, _map);
        if (checkWeeds && !_xenoWeeds.IsOnWeeds((gridId, grid), target))
        {
            _popup.PopupClient(Loc.GetString("cm-xeno-construction-failed-need-weeds"), target, xeno);
            return false;
        }

        var ev = new XenoConstructionRangeEvent(xeno.Comp.BuildRange);
        RaiseLocalEvent(xeno, ref ev);
        if (ev.Range > 0 &&
            !InRangePopup(xeno, target, ev.Range.Float()))
        {
            return false;
        }

        if (!TileSolidAndNotBlocked(target))
        {
            _popup.PopupClient(Loc.GetString("cm-xeno-construction-failed-cant-build"), target, xeno);
            return false;
        }

        var tile = _mapSystem.CoordinatesToTile(gridId, grid, target);
        var anchored = _mapSystem.GetAnchoredEntitiesEnumerator(gridId, grid, tile);
        while (anchored.MoveNext(out var uid))
        {
            if (_xenoConstructQuery.HasComp(uid) ||
                _xenoEggQuery.HasComp(uid) ||
                _blockXenoConstructionQuery.HasComp(uid))
            {
                _popup.PopupClient(Loc.GetString("cm-xeno-construction-failed-cant-build"), target, xeno);
                return false;
            }
        }

        if (checkStructureSelected &&
            GetStructurePlasmaCost(buildChoice) is { } cost &&
            !_xenoPlasma.HasPlasmaPopup(xeno.Owner, cost))
        {
            return false;
        }

        if (checkStructureSelected &&
            buildChoice is { } choice &&
            _prototype.TryIndex(choice, out var choiceProto) &&
            choiceProto.HasComponent<XenoConstructionRequiresSupportComponent>(_compFactory))
        {
            if (!IsSupported((gridId, grid), target))
            {
                _popup.PopupClient(Loc.GetString("cm-xeno-construction-failed-requires-support", ("choice", choiceProto.Name)), target, xeno);
                return false;
            }
        }

        return true;
    }

    private bool CanOrderConstructionPopup(Entity<XenoConstructionComponent> xeno, EntityCoordinates target, EntProtoId? choice)
    {
        if (!CanSecreteOnTilePopup(xeno, xeno.Comp.BuildChoice, target, false, false))
            return false;

        if (_transform.GetGrid(target) is not { } gridId ||
            !TryComp(gridId, out MapGridComponent? grid))
        {
            return false;
        }

        return true;
    }

    private bool IsSupported(Entity<MapGridComponent> grid, EntityCoordinates coordinates)
    {
        var indices = _mapSystem.TileIndicesFor(grid, grid, coordinates);
        return IsSupported(grid, indices);
    }

    private bool IsSupported(Entity<MapGridComponent> grid, Vector2i tile)
    {
        var supported = false;
        for (var i = 0; i < 4; i++)
        {
            var dir = (AtmosDirection) (1 << i);
            var pos = tile.Offset(dir);
            var anchored = _mapSystem.GetAnchoredEntitiesEnumerator(grid, grid, pos);
            while (anchored.MoveNext(out var uid))
            {
                if (TerminatingOrDeleted(uid.Value) || EntityManager.IsQueuedForDeletion(uid.Value))
                    continue;

                if (_constructionSupportQuery.HasComp(uid))
                {
                    supported = true;
                    break;
                }
            }

            if (supported)
                break;
        }

        return supported;
    }

    private bool CanPlaceSpaceRequiringStructurePopup(MapCoordinates mapCoords, Entity<MapGridComponent> map, EntityUid user, string structName)
    {
        var mapId = mapCoords.MapId;
        var aabbRange = new Box2(mapCoords.X - 1.5F, mapCoords.Y + 1.5F, mapCoords.X + 1.5F, mapCoords.Y - 1.5F);
        var centerTile = _mapSystem.GetTileRef(map, mapCoords);
        var userCoords = _transform.ToCoordinates(user, mapCoords);

        for (var adjacentX = centerTile.X - 1; adjacentX <= centerTile.X + 1; adjacentX++)
        {
            for (var adjacentY = centerTile.Y - 1; adjacentY <= centerTile.Y + 1; adjacentY++)
            {
                if (adjacentX == adjacentY && adjacentX == 0)
                {
                    continue;
                }

                var adjacentTile = new Vector2i(adjacentX, adjacentY);
                if (_turf.IsTileBlocked(map, adjacentTile, MobMask, map.Comp))
                {
                    _popup.PopupClient(
                    Loc.GetString("rmc-xeno-construction-requires-space", ("choice", structName)),
                    userCoords,
                    user);
                    return false;
                }
            }
        }
        return true;
    }
    public bool CanPlaceXenoStructure(EntityUid user, EntityCoordinates coords, [NotNullWhen(false)] out string? popupType, bool needsWeeds = true)
    {
        popupType = null;
        if (_transform.GetGrid(coords) is not { } gridId ||
    !TryComp(gridId, out MapGridComponent? grid))
        {
            popupType = "rmc-xeno-construction-no-map";
            return false;
        }

        var tile = _mapSystem.TileIndicesFor(gridId, grid, coords);
        var anchored = _mapSystem.GetAnchoredEntitiesEnumerator(gridId, grid, tile);
        var hasWeeds = false;
        while (anchored.MoveNext(out var uid))
        {
            if (HasComp<XenoEggComponent>(uid))
            {
                popupType = "rmc-xeno-construction-blocked";
                return false;
            }

            if (HasComp<XenoConstructComponent>(uid) ||
                _tags.HasAnyTag(uid.Value, StructureTag, AirlockTag) ||
                HasComp<StrapComponent>(uid) ||
                _blockXenoConstructionQuery.HasComp(uid))
            {
                popupType = "rmc-xeno-construction-blocked";
                return false;
            }

            if (HasComp<XenoWeedsComponent>(uid))
                hasWeeds = true;
        }

        if (_turf.IsTileBlocked(gridId, tile, Impassable | MidImpassable | HighImpassable, grid))
        {
            popupType = "rmc-xeno-construction-blocked";
            return false;
        }

        if (!hasWeeds && needsWeeds)
        {
            popupType = "rmc-xeno-construction-must-have-weeds";
            return false;
        }

        return true;
    }
}
