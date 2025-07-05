namespace practice.ViewModels
{
    public class SurveyStatsViewModel
    {
        public string QuestionText { get; set; }
        public List<(string AnswerText, double Percentage)> AnswerStats { get; set; }
    }
}
