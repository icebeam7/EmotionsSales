using Plugin.Media;
using Plugin.Media.Abstractions;
using System.Threading.Tasks;

namespace EmotionsSales.Classes
{
    public class ImagesService
    {
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
                    Name = "test.jpg"
                })
                : await CrossMedia.Current.PickPhotoAsync();

            return file;
        }
    }
}
