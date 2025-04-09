using Content.Server.GameObjects.EntitySystems;
using Content.Shared.GameObjects.Components.Photography;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;

namespace Content.Server.Photography
{
    [RegisterComponent]
    public sealed partial class PhotoFilmComponent : SharedPhotoFilmComponent
    {
        public int FilmInt = 10;
        public int FilmMaxInt = 10;

        [ViewVariables]
        public int Film
        {
            get => FilmInt;
            set
            {
                FilmInt = value;
                Dirty();
            }
        }
        [ViewVariables] public int FilmMax
        {
            get => FilmMaxInt;
            set
            {
                FilmMaxInt = value;
                Dirty();
            }
        }

        public override void Initialize()
        {
            base.Initialize();

            Dirty();
        }

        public override void ExposeData(ObjectSerializer serializer)
        {
            base.ExposeData(serializer);

            serializer.DataField(ref FilmInt, "film", 10);
            serializer.DataField(ref FilmMaxInt, "maxfilm", 10);
        }

        public override ComponentState GetComponentState()
        {
            return new PhotoFilmComponentState(Film, FilmMax);
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
}
