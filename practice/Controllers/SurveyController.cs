using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<SurveyController> _logger;

        public SurveyController(AppDbContext context, UserManager<User> userManager, ILogger<SurveyController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpGet("watch")]
        [Authorize]
        public async Task<IActionResult> Watch(int id)
        {
            _logger.LogInformation("User {User} requests survey watch with id {SurveyId}", User.Identity?.Name ?? "Anonymous", id);
            try
            {
                var survey = await _context.Surveys
                    .Include(s => s.Hashtags)
                    .Include(s => s.Questions)
                        .ThenInclude(q => q.Answers)
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (survey == null)
                {
                    _logger.LogWarning("Survey with id {SurveyId} not found", id);
                    return NotFound();
                }

                return View(survey);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error loading survey with id {SurveyId}", id);
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet("create")]
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            _logger.LogInformation("Admin user {User} accessed survey creation page", User.Identity?.Name ?? "Anonymous");
            return View();
        }

        [HttpPost("create")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Survey survey, List<string> hashtags)
        {
            _logger.LogInformation("Admin user {User} submitting new survey", User.Identity?.Name ?? "Anonymous");
            try
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

                _logger.LogInformation("Survey {SurveyName} created successfully", survey.Name);

                return RedirectToAction("Index", "Home");
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error creating survey {SurveyName}", survey.Name);
                ModelState.AddModelError("", "Помилка при створенні опитування");
                return View(survey);
            }
        }

        [HttpGet("edit")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            _logger.LogInformation("Admin user {User} requests edit page for survey id {SurveyId}", User.Identity?.Name ?? "Anonymous", id);
            try
            {
                var survey = await _context.Surveys
                    .Include(s => s.Questions)
                        .ThenInclude(q => q.Answers)
                    .Include(s => s.Hashtags)
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (survey == null)
                {
                    _logger.LogWarning("Survey with id {SurveyId} not found for editing", id);
                    return NotFound();
                }

                return View(survey);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error loading survey with id {SurveyId} for editing", id);
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost("edit")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Survey survey)
        {
            _logger.LogInformation("Admin user {User} submitting edit for survey id {SurveyId}", User.Identity?.Name ?? "Anonymous", survey.Id);
            try
            {
                var existingSurvey = await _context.Surveys
                    .Include(s => s.Hashtags)
                    .Include(s => s.Questions)
                        .ThenInclude(q => q.Answers)
                    .FirstOrDefaultAsync(s => s.Id == survey.Id);

                if (existingSurvey == null)
                {
                    _logger.LogWarning("Survey with id {SurveyId} not found for update", survey.Id);
                    return NotFound();
                }

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

                _logger.LogInformation("Survey id {SurveyId} updated successfully", survey.Id);

                return RedirectToAction("Index", "Home");
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error updating survey id {SurveyId}", survey.Id);
                ModelState.AddModelError("", "Помилка при оновленні опитування");
                return View(survey);
            }
        }

        [HttpGet("delete")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("Admin user {User} requested deletion of survey id {SurveyId}", User.Identity?.Name ?? "Anonymous", id);
            try
            {
                var survey = await _context.Surveys
                    .Include(s => s.Questions)
                        .ThenInclude(q => q.Answers)
                    .Include(s => s.Hashtags)
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (survey == null)
                {
                    _logger.LogWarning("Survey with id {SurveyId} not found for deletion", id);
                    return NotFound();
                }

                _context.Surveys.Remove(survey);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Survey id {SurveyId} deleted successfully", id);

                return RedirectToAction("Index", "Home");
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error deleting survey id {SurveyId}", id);
                return RedirectToAction("Error", "Home");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("stats")]
        public async Task<IActionResult> Stats(int id)
        {
            _logger.LogInformation("Admin user {User} requested stats for survey id {SurveyId}", User.Identity?.Name ?? "Anonymous", id);
            try
            {
                var survey = await _context.Surveys
                    .Include(s => s.Questions)
                        .ThenInclude(q => q.Answers)
                            .ThenInclude(a => a.SurveyResults)
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (survey == null)
                {
                    _logger.LogWarning("Survey with id {SurveyId} not found for stats", id);
                    return NotFound();
                }

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
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error getting stats for survey id {SurveyId}", id);
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost("submit")]
        [Authorize]
        public async Task<IActionResult> Submit(int surveyId, Dictionary<int, int> answers)
        {
            var userName = User.Identity?.Name ?? "Anonymous";
            _logger.LogInformation("User {User} submitting survey id {SurveyId} answers", userName, surveyId);

            try
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

                _logger.LogInformation("Survey results saved for user {User} and survey id {SurveyId}", userName, surveyId);

                return RedirectToAction("Index", "Home");
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error submitting survey results for user {User} and survey id {SurveyId}", userName, surveyId);
                return RedirectToAction("Error", "Home");
            }
        }
    }
}
