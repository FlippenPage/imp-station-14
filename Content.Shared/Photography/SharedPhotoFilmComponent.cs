using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Content.Shared.GameObjects.Components.Photography
{
    public sealed partial class SharedPhotoFilmComponent : Component
    {
        public string Name => "PhotoFilm";
        public int NetID;
    }

    [NetSerializable, Serializable]
    public class PhotoFilmComponentState : ComponentState
    {
        public readonly int Film;
        public readonly int FilmMax;
        public PhotoFilmComponentState(int film, int filmMax) : base(ContentNetIDs.PHOTO_FILM)
        {
            Film = film;
            FilmMax = filmMax;
        }
    }
}
