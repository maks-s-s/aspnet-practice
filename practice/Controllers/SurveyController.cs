using Microsoft.AspNetCore.Mvc;
using practice.Models;
using System.Collections.Generic;

namespace practice.Controllers
{
    [Route("surveys")]
    public class SurveyController : Controller
    {
        private static List<Survey> surveys = new List<Survey>
        {
            new Survey
            {
                Id = 1,
                Name = "Опитування 1",
                Topic = "Тема 1",
                Hashtags = new List<string> { "#education", "#survey" },
                Questions = new List<Question>
                {
                    new Question
                    {
                        Text = "Питання 1",
                        Answers = new List<Answer>
                        {
                            new Answer { Text = "Відповідь 1", IsCorrect = true },
                            new Answer { Text = "Відповідь 2", IsCorrect = false }
                        }
                    },
                    new Question
                    {
                        Text = "Питання 2",
                        Answers = new List<Answer>
                        {
                            new Answer { Text = "Відповідь A", IsCorrect = false },
                            new Answer { Text = "Відповідь B", IsCorrect = true }
                        }
                    }
                }
            },
            new Survey
            {
                Id = 2,
                Name = "Опитування 2",
                Topic = "Тема 2",
                Hashtags = new List<string> { "#feedback", "#survey" },
                Questions = new List<Question>
                {
                    new Question
                    {
                        Text = "Питання X",
                        Answers = new List<Answer>
                        {
                            new Answer { Text = "Відповідь X1", IsCorrect = false },
                            new Answer { Text = "Відповідь X2", IsCorrect = true }
                        }
                    }
                }
            }
        };

        // GET /surveys?id=1
        [HttpGet("")]
        public IActionResult Index(int id)
        {
            var survey = surveys.Find(s => s.Id == id);
            if (survey == null)
                return NotFound();

            return View(survey);
        }
    }
}
