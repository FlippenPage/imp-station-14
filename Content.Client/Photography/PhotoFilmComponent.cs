using Content.Shared.Photography;

namespace Content.Client.Photography;

[RegisterComponent]
public abstract partial class PhotoFilmComponent : SharedPhotoFilmComponent
{
    [ViewVariables(VVAccess.ReadWrite)]
    public bool UiUpdateNeeded;

    [ViewVariables, DataField]
    public int Film { get; private set; } = 10;

    [ViewVariables, DataField]
    public int FilmMax { get; private set; } = 10;
}
