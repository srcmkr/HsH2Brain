using System;
using System.Collections.Generic;
using LiteDB;
using Newtonsoft.Json;

namespace HsH2Brain.Shared.Models
{
    public class QuestionSetModel
    {
        // unique id
        [BsonId] public Guid Id { get; set; }

        // name of the question set e.g. "Mobile Computing"
        public string Title { get; set; }

        // Author of this question set
        public Guid AuthorId { get; set; }

        // store to avoid multiple tables serialized
        [JsonIgnore]
        public string QuestionsSerialized 
        { 
            get => JsonConvert.SerializeObject(Questions);
            set
            {
                if (value != null && value != "null" && value != string.Empty)
                    Questions = JsonConvert.DeserializeObject<List<QuestionModel>>(value);
                else
                    Questions = new List<QuestionModel>();
            }
        }

        [BsonIgnore] public List<QuestionModel> Questions { get; set; }

        public QuestionSetModel()
        {
            Questions = new List<QuestionModel>();
        }
    }
}
