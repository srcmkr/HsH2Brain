using HsH2Brain.Dto;
using HsH2BrainEditor.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HsH2BrainEditor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizController : ControllerBase
    {
        [HttpPost]
        public string GetMyQuizzes(LoginDto login)
        {
            var userService = new UserService();
            var currentUser = userService.Login(login.Username, login.Password);
            if (currentUser == null)
                return string.Empty;

            var quizService = new QuestionService(currentUser.Id);
            return JsonConvert.SerializeObject(quizService.Load());
        }
    }
}
