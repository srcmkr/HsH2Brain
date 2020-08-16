using HsH2BrainEditor.Services;
using HsH2BrainEditor.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
