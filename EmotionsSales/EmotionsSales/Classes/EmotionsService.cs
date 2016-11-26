using Microsoft.ProjectOxford.Emotion;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Text;

using Microsoft.ProjectOxford.Common;
using Microsoft.ProjectOxford.Common.Contract;
using Microsoft.ProjectOxford.Emotion.Contract;
using Newtonsoft.Json;

namespace EmotionsSales.Classes
{
    public class EmotionsService
    {
        static string key = "062660bb5efa4f02a9d23c9225c5169b";

        public static async Task<Dictionary<string, float>> GetEmotionsPicture(Stream stream)
        {
            EmotionServiceClient client = new EmotionServiceClient(key);
            var emotions = await client.RecognizeAsync(stream);

            if (emotions == null || emotions.Count() == 0)
                return null;

            return emotions[0].Scores.ToRankedList().ToDictionary(x => x.Key, x => x.Value);
        }

        public static async Task<Dictionary<string, float>> GetEmotionsVideo(Stream stream)
        {
            try
            {
                EmotionServiceClient client = new EmotionServiceClient(key);
                VideoEmotionRecognitionOperation videoOperation = await client.RecognizeInVideoAsync(stream);

                VideoOperationResult operationResult = await client.GetOperationResultAsync(videoOperation);
                string ciclo = "";

                while (true)
                {
                    operationResult = await client.GetOperationResultAsync(videoOperation);

                    switch (operationResult.Status)
                    {
                        case VideoOperationStatus.NotStarted: ciclo += "NS"; break;
                        case VideoOperationStatus.Uploading: ciclo += "Upl"; break;
                        case VideoOperationStatus.Running: ciclo += "Run"; break;
                        case VideoOperationStatus.Failed: ciclo += "Fail"; break;
                        case VideoOperationStatus.Succeeded: ciclo += "Succ"; break;
                        default: ciclo += "Def"; break;
                    }

                    ciclo += "_";
                    
                    if (operationResult.Status == VideoOperationStatus.Succeeded || operationResult.Status == VideoOperationStatus.Failed)
                    {
                        break;
                    }

                    Task.Delay(15000).Wait();
                }

                Dictionary<string, float> dictionary = new Dictionary<string, float>();
                Dictionary<string, float> scores = new Dictionary<string, float>();

                if (operationResult.Status == VideoOperationStatus.Succeeded)
                {
                    var info = ((VideoOperationInfoResult<VideoAggregateRecognitionResult>)operationResult).ProcessingResult;
                    int events = 0;

                    if (info.Fragments != null)
                        foreach (var f in info.Fragments)
                        {
                            if (f.Events != null)
                                foreach (var evs in f.Events)
                                {
                                    foreach (var ev in evs)
                                    {
                                        if (ev.WindowMeanScores != null)
                                        {
                                            var meanScores = ev.WindowMeanScores.ToRankedList().ToDictionary(x => x.Key, x => x.Value);
                                            events++;

                                            foreach (var score in meanScores)
                                            {
                                                if (dictionary.ContainsKey(score.Key))
                                                    dictionary[score.Key] += score.Value;
                                                else
                                                    dictionary.Add(score.Key, score.Value);
                                            }
                                        }
                                        else { }
                                    }
                                }
                        }

                    foreach (var emotion in dictionary)
                    {
                        scores.Add(emotion.Key, emotion.Value / events);
                    }
                }

                return scores;
            }
            catch(Exception ex)
            {
                return null;
            }
        }
    }
}
