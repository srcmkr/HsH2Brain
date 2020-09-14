using HsH2Brain.Shared.Models;

namespace HsH2BrainEditor.ViewModels
{
    public class QuizViewModel : BaseViewModel
    {
        public QuestionSetModel Quiz { get; set; }
        public QuestionModel NewQuestion { get; set; }
    }
}
