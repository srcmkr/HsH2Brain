using System;
using HsH2Brain.Models;
using Plugin.Toast;
using Xamarin.Forms;

namespace HsH2Brain
{
    public class LoginPage : ContentPage
    {
        private readonly App _app;
        private readonly Entry _usernameInput;
        private readonly Entry _passwordInput;

        public LoginPage(App app)
        {
            _app = app;
            if (_app.Services.DeviceUser != null)
            {
                _app.Services.DeviceUser = null;
                _app.Services.Save();
            }
            else
            {
                var stackLayout = new StackLayout
                {
                    BackgroundColor = Color.LightBlue,
                    Padding = new Thickness(50,50,50,50)
                };

                var frameLayout = new StackLayout();

                var headerLabel = new Label
                {
                    Text = "Login / Registrierung",
                    FontSize = 17
                };
                frameLayout.Children.Add(headerLabel);

                var descriptionlabel = new Label
                {
                    Text = "Bitte melde dich mit deinen Zugangsdaten an:"
                };
                frameLayout.Children.Add(descriptionlabel);

                _usernameInput = new Entry
                {
                    Placeholder = "Benutzername"
                };
                frameLayout.Children.Add(_usernameInput);

                _passwordInput = new Entry
                {
                    Placeholder = "Kennwort",
                    IsPassword = true
                };
                frameLayout.Children.Add(_passwordInput);

                var sendButton = new Button
                {
                    Text = "Login",
                };
                sendButton.Clicked += DoLogin;
                frameLayout.Children.Add(sendButton);

                var credentialsHint = new Label
                {
                    Padding = new Thickness(0,40,0,0),
                    Text = "Hinweis: Nutze unter keinen Umständen deine HsH-Zugangsdaten."
                };
                frameLayout.Children.Add(credentialsHint);

                var contentFrame = new Frame
                {
                    HasShadow = true,
                    CornerRadius = 15,
                    BackgroundColor = Color.White,
                    Margin = new Thickness(5, 5),
                    Content = frameLayout
                };
                stackLayout.Children.Add(contentFrame);

                Content = stackLayout;
            }
        }

        private async void DoLogin(object sender, EventArgs e)
        {
            var success = await _app.SyncService.Sync(_usernameInput.Text, _passwordInput.Text);

            if (success)
            {
                if (_app.Services.DeviceUser == null)
                {
                    _app.Services.DeviceUser = new UserModel
                    {
                        Username = _usernameInput.Text,
                        Password = _passwordInput.Text
                    };
                    _app.Services.Save();
                }

                _app.SyncButton.Text = "Sync";
                await Navigation.PopModalAsync();
                CrossToastPopUp.Current.ShowToastMessage("Login / Registrierung erfolgreich");
            }
            else
            {
                CrossToastPopUp.Current.ShowToastMessage("Benutzerdaten fehlerhaft (Account existiert)");
            }
        }
    }
}
