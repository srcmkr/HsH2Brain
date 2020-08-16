using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using HsH2BrainEditor.ViewModels;

namespace HsH2BrainEditor.Controllers
{
    public class HomeController : FrontendController
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var vm = new IndexViewModel { CurrentUser = CurrentUser };
            return View(vm);
        }
    }
}
