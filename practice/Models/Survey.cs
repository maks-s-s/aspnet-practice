namespace practice.Models
{
    public class Survey
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Topic { get; set; }
        public List<string> Hashtags { get; set; }
        public List<Question> Questions { get; set; }
    }
}
