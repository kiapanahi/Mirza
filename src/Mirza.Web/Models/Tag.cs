namespace Mirza.Web.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public string Value { get; set; }

        public int WorkLogId { get; set; }
        public WorkLog WorkLog { get; set; }
        
        public Tag()
        {
        }

        public Tag(string value)
        {
            Value = value;
        }
    }
}