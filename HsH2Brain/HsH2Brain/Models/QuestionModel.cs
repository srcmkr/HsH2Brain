using LiteDB;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace HsH2Brain.Models
{
    public class QuestionModel
    {
        // unique identifier
        public Guid Id { get; set; }

        // this is the question
        public string Question { get; set; }

        // serialize to store in a string
        public string AnswersSerialized
        {
            get
            {
                return JsonConvert.SerializeObject(Answers);
            }
            set
            {
                if (value != null && value != "null" && value != string.Empty)
                    Answers = JsonConvert.DeserializeObject<List<AnswerModel>>(value);
                else
                    Answers = new List<AnswerModel>();
            }
        }
        
        [NotMapped, BsonIgnore] public List<AnswerModel> Answers { get; set; }

        // comes from local: bucket to train with and stats
        public int Bucket { get; set; }
        public int CorrectAnswers { get; set; }
        public int WrongAnswers { get; set; }

        public EQuestionType QuestionType { get; set; }

        public QuestionModel()
        {
            // create a new list, so it's never null ☜(ﾟヮﾟ☜)
            Answers = new List<AnswerModel>();
        }
    }

    public enum EQuestionType
    {
        SimpleText,
        SingleChoice,
        MultipleChoice
    }
}
