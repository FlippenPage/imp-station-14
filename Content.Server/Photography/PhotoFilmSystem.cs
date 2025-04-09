using Content.Shared.Examine;
using Robust.Shared.Utility;

namespace Content.Server.Photography
{
    public sealed class PhotoFilmSystem : EntitySystem
    {
        public override void Initialize()
        {
            base.Initialize();
            SubscribeLocalEvent<PhotoFilmComponent, ExaminedEvent>(OnExamine);
        }

        private void OnExamine(EntityUid uid, PhotoFilmComponent component, ExaminedEvent args)
        {
            if (!args.IsInDetailsRange)
                return;

            var message = new FormattedMessage();
            message.AddMarkupOrThrow(Loc.GetString("Photos: [color=white]{0}/{1}[/color]", ("0", component.FilmInt), ("1", component.FilmMaxInt)));
        }
    }
}
