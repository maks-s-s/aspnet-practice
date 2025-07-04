using System.ComponentModel.DataAnnotations;

namespace practice.Models
{
    public class Question
    {
        public int Id { get; set; }

        [Required]
        public string Text { get; set; }

        public int SurveyId { get; set; }
        public Survey Survey { get; set; }

        public List<Answer> Answers { get; set; } = new();
    }
}
