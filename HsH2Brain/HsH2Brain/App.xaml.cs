using HsH2Brain.Services;
using Plugin.Toast;
using System;
using Xamarin.Forms;

namespace HsH2Brain
{
    public partial class App : Application
    {
        public InMemoryService Services { get; set; }
        public SyncService SyncService { get; set; }

        private ToolbarItem SyncButton { get; set; }

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

            // add sync button
            SyncButton = new ToolbarItem
            {
                Text = "Sync"
            };

            SyncButton.Clicked += StartSync;

            MainPage.ToolbarItems.Add(SyncButton);


            MainPage.Navigation.PushAsync(new MainPage(Services));
        }

        private async void StartSync(object sender, EventArgs e)
        {
            SyncButton.IsEnabled = false;
            SyncButton.Text = "Lade...";

            // wait to have it synced
            var success = await SyncService.Sync();

            if (success)
            {
                CrossToastPopUp.Current.ShowToastMessage("Sync komplett");
            }
            else
            {
                // indev: inject predefined set
                SyncService.SyncLocal();
                CrossToastPopUp.Current.ShowToastMessage("Sync fehlgeschlagen");
            }
                

            SyncButton.Text = "Sync";
            SyncButton.IsEnabled = true;
        }
    }
}
