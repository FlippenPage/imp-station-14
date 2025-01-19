using System.Linq;
using Content.Shared.Interaction;
using Content.Server.Popups;
using Content.Shared.UserInterface;
using Content.Shared._Impstation.CosmicCult;
using Content.Server._Impstation.CosmicCult.Components;
using Content.Shared._Impstation.CosmicCult.Components;
using Robust.Server.GameObjects;
using Robust.Shared.Prototypes;

namespace Content.Server._Impstation.CosmicCult.EntitySystems;

public sealed class CosmicMonumentSystem : EntitySystem
{
        [Dependency] private readonly UserInterfaceSystem _uiSystem = default!;

        public override void Initialize()
        {
            base.Initialize();
            SubscribeLocalEvent<CosmicMonumentComponent, AfterActivatableUIOpenEvent>(OnUIOpen);
        }
        private void OnUIOpen(EntityUid uid, CosmicMonumentComponent component, AfterActivatableUIOpenEvent args)
        {
            UpdateUserInterface(uid, component);
        }
        public void UpdateUserInterface(EntityUid uid, CosmicMonumentComponent component)
        {
            if (!_uiSystem.HasUi(uid, CosmicMonumentUiKey.Key))
                return;
        }        
}
