using practice.Models;
using System.ComponentModel.DataAnnotations;

namespace practice.Models
{
    public class Survey
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        public string Topic { get; set; }

        public List<Hashtag> Hashtags { get; set; } = new();
        public List<Question> Questions { get; set; } = new();
    }

    public class Hashtag
    {
        public int Id { get; set; }
        public string Tag { get; set; }

        public int SurveyId { get; set; }
        public Survey Survey { get; set; }
    }
}
