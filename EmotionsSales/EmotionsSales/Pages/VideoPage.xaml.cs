using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.IO;
using EmotionsSales.Classes;

namespace EmotionsSales.Pages
{
	public partial class VideoPage : ContentPage
	{
		public VideoPage ()
		{
			InitializeComponent ();
		}

        static Stream streamCopy;

        async void btnPic_Clicked(object sender, EventArgs e)
        {
            var useCamara = ((Button)sender).Text.Contains("camera");
            var file = await ImagesService.TakePic(useCamara);
            panelResults.Children.Clear();
            lblResult.Text = "---";

            imgPic.Source = ImageSource.FromStream(() => {
                var stream = file.GetStream();
                streamCopy = new MemoryStream();
                stream.CopyTo(streamCopy);
                stream.Seek(0, SeekOrigin.Begin);
                file.Dispose();
                return stream;
            });
        }

        async void btnAnalyzeEmotions_Clicked(object sender, EventArgs e)
        {
            if (streamCopy != null)
            {
                streamCopy.Seek(0, SeekOrigin.Begin);
                var emotions = await EmotionsService.GetEmotions(streamCopy);

                if (emotions != null)
                {
                    lblResult.Text = "---Emotions Analysis---";
                    DrawResults(emotions);
                }
                else lblResult.Text = "---No face detected---";
            }
            else lblResult.Text = "---No image select---";
        }

        void DrawResults(Dictionary<string, float> emotions)
        {
            panelResults.Children.Clear();

            foreach (var emotion in emotions)
            {
                Label lblEmotion = new Label()
                {
                    Text = emotion.Key,
                    TextColor = Color.Blue,
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
