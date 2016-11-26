﻿using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Threading.Tasks;

namespace EmotionsSales.Classes
{
    public class MediaService
    {
        public static async Task<MediaFile> TakeVideo(bool useCamera)
        {
            await CrossMedia.Current.Initialize();

            if (useCamera)
            {
                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                {
                    return null;
                }
            }

            var file = useCamera
                ? await CrossMedia.Current.TakeVideoAsync(new StoreVideoOptions
                {
                    Directory = "Cognitive",
                    Name = $"{DateTime.UtcNow}.mp4"
                })
                : await CrossMedia.Current.PickVideoAsync();

            return file;
        }

        public static async Task<MediaFile> TakePic(bool useCamera)
        {
            await CrossMedia.Current.Initialize();

            if (useCamera)
            {
                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                {
                    return null;
                }
            }

            var file = useCamera
                ? await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                {
                    Directory = "Cognitive",
                    Name = $"{DateTime.UtcNow}.jpg"
                })
                : await CrossMedia.Current.PickPhotoAsync();

            return file;
        }
    }
}
