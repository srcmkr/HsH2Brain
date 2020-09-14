using LiteDB;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HsH2Brain.Shared.Models;
using Xamarin.Essentials;

namespace HsH2Brain.Services
{
    public class InMemoryService
    {
        // full set of all questions
        public List<QuestionSetModel> QuestionSets { get; set; }

        // local settings, might be more in the future
        public SettingsModel Settings { get; set; }

        // different from platform to platform
        private string Databasepath { get; set; }

        // ctor
        public InMemoryService()
        {
            // set databasepath if on android
            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                Databasepath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            }

            // set databasepath if on ios
            if (DeviceInfo.Platform == DevicePlatform.iOS)
            {
                var documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                Databasepath = Path.Combine(documentsPath, "..", "Library");
            }

            // add filename to dbpath
            Databasepath = Path.Combine(Databasepath, "hsh2brain.db");
        }

        // load all data from storage to memory
        public void Load()
        {
            using (var db = new LiteDatabase(Databasepath))
            {
                // get questions
                var questionCol = db.GetCollection<QuestionSetModel>("questionsets");
                QuestionSets = questionCol.FindAll().ToList();

                // get settings
                var settingsCol = db.GetCollection<SettingsModel>("settings");
                Settings = settingsCol.FindOne(c => true);
            }

            // fallback: if it's null (because there is no entry yet): create some
            if (QuestionSets == null) QuestionSets = new List<QuestionSetModel>();
            if (Settings == null) Settings = new SettingsModel();
        }

        // like load, but this time store memory to storage
        public void Save()
        {
            using (var db = new LiteDatabase(Databasepath))
            {
                var questionCol = db.GetCollection<QuestionSetModel>("questionsets");
                questionCol.Upsert(QuestionSets);

                var settingsCol = db.GetCollection<SettingsModel>("settings");
                settingsCol.Upsert(Settings);
            }
        }
    }
}
