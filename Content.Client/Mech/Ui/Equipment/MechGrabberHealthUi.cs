using Content.Client.HealthAnalyzer.UI;
using Content.Client.UserInterface.Fragments;
using Content.Shared.Mech;
using Content.Shared.MedicalScanner;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;

namespace Content.Client.Mech.Ui.Equipment;

public sealed partial class MechGrabberHealthUi : UIFragment
{
    private HealthAnalyzerWindow? _fragment;

    public override Control GetUIFragmentRoot()
    {
        return _fragment!;
    }

    public override void Setup(BoundUserInterface userInterface, EntityUid? fragmentOwner)
    {
        if (fragmentOwner == null)
            return;

        _fragment = new HealthAnalyzerWindow();
    }

    protected void ReceiveMessage(BoundUserInterfaceMessage message)
    {
        if (_fragment == null)
            return;

        if (message is not HealthAnalyzerScannedUserMessage cast)
            return;

        _fragment.Populate(cast);
    }

    public override void UpdateState(BoundUserInterfaceState state)
    {
        if (state is not MechGrabberUiState grabberState)
            return;
    }
}
