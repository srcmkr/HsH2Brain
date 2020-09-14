using HsH2Brain.Services;
using System;
using HsH2Brain.Shared.Models;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace HsH2Brain
{
    public partial class MainPage
    {
        private InMemoryService Services { get; }

        public MainPage(InMemoryService services)
        {
            // setting service to private var
            Services = services;
            InitializeComponent();

            // stack layout for sure as content layout
            var contentLayout = new StackLayout();

            // need action / title bar
            Title = "HsH2Brain";


            // add a new frame for each question set
            foreach(var questionSet in Services.QuestionSets)
            {
                contentLayout.Children.Add(CreateSetCard(questionSet));
            }

            if (Services.QuestionSets.Count == 0)
            {
                contentLayout.Children.Add(CreateEmptyCard());
            }

            // wrap everything in a scroll view and make it pretty ╰(*°▽°*)╯
            Content = new ScrollView
            {
                Content = contentLayout,
                BackgroundColor = Color.LightBlue,
                Padding = 10
            };
        }

        // Creates frame if there are no quiz models on device (needs sync)
        private Frame CreateEmptyCard()
        {
            // frames have shadows \o/
            var contentFrame = new Frame
            {
                HasShadow = true,
                CornerRadius = 15,
                BackgroundColor = Color.White,
                Margin = new Thickness(5, 5)
            };

            // make stack layout horizontal, could use grid, but no need
            var stackLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal
            };

            // card label
            var captionLabel = new Label
            {
                Text = "Keine Sets gefunden 🤔\r\nDrücke Sync oder registriere Dich - kostenlos und unverbindlich",
                TextColor = Color.DarkGray,
                FontSize = 16,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalTextAlignment = TextAlignment.Center,
            };

            stackLayout.Children.Add(captionLabel);

            // take quiz button
            var button = new Button
            {
                Text = "Registrieren",
                CornerRadius = 5,
                BackgroundColor = Color.DarkMagenta,
                TextColor = Color.White
            };

            button.Clicked += (s, e) => { Launcher.TryOpenAsync(new Uri("https://hsh2brain.privacy.ltd/")); };

            // wrap up and return
            stackLayout.Children.Add(button);
            contentFrame.Content = stackLayout;
            return contentFrame;
        }

        // creates Cards for quiz sets
        private Frame CreateSetCard(QuestionSetModel set)
        {
            // frames have shadows \o/
            var contentFrame = new Frame
            {
                HasShadow = true,
                CornerRadius = 15,
                BackgroundColor = Color.White,
                Margin = new Thickness(5, 5)
            };

            // make stack layout horizontal, could use grid, but no need
            var stackLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal
            };

            // quiz title label
            var captionLabel = new Label
            {
                Text = set.Title,
                TextColor = Color.DarkGray,
                FontSize = 16,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalTextAlignment = TextAlignment.Center,
            };

            stackLayout.Children.Add(captionLabel);

            // take quiz button
            var button = new Button
            {
                Text = $"{set.Questions.Count} Fragen",
                CornerRadius = 5,
                BackgroundColor = Color.DarkMagenta,
                TextColor = Color.White
            };

            button.Clicked += (s, e) => { OpenQuestionSet(set.Id); };

            // wrap up and return
            stackLayout.Children.Add(button);
            contentFrame.Content = stackLayout;
            return contentFrame;
        }

        private void OpenQuestionSet(Guid setId)
        {
            Navigation.PushAsync(new QuestionPage(Services, setId));
        }
    }
}
