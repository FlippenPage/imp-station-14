using Content.Client.GameObjects.EntitySystems;
using Content.Client.Photography.UI;
using Content.Shared.GameObjects.Components.Photography;
using Content.Shared.Popups;
using Content.Shared.IdentityManagement;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.ContentPack;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Maths;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.Numerics;
using System.IO;
using System.Threading.Tasks;

namespace Content.Client.Photography;

[RegisterComponent]
public sealed partial class PhotoCameraComponent : SharedPhotoCameraComponent
{
    [Dependency] private readonly SharedPopupSystem _popup = default;
    [Dependency] private readonly IClyde _clyde = default;
    [Dependency] private readonly IEyeManager _eyeManager = default;
    [Dependency] private readonly IPlayerManager _playerManager = default;
    [Dependency] private readonly IResourceManager _resourceManager = default;
    [Dependency] private readonly EntityManager _entityManager = default;

    private PhotoSystem _photoSystem;

    [ViewVariables(VVAccess.ReadWrite)]
    public bool UiUpdateNeeded;

    [ViewVariables]
    public bool CameraOn { get; private set; } = false;

    [ViewVariables]
    public int Radius { get; private set; } = 0;

    [ViewVariables]
    public int Film { get; private set; } = 0;

    [ViewVariables]
    public int FilmMax { get; private set; } = 0;

    public void HandleComponentState(ComponentState curState, ComponentState nextState)
    {
        if (!(curState is PhotoCameraComponentState camera))
            return;

        CameraOn = camera.On;
        Radius = camera.Radius;
        Film = camera.Film;
        FilmMax = camera.FilmMax;
        UiUpdateNeeded = true;
    }

    public Control MakeControl() => new PhotoCameraStatusControl(this);

    public async void TryTakePhoto(EntityUid author, Vector2 photoCenter)
    {
        if (!CameraOn)
        {
            _popup.PopupCursor(_playerManager.LocalSession?.AttachedEntity, Loc.GetString("Turn the {0} on first!", Owner.Name));
            return;
        }

        if(Film <= 0)
        {
            _popup.PopupCursor(_playerManager.LocalSession?.AttachedEntity, Loc.GetString("No film!"));
            return;
        }

        //Play sounds
        var photoEv = new TakingPhotoEvent();
        _entityManager.EventBus.RaiseLocalEvent(author, photoEv);

        //Take a screenshot before the UI, and then crop it to the photo radius
        var screenshot = await _clyde.ScreenshotAsync(ScreenshotType.BeforeUI);
        var cropDimensions = EyeManager.PixelsPerMeter * (Radius * 4);

        //We'll try and center, but otherwise we'll shift the box so it doesn't go outside the viewport
        var cropX = (int)Math.Clamp(Math.Floor(photoCenter.X - cropDimensions / 2), 0, screenshot.Width - cropDimensions);
        var cropY = (int)Math.Clamp(Math.Floor(photoCenter.Y - cropDimensions / 2), 0, screenshot.Height - cropDimensions);

        ISawmill.Info("photo", $"cropX:{cropX}, cropY:{cropY}, w:{screenshot.Width}, h:{screenshot.Height}");

        using (screenshot)
        {
            //Crop screenshot to photo dimensions
            screenshot.Mutate(
                i => i.Crop(new Rectangle(cropX, cropY, cropDimensions, cropDimensions))
            );

            //Store it to disk as a PNG
            var path = await _photoSystem.StoreImagePNG(screenshot);

            await using var file =
                _resourceManager.UserData.Open(path, FileMode.Open);

            var tookEv = new TookPhotoEvent();
            _entityManager.EventBus.RaiseLocalEvent(author, file.CopyToArray());
        }
    }
}
