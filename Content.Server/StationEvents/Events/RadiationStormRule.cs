using Content.Shared.GameTicking.Components;
using Content.Server.Station.Components;
using Content.Server.Station.Systems;
using Content.Server.StationEvents.Components;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Random;

namespace Content.Server.StationEvents.Events
{
    internal sealed class RadiationStormRule : StationEventSystem<RadiationStormRuleComponent>
    {
        // Based on Goonstation style radiation storm with some TG elements (announcer, etc.)
        [Dependency] private readonly IEntityManager _entityManager = default!;
        [Dependency] private readonly IRobustRandom _robustRandom = default!;
        [Dependency] private readonly SharedMapSystem _mapSystem = default!;

        private StationSystem _stationSystem = default!;

        private void ResetTimeUntilPulse(EntityUid uid, RadiationStormRuleComponent component)
        {
            component.TimeUntilPulse = _robustRandom.NextFloat() * (component.MaxPulseDelay - component.MinPulseDelay) + component.MinPulseDelay;
        }

        protected override void Started(EntityUid uid, RadiationStormRuleComponent component, GameRuleComponent gameRule, GameRuleStartedEvent args)
        {
            if (!TryComp<StationEventComponent>(uid, out var stationEvent))
                return;

            _entityManager.EntitySysManager.Resolve(ref _stationSystem);
            ResetTimeUntilPulse(uid, component);

            if (!TryGetRandomStation(out var chosenStation))
                return;

            component.Target = chosenStation.Value;
            base.Started(uid, component, gameRule, args);
        }

        protected override void ActiveTick(EntityUid uid, RadiationStormRuleComponent component, GameRuleComponent gameRule, float frameTime)
        {
            base.ActiveTick(uid, component, gameRule, frameTime);

            if (component.Target.Valid == false)
            {
                return;
            }

            component.TimeUntilPulse -= frameTime;

            if (component.TimeUntilPulse <= 0.0f)
            {
                // Account for split stations by just randomly picking a piece of it.
                var possibleTargets = _entityManager.GetComponent<StationDataComponent>(component.Target).Grids;
                if (possibleTargets.Count == 0)
                {
                    return;
                }

                var stationEnt = _robustRandom.Pick(possibleTargets);

                if (!_entityManager.TryGetComponent<MapGridComponent>(stationEnt, out var grid))
                    return;

                if (_mapSystem.IsPaused(grid.Owner))
                    return;

                SpawnPulse(uid, component, grid);
            }
        }

        private void SpawnPulse(EntityUid uid, RadiationStormRuleComponent component, MapGridComponent mapGrid)
        {
            if (!TryFindRandomGrid(mapGrid, out var coordinates))
                return;

            _entityManager.SpawnEntity("RadiationPulse", coordinates);
            ResetTimeUntilPulse(uid, component);
        }

        public void SpawnPulseAt(EntityCoordinates at)
        {
            var pulse = IoCManager.Resolve<IEntityManager>()
                .SpawnEntity("RadiationPulse", at);
        }

        private bool TryFindRandomGrid(MapGridComponent mapGrid, out EntityCoordinates coordinates)
        {
            if (!(mapGrid.Owner).IsValid())
            {
                coordinates = default;
                return false;
            }

            var bounds = mapGrid.LocalAABB;
            var randomX = _robustRandom.Next((int)bounds.Left, (int)bounds.Right);
            var randomY = _robustRandom.Next((int)bounds.Bottom, (int)bounds.Top);

            var tile = new Vector2i(randomX, randomY);

            coordinates = _mapSystem.ToCoordinates(mapGrid.Owner, tile);

            // TODO: Need to get valid tiles? (maybe just move right if the tile we chose is invalid?)
            if (!coordinates.IsValid(_entityManager))
            {
                coordinates = default;
                return false;
            }
            return true;
        }
    }
}
