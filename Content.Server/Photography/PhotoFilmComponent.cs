using Content.Shared.Photography;

namespace Content.Server.Photography;

[RegisterComponent, Access(typeof(PhotoFilmSystem))]
public abstract partial class PhotoFilmComponent : SharedPhotoFilmComponent
{

    [DataField]
    public int FilmInt = 10;

    [DataField]
    public int FilmMaxInt = 10;

    [ViewVariables, DataField]
    public int Film
    {
        get => FilmInt;
        set
        {
            FilmInt = value;
            Dirty();
        }
    }
    [ViewVariables, DataField]
    public int FilmMax
    {
        get => FilmMaxInt;
        set
        {
            FilmMaxInt = value;
            Dirty();
        }
    }
    public bool TakeFilm(int take, out int took)
    {
        if (take <= 0)
        {
            took = 0;
            return false;
        }

        if(FilmInt >= take)
        {
            took = take;
            Film -= take;
        } else
        {
            took = FilmInt;
            Film = 0;
        }

        return FilmInt == 0;
    }
}
