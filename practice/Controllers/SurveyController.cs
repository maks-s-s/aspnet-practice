using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using practice.Data;
using practice.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace practice.Controllers
{
    [Route("surveys")]
    public class SurveyController : Controller
    {
        private readonly AppDbContext _context;

        public SurveyController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("watch")]
        public async Task<IActionResult> Watch(int id)
        {
            var survey = await _context.Surveys
                .Include(s => s.Hashtags)
                .Include(s => s.Questions)
                    .ThenInclude(q => q.Answers)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (survey == null)
                return NotFound();

            return View(survey);
        }

        // Форма создания опроса: /surveys/create
        [HttpGet("create")]
        public IActionResult Create()
        {
            return View();
        }

        // Обработка создания опроса (POST): /surveys/create
        [HttpPost("create")]
        public async Task<IActionResult> Create(Survey survey, List<string> hashtags)
        {
            // Инициализируем список, если null
            if (survey.Hashtags == null)
                survey.Hashtags = new List<Hashtag>();

            // Добавляем хештеги из формы
            foreach (var tag in hashtags)
            {
                if (!string.IsNullOrWhiteSpace(tag))
                {
                    survey.Hashtags.Add(new Hashtag { Tag = tag });
                }
            }

            _context.Surveys.Add(survey);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        [HttpGet("edit")]
        public async Task<IActionResult> Edit(int id)
        {
            var survey = await _context.Surveys
                .Include(s => s.Questions)
                    .ThenInclude(q => q.Answers)
                .Include(s => s.Hashtags)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (survey == null)
                return NotFound();

            return View(survey);
        }

        [HttpPost("edit")]
        public async Task<IActionResult> Edit(Survey survey)
        {
            var existingSurvey = await _context.Surveys
                .Include(s => s.Hashtags)
                .Include(s => s.Questions)
                    .ThenInclude(q => q.Answers)
                .FirstOrDefaultAsync(s => s.Id == survey.Id);

            if (existingSurvey == null)
                return NotFound();

            existingSurvey.Name = survey.Name;
            existingSurvey.Topic = survey.Topic;

            // НЕ обновляем хештеги, оставляем как есть

            // Обновляем вопросы и ответы
            existingSurvey.Questions.Clear();
            if (survey.Questions != null)
            {
                foreach (var q in survey.Questions.Where(q => !string.IsNullOrWhiteSpace(q.Text)))
                {
                    var question = new Question
                    {
                        Text = q.Text.Trim(),
                        Answers = new List<Answer>()
                    };

                    if (q.Answers != null)
                    {
                        foreach (var a in q.Answers.Where(a => !string.IsNullOrWhiteSpace(a.Text)))
                        {
                            question.Answers.Add(new Answer { Text = a.Text.Trim() });
                        }
                    }

                    existingSurvey.Questions.Add(question);
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }




        [HttpGet("delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var survey = await _context.Surveys
                .Include(s => s.Questions)
                    .ThenInclude(q => q.Answers)
                .Include(s => s.Hashtags)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (survey == null)
                return NotFound();

            _context.Surveys.Remove(survey);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }
    }
}
