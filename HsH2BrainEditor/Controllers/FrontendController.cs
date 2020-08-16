using HsH2BrainEditor.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HsH2BrainEditor.Controllers
{
    public class FrontendController : Controller
    {
        public UserModel CurrentUser { get; set; }
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FrontendController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;

            try
            {
                var currentUser = _httpContextAccessor.HttpContext.Session.GetString("currentuser");
                if (string.IsNullOrEmpty(currentUser)) return;
                CurrentUser = JsonConvert.DeserializeObject<UserModel>(currentUser);
            } catch
            {
                // that's ok
            }
        }
    }
}
