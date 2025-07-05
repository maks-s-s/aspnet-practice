using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using practice.Data;
using practice.Models;
using practice.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace practice.Controllers
{
    [Route("surveys")]
    public class SurveyController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;

        public SurveyController(AppDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet("watch")]
        [Authorize]
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

        [HttpGet("create")]
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost("create")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Survey survey, List<string> hashtags)
        {
            if (survey.Hashtags == null)
                survey.Hashtags = new List<Hashtag>();

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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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

            existingSurvey.Questions.Clear();
            if (survey.Questions != null)
            {
                foreach (var q in survey.Questions)
                {
                    if (!string.IsNullOrWhiteSpace(q.Text))
                    {
                        var question = new Question
                        {
                            Text = q.Text.Trim(),
                            Answers = new List<Answer>()
                        };

                        if (q.Answers != null)
                        {
                            foreach (var a in q.Answers)
                            {
                                if (!string.IsNullOrWhiteSpace(a.Text))
                                {
                                    question.Answers.Add(new Answer { Text = a.Text.Trim() });
                                }
                            }
                        }

                        existingSurvey.Questions.Add(question);
                    }
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        [HttpGet("delete")]
        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
        [HttpGet("stats")]
        public async Task<IActionResult> Stats(int id)
        {
            var survey = await _context.Surveys
                .Include(s => s.Questions)
                    .ThenInclude(q => q.Answers)
                        .ThenInclude(a => a.SurveyResults)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (survey == null)
                return NotFound();

            var statsList = new List<SurveyStatsViewModel>();

            foreach (var question in survey.Questions)
            {
                int totalVotes = 0;
                foreach (var answer in question.Answers)
                    totalVotes += answer.SurveyResults.Count;

                var answerStats = new List<(string AnswerText, double Percentage)>();

                foreach (var answer in question.Answers)
                {
                    int answerVotes = answer.SurveyResults.Count;
                    double percentage = totalVotes > 0 ? System.Math.Round((double)answerVotes / totalVotes * 100, 2) : 0;

                    answerStats.Add((answer.Text, percentage));
                }

                statsList.Add(new SurveyStatsViewModel
                {
                    QuestionText = question.Text,
                    AnswerStats = answerStats
                });
            }

            return View(statsList);
        }

        [HttpPost("submit")]
        [Authorize]
        public async Task<IActionResult> Submit(int surveyId, Dictionary<int, int> answers)
        {
            var user = await _userManager.GetUserAsync(User);

            foreach (var entry in answers)
            {
                _context.SurveyResults.Add(new SurveyResult
                {
                    SurveyId = surveyId,
                    QuestionId = entry.Key,
                    AnswerId = entry.Value,
                    UserId = user.Id
                });
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }
    }
}
