using Content.Client.Stylesheets;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Client.Photography.UI;
public sealed class PhotoFilmStatusControl : Control
{
    private readonly PhotoFilmComponent _parent;
    private readonly RichTextLabel _label;

    public PhotoFilmStatusControl(PhotoFilmComponent parent)
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
        message.AddMarkupOrThrow(Loc.GetString("Photos: [color=white]{0}/{1}[/color]", ("0", _parent.Film), ("1", _parent.FilmMax)));

        _label.SetMessage(message);
    }
}
