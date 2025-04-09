using Content.Client.Stylesheets;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Client.Photography.UI
{
    public sealed class PhotoCameraStatusControl : Control
    {
        private readonly PhotoCameraComponent _parent;
        private readonly RichTextLabel _label;

        public PhotoCameraStatusControl(PhotoCameraComponent parent)
        {
            _parent = parent;
            _label = new RichTextLabel { StyleClasses = { StyleNano.StyleClassItemStatus } };
            AddChild(_label);

            parent.UiUpdateNeeded = true;
        }

        public void Update(FrameEventArgs args)
        {
            if (!_parent.UiUpdateNeeded)
            {
                return;
            }

            _parent.UiUpdateNeeded = false;

            var message = new FormattedMessage();

            if (_parent.CameraOn)
            {
                message.AddMarkupOrThrow(Loc.GetString("[color=green]On[/color]\n"));
            }
            else
            {
                message.AddMarkupOrThrow(Loc.GetString("[color=red]Off[/color]\n"));
            }

            message.AddMarkupOrThrow(Loc.GetString("Film: [color={color}]{film}/{max}[/color], ",
            ("{color}", _parent.Film <= 0 ? "red" : "white"), ("film", _parent.Film), ("max", _parent.FilmMax)));
            message.AddMarkupOrThrow(Loc.GetString("Radius: [color=white]{0}x{0}[/color]", ("0", _parent.Radius * 2)));

            _label.SetMessage(message);
        }
    }
}