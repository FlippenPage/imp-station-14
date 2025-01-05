using Content.Client.Kudzu;
using Content.Shared.Spreader;
using Robust.Client.GameObjects;

namespace Content.Client.CrystalMass;

public sealed class CrystalMassVisualsSystem : VisualizerSystem<CrystalMassVisualsComponent>
{
    protected override void OnAppearanceChange(EntityUid uid, CrystalMassVisualsComponent component, ref AppearanceChangeEvent args)
    {

        if (args.Sprite == null)
            return;
        if (AppearanceSystem.TryGetData<int>(uid, CrystalMassVisuals.Variant, out var var, args.Component))
        {
            var index = args.Sprite.LayerMapReserveBlank(component.Layer);
            args.Sprite.LayerSetState(index, $"crystal_cascade_{var}");
        }
    }
}
