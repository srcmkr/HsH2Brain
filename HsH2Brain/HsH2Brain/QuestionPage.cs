using HsH2Brain.Models;
using HsH2Brain.Services;
using Plugin.Toast;
using System;
using System.Linq;

using Xamarin.Forms;

namespace HsH2Brain
{
    public class QuestionPage : ContentPage
    {
        private InMemoryService Services { get; set; }
        private QuestionModel CurrentQuestion { get; set; }
        private QuestionSetModel CurrentQuestionSet { get; set; }

        // this will change for sure
        private Label TextLabel { get; set; }
        private Label WrongLabel { get; set; }
        private Label CorrectLabel { get; set; }
        private Label BucketLabel { get; set; }
        private bool BackSideShown { get; set; }

        public QuestionPage(InMemoryService services, Guid setId)
        {
            // Load InMemoryService and check if questionset is valid
            Services = services;
            CurrentQuestionSet = Services.QuestionSets.SingleOrDefault(c => c.Id == setId);
            if (CurrentQuestionSet == null || CurrentQuestionSet.Questions == null || CurrentQuestionSet.Questions.Count < 1)
            {
                CrossToastPopUp.Current.ShowToastMessage("Das ausgewählte Set ist ungültig");
                Navigation.PopAsync();
            }

            // let's pick the first question, doesn't matter which one, but should be in the very first filled bucket
            CurrentQuestion = CurrentQuestionSet.Questions.OrderBy(c => c.Bucket).ThenBy(c => Guid.NewGuid()).First();

            // prepare stacklayout for view
            var stackLayout = new StackLayout
            {
                BackgroundColor = Color.LightGray,
            };

            // Frame with question content // this is flipable
            var questionFrameFront = new Frame
            {
                BackgroundColor = Color.LightYellow,
                VerticalOptions = LayoutOptions.FillAndExpand,
                CornerRadius = 3
            };

            TextLabel = new Label
            {
                Text = CurrentQuestion.Question,
                FontSize = 24
            };

            questionFrameFront.Content = TextLabel;

            // add flash card within scrollview
            stackLayout.Children.Add(
                new ScrollView
                {
                    Content = questionFrameFront,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    Padding = 5
                }
            );

            // add stats bar
            var statsBar = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Padding = 5
            };

            WrongLabel = new Label
            {
                FontSize = 14,
                HorizontalOptions = LayoutOptions.CenterAndExpand
            };

            CorrectLabel = new Label
            {
                FontSize = 14,
                HorizontalOptions = LayoutOptions.CenterAndExpand
            };

            BucketLabel = new Label
            {
                FontSize = 14,
                HorizontalOptions = LayoutOptions.CenterAndExpand
            }; 

            UpdateStatsBar();

            statsBar.Children.Add(WrongLabel);
            statsBar.Children.Add(CorrectLabel);
            statsBar.Children.Add(BucketLabel);
            stackLayout.Children.Add(statsBar);

            // add toolbar
            var bottomMenu = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Padding = 5
            };

            // add a "Richtig"-Button
            var correctButton = new Button
            {
                Text = "Richtig",
                CornerRadius = 5,
                BackgroundColor = Color.ForestGreen,
                TextColor = Color.White,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };
            correctButton.Clicked += HandleCorrect;
            bottomMenu.Children.Add(correctButton);

            // add a "Lösung"-Button
            var solutionButton = new Button
            {
                Text = "Karte drehen",
                CornerRadius = 5,
                BackgroundColor = Color.Blue,
                TextColor = Color.White,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };
            solutionButton.Clicked += HandleSolution;
            bottomMenu.Children.Add(solutionButton);

            // Add a "Falsch"-Button
            var wrongButton = new Button
            {
                Text = "Falsch",
                CornerRadius = 5,
                BackgroundColor = Color.PaleVioletRed,
                TextColor = Color.White,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };
            wrongButton.Clicked += HandleWrong;
            bottomMenu.Children.Add(wrongButton);

            // wrap everything up and set content
            stackLayout.Children.Add(bottomMenu);
            Content = stackLayout;
        }

        private void HandleWrong(object sender, EventArgs e)
        {
            // increase wrong answer counter
            CurrentQuestion.WrongAnswers++;
            // decrease bucket, if not in the very first already
            if (CurrentQuestion.Bucket > 0) CurrentQuestion.Bucket--;

            // Take the next question
            CurrentQuestion = CurrentQuestionSet.Questions.OrderBy(c => c.Bucket).ThenBy(c => Guid.NewGuid()).First();

            // Update textlabel to the (next) question
            TextLabel.Text = CurrentQuestion.Question;

            // update lower stats bar
            UpdateStatsBar();

            // Auto-flip to front
            BackSideShown = false;

            // save progress
            Services.Save();
        }

        private void HandleSolution(object sender, EventArgs e)
        {
            if (BackSideShown)
            {
                BackSideShown = false;
                TextLabel.Text = CurrentQuestion.Question;
            }               
            else
            {
                BackSideShown = true;
                TextLabel.Text = CurrentQuestion.Answer;
            }
                
        }

        private void HandleCorrect(object sender, EventArgs e)
        {
            // increase right answer counter
            CurrentQuestion.CorrectAnswers++;
            // increase bucket, if not in the very last already
            if (CurrentQuestion.Bucket < 5) CurrentQuestion.Bucket++;

            // Take the next question
            CurrentQuestion = CurrentQuestionSet.Questions.OrderBy(c => c.Bucket).ThenBy(c => Guid.NewGuid()).First();

            // Update textlabel to the (next) question
            TextLabel.Text = CurrentQuestion.Question;

            // update lower stats bar
            UpdateStatsBar();

            // Auto-flip to front
            BackSideShown = false;

            // save progress
            Services.Save();
        }

        // Update lower status bar labels
        private void UpdateStatsBar()
        {
            WrongLabel.Text = string.Format("Falsch: {0}x", CurrentQuestion.WrongAnswers);
            CorrectLabel.Text = string.Format("Richtig: {0}x", CurrentQuestion.CorrectAnswers);
            BucketLabel.Text = string.Format("Bucket: {0}", CurrentQuestion.Bucket);
        }
    }
}