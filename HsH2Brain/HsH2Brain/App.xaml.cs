using HsH2Brain.Services;
using Plugin.Toast;
using System;
using HsH2Brain.Models;
using Xamarin.Forms;

namespace HsH2Brain
{
    public partial class App
    {
        public InMemoryService Services { get; set; }
        public SyncService SyncService { get; set; }
        public ToolbarItem SyncButton { get; set; }

        public App()
        {
            // default
            InitializeComponent();

            // inject IMS from the very top (giving down to, might be replaced with a singleton in the future)
            Services = new InMemoryService();
            SyncService = new SyncService(Services);

            // load data
            Services.Load();

            // init mainpage
            MainPage = new NavigationPage();

            // add login/logout button
            if (Services.DeviceUser == null)
            {
                SyncButton = new ToolbarItem
                {
                    Text = "Login"
                };
            }
            else
            {
                SyncButton = new ToolbarItem
                {
                    Text = "Sync"
                };
            }

            SyncButton.Clicked += StartSync;

            MainPage.ToolbarItems.Add(SyncButton);

            MainPage.Navigation.PushAsync(new MainPage(Services));
        }

        private async void StartSync(object sender, EventArgs e)
        {
            SyncButton.IsEnabled = false;
            SyncButton.Text = "Lade...";

            // call login dialog until login is successful
            if (Services.DeviceUser == null)
            {
                SyncButton.IsEnabled = true;
                SyncButton.Text = "Login";
                await MainPage.Navigation.PushModalAsync(new LoginPage(this));
                return;
            }

            // wait to have it synced
            var success = await SyncService.Sync(Services.DeviceUser.Username, Services.DeviceUser.Password);

            CrossToastPopUp.Current.ShowToastMessage(success ? "Synchronisierung erfolgreich" : "Synchronisierung fehlgeschlagen");

            if (success)
            {
                await MainPage.Navigation.PopAsync();
                await MainPage.Navigation.PushAsync(new MainPage(Services));
            }

            SyncButton.Text = "Sync";
            SyncButton.IsEnabled = true;
        }
    }
}
