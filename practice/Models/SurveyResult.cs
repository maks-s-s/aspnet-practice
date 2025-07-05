namespace practice.Models
{
    public class SurveyResult
    {
        public int Id { get; set; }
        public int SurveyId { get; set; }
        public int QuestionId { get; set; }
        public int AnswerId { get; set; }
        public string UserId { get; set; }
        public Survey Survey { get; set; }
        public Question Question { get; set; }
        public Answer Answer { get; set; }
        public User User { get; set; }
    }
}
