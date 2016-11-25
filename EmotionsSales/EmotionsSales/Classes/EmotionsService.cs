using Microsoft.ProjectOxford.Emotion;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace EmotionsSales.Classes
{
    public class EmotionsService
    {
        static string key = "062660bb5efa4f02a9d23c9225c5169b";

        public static async Task<Dictionary<string, float>> GetEmotions(Stream stream)
        {
            EmotionServiceClient client = new EmotionServiceClient(key);
            var emotions = await client.RecognizeAsync(stream);

            if (emotions == null || emotions.Count() == 0)
                return null;

            return emotions[0].Scores.ToRankedList().ToDictionary(x => x.Key, x => x.Value);
        }
    }
}
