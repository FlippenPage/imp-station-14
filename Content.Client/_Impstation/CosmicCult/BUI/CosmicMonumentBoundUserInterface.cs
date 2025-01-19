using JetBrains.Annotations;
using Robust.Client.UserInterface;
using Robust.Shared.Prototypes;
using Content.Client._Impstation.CosmicCult.UI;

namespace Content.Client._Impstation.CosmicCult;

public sealed class CosmicMonumentBoundUserInterface : BoundUserInterface
{
    [ViewVariables]
    private CosmicMonumentMenu? _Menu;
    public CosmicMonumentBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
    }

    protected override void Open()
    {
        base.Open();

        _Menu = this.CreateWindow<CosmicMonumentMenu>();
    }
}
