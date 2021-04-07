using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using QASite.Data;
using QASite.Web.Models;

namespace QASite.Web.Controllers
{
    public class QuestionsController : Controller
    {
        private readonly string _connectionString;

        public QuestionsController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConStr");
        }

        [Authorize]
        public IActionResult AskAQuestion()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult Add(Question question, List<string> tags)
        {
            question.Date = DateTime.Now;
            var userRepo = new UserRepository(_connectionString);
            var user = userRepo.GetByEmail(User.Identity.Name);
            question.UserId = user.Id;
            var questionsRepo = new QuestionsRepository(_connectionString);
            questionsRepo.AddQuestion(question, tags);
            return RedirectToAction("ViewQuestion", new { id = question.Id });
        }

        public IActionResult ViewQuestion(int id)
        {
            var questionsRepo = new QuestionsRepository(_connectionString);
            var question = questionsRepo.Get(id);
            var viewModel = new ViewQuestionViewModel
            {
                Question = question
            };
            if (User.Identity.IsAuthenticated)
            {
                var userRepo = new UserRepository(_connectionString);
                viewModel.CurrentUser = userRepo.GetByEmail(User.Identity.Name);
            }
            if (question == null)
            {
                return Redirect("/");
            }

            return View(viewModel);
        }

        [HttpPost]
        [Authorize]
        public IActionResult AddAnswer(int questionId, string text)
        {
            var userRepo = new UserRepository(_connectionString);
            var user = userRepo.GetByEmail(User.Identity.Name);
            var answer = new Answer
            {
                Date = DateTime.Now,
                Text = text,
                QuestionId = questionId,
                UserId = user.Id
            };
            var questionRepo = new QuestionsRepository(_connectionString);
            questionRepo.AddAnswer(answer);
            return RedirectToAction("ViewQuestion", new { id = questionId });
        }

        [HttpPost]
        [Authorize]
        public void AddQuestionLike(int questionId)
        {
            var userRepo = new UserRepository(_connectionString);
            var user = userRepo.GetByEmail(User.Identity.Name);
            var questionRepo = new QuestionsRepository(_connectionString);
            questionRepo.AddQuestionLike(questionId, user.Id);
        }

        public IActionResult GetLikes(int questionId)
        {
            var questionRepo = new QuestionsRepository(_connectionString);
            return Json(new { likes = questionRepo.GetQuestionLikes(questionId) });
        }
    }
}