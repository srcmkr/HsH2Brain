using System.Collections.Generic;
using HsH2Brain.Shared.Models;

namespace HsH2BrainEditor.ViewModels
{
    public class MyQuizzesViewModel : BaseViewModel
    {
        public List<QuestionSetModel> QuizSets { get; set; }
    }
}
