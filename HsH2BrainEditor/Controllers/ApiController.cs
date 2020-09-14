using System.Collections.Generic;
using HsH2Brain.Dto;
using HsH2Brain.Shared.Models;
using HsH2BrainEditor.Services;
using Microsoft.AspNetCore.Mvc;

namespace HsH2BrainEditor.Controllers
{
    public class ApiController : ControllerBase
    {
        [HttpPost("/api/")]
        public JsonResult Index([FromBody]LoginDto dto)
        {
            if (string.IsNullOrEmpty(dto.Username) || string.IsNullOrEmpty(dto.Password))
                return new JsonResult(new List<QuestionSetModel>());

            var userService = new UserService();

            var currentUser = userService.Login(dto.Username, dto.Password);
            if (currentUser == null) return new JsonResult(new List<QuestionSetModel>());

            var questionService = new QuestionService(currentUser.Id);
            var myQuestions = questionService.Load();

            return new JsonResult(myQuestions);
        }
    }
}
