using System;
using Content.Shared.Actions;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.GameObjects.Components.Photography
{
    public abstract partial class SharedPhotoCameraComponent : Component
    {
        public string Name => "PhotoCamera";
        public int NetID;
    }

    /// <summary>
    /// The user started taking a photo, this is an async process on the client
    /// So we'll play a "took photo sound" and then a looping "printing a photo" sound
    /// And notify the user that they're just waiting for the camera to print their photo.
    /// </summary>
    [Serializable, NetSerializable]
    public sealed partial class TakingPhotoEvent : InstantActionEvent
    {
        public TakingPhotoEvent()
        {
        }
    }



    /// <summary>
    /// Photo was taken clientside, upload image *SOMEWHERE* so others can access it *SOMEHOW*.
    /// Also makes the photo item.
    /// </summary>
    [Serializable, NetSerializable]
    public sealed partial class TookPhotoMessage : BoundUserInterfaceMessage
    {
        public readonly EntityUid Author;
        public readonly byte[] Data;
        public TookPhotoMessage(EntityUid author, byte[] data)
        {
            Author = author;
            Data = data;
        }
    }

    [Serializable, NetSerializable]
    public class PhotoCameraComponentState : ComponentState
    {
        public readonly bool On;
        public readonly int Radius;
        public readonly int Film;
        public readonly int FilmMax;
        public PhotoCameraComponentState(bool on, int radius, int film, int filmMax) : base(ContentNetIDs.PHOTO_CAMERA)
        {
            On = on;
            Radius = radius;
            Film = film;
            FilmMax = filmMax;
        }
    }

    [Serializable, NetSerializable]
    public enum PhotoUiKey
    {
        Key
    }
}
