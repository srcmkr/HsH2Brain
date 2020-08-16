using HsH2Brain.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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

        public void SyncLocal()
        {
            InMemoryService.QuestionSets = new List<QuestionSetModel>
            {
                new QuestionSetModel
                {
                    Id = Guid.NewGuid(),
                    Questions = new List<QuestionModel>
                    {
                        new QuestionModel
                        {
                            Id = Guid.NewGuid(),
                            Bucket = 0,
                            QuestionType = EQuestionType.SimpleText,
                            Question = "Welche Programmiersprachen sind nativ unter iOS?",
                            Answers = new List<AnswerModel>
                            {
                                new AnswerModel
                                {
                                    Id = Guid.NewGuid(),
                                    AnswerText = "Swift, Objective C",
                                    IsCorrect = true
                                }
                            }
                        },
                        new QuestionModel
                        {
                            Id = Guid.NewGuid(),
                            Bucket = 0,
                            QuestionType = EQuestionType.SimpleText,
                            Question = "Welche technischen Ansätze existieren zur Entwicklung einer App?",
                            Answers = new List<AnswerModel>
                            {
                                new AnswerModel
                                {
                                    Id = Guid.NewGuid(),
                                    AnswerText = "Nativ, Hybrid (Crossplattform), WebApp",
                                    IsCorrect = true
                                }
                            }
                        }
                    },
                    Title = "Mobile Computing"
                }
            };

            InMemoryService.Save();
        }

        public async Task<bool> Sync()
        {
            try
            {
                // init new HttpClient
                var apiClient = new HttpClient();

                // get content from api
                var getAll = await apiClient.GetAsync("https://api.hsh2brain.de/");

                // only keep on doing if code is 2xx
                if (getAll.IsSuccessStatusCode)
                {
                    // (try) parse content to List of QuestionSetModels
                    var content = JsonConvert.DeserializeObject<List<QuestionSetModel>>(getAll.Content.ToString());

                    // iterate through every parsed questionsetmodel
                    foreach (var fromApi in content)
                    {
                        // only add questionset if it doesn't exist before (to keep progress)
                        if (!InMemoryService.QuestionSets.Exists(c => c.Id == fromApi.Id))
                        {
                            InMemoryService.QuestionSets.Add(fromApi);
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
