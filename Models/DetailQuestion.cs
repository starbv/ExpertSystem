namespace ExpertSystem.Models
{
    public class DetailQuestion
    {
        public Answer YesAnswer { get; set; }
        public Answer NoAnswer { get; set; }
        public Question YesQuestion { get; set; }
        public Question NoQuestions { get; set; }
    }
}
