using Content.Shared.Teleportation;
using Content.Shared._Impstation.DropPod;
using JetBrains.Annotations;
using Robust.Client.UserInterface;

namespace Content.Client._Impstation.DropPod;

[UsedImplicitly]
public sealed class DropPodLocationsBUI : BoundUserInterface
{
    [ViewVariables]
    private DropPodLocationsMenu? _menu;

    public DropPodLocationsBUI(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
    }

    protected override void Open()
    {
        base.Open();

        _menu = this.CreateWindow<DropPodLocationsMenu>();

        if (!EntMan.TryGetComponent<DropPodConsoleComponent>(Owner, out var consoleComp))
            return;

        _menu.Title = Loc.GetString(consoleComp.Name);
        _menu.Beacons = consoleComp.AvailablePoints;
        _menu.AddTeleportButtons();

        _menu.TeleportClicked += (netEnt, pointName) =>
        {
            SendPredictedMessage(new TeleportLocationDestinationMessage(netEnt, pointName));
        };
    }
}
