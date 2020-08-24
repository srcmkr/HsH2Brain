using Acr.UserDialogs;
using HsH2Brain.Dto;
using HsH2Brain.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace HsH2Brain.Services
{
    public class SyncService
    {
        private InMemoryService InMemoryService { get; set; }

        public SyncService(InMemoryService service)
        {
            // store passed InMemoryService
            InMemoryService = service;
        }

        public async Task<bool> Sync()
        {
            try
            {
                // init new HttpClient
                var apiClient = new HttpClient();

                // get content from api
                var loginDto = new LoginDto();

                var userNameTask = await UserDialogs.Instance.PromptAsync("Dein Benutzername:", "Login");
                loginDto.Username = userNameTask.Value;

                var passwordTask = await UserDialogs.Instance.PromptAsync("Dein Kennwort:", "Login", null, null, null, InputType.Password);
                loginDto.Password = passwordTask.Value;

                var content = new StringContent(JsonConvert.SerializeObject(loginDto), System.Text.Encoding.UTF8, "application/json");
                var getAll = await apiClient.PostAsync("https://localhost:5001/api/quiz/", content);

                // only keep on doing if code is 2xx
                if (getAll.IsSuccessStatusCode)
                {
                    // (try) parse content to List of QuestionSetModels
                    var apiSetModels = JsonConvert.DeserializeObject<List<QuestionSetModel>>(getAll.Content.ToString());

                    // iterate through every parsed questionsetmodel
                    foreach (var apiSetModel in apiSetModels)
                    {
                        // only add questionset if it doesn't exist before (to keep progress)
                        if (!InMemoryService.QuestionSets.Exists(c => c.Id == apiSetModel.Id))
                        {
                            InMemoryService.QuestionSets.Add(apiSetModel);
                        } else
                        {
                            // check if there are new questions
                            var storageSet = InMemoryService.QuestionSets.Single(c => c.Id == apiSetModel.Id);
                            foreach(var apiQuestion in apiSetModel.Questions)
                            {
                                // lets add all questions which doesn't exist before
                                if (!storageSet.Questions.Exists(c => c.Id == apiQuestion.Id))
                                {
                                    storageSet.Questions.Add(apiQuestion);
                                }
                            }
                        }
                    }

                    // last: store everything
                    InMemoryService.Save();

                    // this was successful
                    return true;
                }

                // something went wrong, maybe no internet or api down
                return false;
            }
            catch
            {
                // something went *really* wrong, but it doesn't matter tho ¯\_(ツ)_/¯
                return false;
            }
        }

    }
}
