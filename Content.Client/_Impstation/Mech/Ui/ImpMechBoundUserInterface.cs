using Content.Client.UserInterface.Fragments;
using Content.Client._Impstation.Mech.Ui;
using Content.Shared.Mech;
using Content.Shared.Mech.Components;
using JetBrains.Annotations;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;

namespace Content.Client.Mech.Ui;

[UsedImplicitly]
public sealed class ImpMechBoundUserInterface : BoundUserInterface
{
    [ViewVariables]
    private ImpMechMenu? _menu;

    public ImpMechBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
    }

    protected override void Open()
    {
        base.Open();

        //imp edit
        _menu = this.CreateWindowCenteredLeft<ImpMechMenu>();
        _menu.SetEntity(Owner);

        _menu.OnRemoveButtonPressed += uid =>
        {
            SendMessage(new MechEquipmentRemoveMessage(EntMan.GetNetEntity(uid)));
        };

        // imp
        _menu.NameChanged += name => 
        {
            SendMessage(new MechSetNameBuiMessage(name));
        };
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);

        if (state is not MechBoundUiState msg)
            return;
        UpdateEquipmentControls(msg);
        _menu?.UpdateMechStats();
        _menu?.UpdateEquipmentView();
    }

    public void UpdateEquipmentControls(MechBoundUiState state)
    {
        if (!EntMan.TryGetComponent<MechComponent>(Owner, out var mechComp))
            return;

        foreach (var ent in mechComp.EquipmentContainer.ContainedEntities)
        {
            var ui = GetEquipmentUi(ent);
            if (ui == null)
                continue;
            foreach (var (attached, estate) in state.EquipmentStates)
            {
                if (ent == EntMan.GetEntity(attached))
                    ui.UpdateState(estate);
            }
        }
    }

    public UIFragment? GetEquipmentUi(EntityUid? uid)
    {
        var component = EntMan.GetComponentOrNull<UIFragmentComponent>(uid);
        component?.Ui?.Setup(this, uid);
        return component?.Ui;
    }
}

