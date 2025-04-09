using Content.Client.GameObjects.EntitySystems;
using Content.Client.Photography.UI;
using Content.Shared.GameObjects.Components.Photography;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
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
using System.IO;
using System.Threading.Tasks;

namespace Content.Client.Photography
{
    [RegisterComponent]
    public sealed partial class PhotoCameraComponent : SharedPhotoCameraComponent
    {

#pragma warning disable 649
        [Dependency] private readonly IClientNotifyManager _notifyManager = default;
        [Dependency] private readonly IClyde _clyde = default;
        [Dependency] private readonly IEyeManager _eyeManager = default;
        [Dependency] private readonly IPlayerManager _playerManager = default;
        [Dependency] private readonly IResourceManager _resourceManager = default;
#pragma warning restore 649

        private PhotoSystem _photoSystem;

        [ViewVariables(VVAccess.ReadWrite)] public bool _uiUpdateNeeded;
        [ViewVariables] public bool CameraOn { get; private set; } = false;
        [ViewVariables] public int Radius { get; private set; } = 0;
        [ViewVariables] public int Film { get; private set; } = 0;
        [ViewVariables] public int FilmMax { get; private set; } = 0;

        public override void Initialize()
        {
            base.Initialize();

            _photoSystem = EntitySystem.Get<PhotoSystem>();
        }

        public override void HandleComponentState(ComponentState curState, ComponentState nextState)
        {
            if (!(curState is PhotoCameraComponentState camera))
                return;

            CameraOn = camera.On;
            Radius = camera.Radius;
            Film = camera.Film;
            FilmMax = camera.FilmMax;
            _uiUpdateNeeded = true;
        }

        public Control MakeControl() => new PhotoCameraStatusControl(this);

        public async void TryTakePhoto(EntityUid author, Vector2 photoCenter, bool suicide = false)
        {
            if (!CameraOn)
            {
                _notifyManager.PopupMessageCursor(_playerManager.LocalPlayer.ControlledEntity, Loc.GetString("Turn the {0} on first!", Owner.Name));
                return;
            }

            if(Film <= 0)
            {
                _notifyManager.PopupMessageCursor(_playerManager.LocalPlayer.ControlledEntity, Loc.GetString("No film!"));
                return;
            }

            //Play sounds
            SendNetworkMessage(new TakingPhotoMessage());

            //Take a screenshot before the UI, and then crop it to the photo radius
            var screenshot = await _clyde.ScreenshotAsync(ScreenshotType.BeforeUI);
            var cropDimensions = EyeManager.PixelsPerMeter * (Radius * 4);

            //We'll try and center, but otherwise we'll shift the box so it doesn't go outside the viewport
            var cropX = (int)Math.Clamp(Math.Floor(photoCenter.X - cropDimensions / 2), 0, screenshot.Width - cropDimensions);
            var cropY = (int)Math.Clamp(Math.Floor(photoCenter.Y - cropDimensions / 2), 0, screenshot.Height - cropDimensions);

            Logger.InfoS("photo", $"cropX:{cropX}, cropY:{cropY}, w:{screenshot.Width}, h:{screenshot.Height}");

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

                SendNetworkMessage(new TookPhotoMessage(author, file.CopyToArray(), suicide));
            }
        }
    }
}
