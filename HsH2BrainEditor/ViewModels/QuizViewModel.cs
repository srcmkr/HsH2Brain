using HsH2Brain.Models;

namespace HsH2BrainEditor.ViewModels
{
    public class QuizViewModel : BaseViewModel
    {
        public QuestionSetModel Quiz { get; set; }
        public QuestionModel NewQuestion { get; set; }
    }
}
