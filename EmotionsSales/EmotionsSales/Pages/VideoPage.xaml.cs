﻿using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.IO;
using EmotionsSales.Classes;
using System.Linq;
using Plugin.Media.Abstractions;

namespace EmotionsSales.Pages
{
	public partial class VideoPage : ContentPage
	{
		public VideoPage ()
		{
			InitializeComponent ();
		}

        static Stream streamCopy;
        bool isVideo;

        async void btnVideo_Clicked(object sender, EventArgs e)
        {
            var text = ((Button)sender).Text;
            var useCamara = text.Contains("camera");
            isVideo = text.Contains("video");

            var file = isVideo 
                ? await MediaService.TakeVideo(useCamara)
                : await MediaService.TakePic(useCamara);
            panelResults.Children.Clear();
            lblResult.Text = "---";

            if (isVideo)
            {
                PrepareStream(file);
            }
            else
            {
                imgPic.Source = ImageSource.FromStream(() => {
                    var stream = file.GetStream();
                    streamCopy = new MemoryStream();
                    stream.CopyTo(streamCopy);
                    stream.Seek(0, SeekOrigin.Begin);
                    file.Dispose();
                    return stream;
                });
            }
        }

        void PrepareStream(MediaFile file)
        {
            var stream = file.GetStream();
            streamCopy = new MemoryStream();
            stream.CopyTo(streamCopy);
            stream.Seek(0, SeekOrigin.Begin);
            file.Dispose();
        }

        async void btnAnalyzeEmotions_Clicked(object sender, EventArgs e)
        {
            ShowProgress(true);

            if (streamCopy != null)
            {
                streamCopy.Seek(0, SeekOrigin.Begin);

                var emotions = isVideo 
                    ? await EmotionsService.GetEmotionsVideo(streamCopy)
                    : await EmotionsService.GetEmotionsPicture(streamCopy);

                if (emotions != null)
                {
                    lblResult.Text = "---Emotions Analysis---";
                    DrawResults(emotions);
                    GetRecommendation(emotions);
                }
                else lblResult.Text = "---No face detected---";
            }
            else lblResult.Text = "---No image select---";

            ShowProgress(false);
        }

        void ShowProgress(bool show)
        {
            actProgress.IsVisible = show;
            actProgress.IsRunning = show;
            btnCamera.IsEnabled = !show;
            btnGallery.IsEnabled = !show;
            btnAnalyzeEmotions.IsEnabled = !show;
        }

        void GetRecommendation(Dictionary<string, float> emotions)
        {
            var message = "No recommendation";
            var maxValue = emotions.Values.Max();
            var percentage = maxValue.ToString("P2");
            var maxEmotion = emotions.FirstOrDefault(x => x.Value == maxValue).Key;

            switch (maxEmotion)
            {
                case "Happiness":
                    message = $"Customer is {percentage} happy. A purchase is on the way!";
                    break;
                case "Neutral":
                    message = $"Customer is {percentage} neutral. Do your best to convince him/her to buy the product.";
                    break;
                case "Contempt":
                    message = $"Customer shows {percentage} contempt. Well, winning is not possible all the time.";
                    break;
                case "Disgust":
                    message = $"Customer is {percentage} disgusted. How about showing him/her the product benefits?";
                    break;
                case "Surprise":
                    message = $"Customer is {percentage} surprised. The best you can do is to mention interesting facts about the product to close the deal.";
                    break;
                case "Anger":
                    message = $"Customer is {percentage} angry. A careful approach is recommended.";
                    break;
                case "Sadness":
                    message = $"Customer is {percentage} sad. Prepare some jokes, show empathy and do your best!";
                    break;
                case "Fear":
                    message = $"Customer shows {percentage} fear. Maybe he/she doesn't know how to use the product.";
                    break;
            }

            lblResult.Text = message;
        }

        void DrawResults(Dictionary<string, float> emotions)
        {
            panelResults.Children.Clear();

            foreach (var emotion in emotions)
            {
                Label lblEmotion = new Label()
                {
                    Text = emotion.Key,
                    TextColor = Color.Black,
                    WidthRequest = 90
                };

                BoxView box = new BoxView()
                {
                    Color = Color.Lime,
                    WidthRequest = 150 * emotion.Value,
                    HeightRequest = 30,
                    HorizontalOptions = LayoutOptions.StartAndExpand
                };

                Label lblPercentage = new Label()
                {
                    Text = emotion.Value.ToString("P4"),
                    TextColor = Color.Maroon
                };

                StackLayout panel = new StackLayout()
                {
                    Orientation = StackOrientation.Horizontal
                };

                panel.Children.Add(lblEmotion);
                panel.Children.Add(box);
                panel.Children.Add(lblPercentage);

                panelResults.Children.Add(panel);
            }
        }
    }
}
