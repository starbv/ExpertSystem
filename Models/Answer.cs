namespace ExpertSystem.Models
{
    public class Answer
    {
        public int ID { get; set; }
        public string Text { get; set; }
        public int? QuestionID { get; set; }
        public bool? isTrue { get; set; }
        public byte[] Image { get; set; }
    }
}
