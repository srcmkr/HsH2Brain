using System;

namespace HsH2Brain.Shared.Models
{
    public class AnswerModel
    {
        // unique identifier
        public Guid Id { get; set; }

        // content of the answer
        public string AnswerText { get; set; }

        // is this the/a correct answer? (useful for single + multiple choice)
        public bool IsCorrect { get; set; }
    }
}
