namespace ExpertSystem.Models
{
    public class Question
    {
        public int ID { get; set; }
        public string Text { get; set; }
        public int? ParentID { get; set; }
        public bool? isTrue { get; set; } = null;
    }
}
