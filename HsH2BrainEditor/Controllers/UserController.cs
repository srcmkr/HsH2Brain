using HsH2Brain.Models;
using HsH2BrainEditor.Services;
using HsH2BrainEditor.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;

namespace HsH2BrainEditor.Controllers
{
    public class UserController : FrontendController
    {
        public UserController(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor) { }

        [HttpGet]
        public IActionResult Login()
        {
            var vm = new LoginViewModel { CurrentUser = CurrentUser };
            return View(vm);
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AddQuiz()
        {
            var vm = new QuizViewModel
            {
                Quiz = new QuestionSetModel(),
                CurrentUser = CurrentUser
            };

            return View(vm);
        }

        [HttpPost]
        public IActionResult AddQuiz(QuizViewModel vm)
        {
            if (string.IsNullOrWhiteSpace(vm.Quiz.Title)) return RedirectToAction("AddQuiz");
            var quizService = new QuestionService();
            var quizId = quizService.Insert(vm.Quiz.Title, CurrentUser.Id);
            return RedirectToAction("EditQuiz", "User", new { id = quizId } );
        }

        [HttpGet]
        public IActionResult EditQuiz(Guid id)
        {
            var questionService = new QuestionService();

            var vm = new QuizViewModel
            {
                CurrentUser = CurrentUser,
                Quiz = questionService.Load(CurrentUser.Id, id)
            };

            return View(vm);
        }

        [HttpGet]
        public IActionResult MyQuiz()
        {
            if (CurrentUser == null) return RedirectToAction("Login", "User");
            var quizService = new QuestionService();
            var myQuizzes = quizService.Load(CurrentUser.Id);

            var vm = new MyQuizzesViewModel
            {
                QuizSets = myQuizzes,
                CurrentUser = CurrentUser
            };

            return View(vm);
        }

        [HttpPost]
        public IActionResult Register(LoginViewModel vm)
        {
            var userService = new UserService();
            var user = userService.SelectUser(vm.LoginName);
            if (user != null)
                return View(new LoginViewModel { LoginName = vm.LoginName, Action = ELoginAction.Error });

            userService.Register(vm.LoginName, vm.Password);
            var loginUser = userService.SelectUserObject(vm.LoginName);
            HttpContext.Session.SetString("currentuser", JsonConvert.SerializeObject(loginUser));
            CurrentUser = loginUser;
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel vm)
        {
            var userService = new UserService();
            var user = userService.SelectUser(vm.LoginName);
            if (user == null)
            {
                return View(new LoginViewModel { LoginName = vm.LoginName, Action = ELoginAction.Registration });
            } else
            {
                if (string.IsNullOrEmpty(vm.Password))
                    return View(new LoginViewModel { LoginName = vm.LoginName, Action = ELoginAction.Login });

                var loginUser = userService.Login(vm.LoginName, vm.Password);
                if (loginUser == null)
                    return View(new LoginViewModel { LoginName = vm.LoginName, Action = ELoginAction.Error });

                HttpContext.Session.SetString("currentuser", JsonConvert.SerializeObject(loginUser));
                CurrentUser = loginUser;
                return RedirectToAction("Index", "Home");
            }
        }


    }
}
