using Content.Shared.Photography;

namespace Content.Client.Photography;

[RegisterComponent]
public sealed partial class PhotoFilmComponent : SharedPhotoFilmComponent
{
    [ViewVariables(VVAccess.ReadWrite)]
    public bool UiUpdateNeeded;

    [ViewVariables]
    public int Film { get; private set; } = 10;

    [ViewVariables]
    public int FilmMax { get; private set; } = 10;
}
