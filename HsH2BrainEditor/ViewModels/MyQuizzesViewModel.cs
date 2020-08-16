using HsH2Brain.Models;
using System.Collections.Generic;

namespace HsH2BrainEditor.ViewModels
{
    public class MyQuizzesViewModel : BaseViewModel
    {
        public List<QuestionSetModel> QuizSets { get; set; }
    }
}
