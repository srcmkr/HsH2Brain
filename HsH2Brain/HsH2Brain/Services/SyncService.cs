using System;
using Acr.UserDialogs;
using HsH2Brain.Dto;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HsH2Brain.Shared.Models;

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

        public async Task<bool> Sync(string username, string password)
        {
            try
            {
                // init new HttpClient
                var apiClient = new HttpClient();

                // prepare user login with parameters (come from localstorage)
                var loginDto = new LoginDto
                {
                    Username = username,
                    Password = password
                };

                // submit to api and get response
                var content = new StringContent(JsonConvert.SerializeObject(loginDto), System.Text.Encoding.UTF8, "application/json");
                var getAll = await apiClient.PostAsync("https://hsh2brain.privacy.ltd/api/", content);

                // only keep on doing if code is 2xx
                if (getAll.IsSuccessStatusCode)
                {
                    // read response
                    var apiContent = await getAll.Content.ReadAsStringAsync();

                    // (try) parse content to List of QuestionSetModels
                    var apiSetModels = JsonConvert.DeserializeObject<List<QuestionSetModel>>(apiContent);

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
            catch (Exception ex)
            {
                // something went *really* wrong, but it doesn't matter tho ¯\_(ツ)_/¯
                Console.WriteLine("Error while fetching data: ", ex);
                return false;
            }
        }

    }
}
