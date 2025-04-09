using Content.Shared.Actions;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Content.Shared.GameObjects.Components.Photography
{
    public sealed partial class SharedPhotoComponent : Component
    {
        public string Name => "Photo";

        public int NetID;
    }

    /// <summary>
    /// Open the photo UI displaying the appropriate photo known as photoId.
    /// Client may request the photo from the server if it doesn't already have it cached.
    /// </summary>
    [Serializable, NetSerializable]
    public sealed partial class OpenPhotoUiEvent : InstantActionEvent
    {
        public readonly string PhotoId;
        public OpenPhotoUiMessage(string photoId)
        {
            PhotoId = photoId;
        }
    }
}
