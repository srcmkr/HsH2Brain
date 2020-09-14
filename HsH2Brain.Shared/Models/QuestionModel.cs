using System;

namespace HsH2Brain.Shared.Models
{
    public class QuestionModel
    {
        // unique identifier
        public Guid Id { get; set; }

        // this is the question
        public string Question { get; set; }
        
        public string Answer { get; set; }

        // comes from local: bucket to train with and stats
        public int Bucket { get; set; }
        public int CorrectAnswers { get; set; }
        public int WrongAnswers { get; set; }

        public EQuestionType QuestionType { get; set; }
    }

    public enum EQuestionType
    {
        SimpleText,
        SingleChoice,
        MultipleChoice
    }
}
