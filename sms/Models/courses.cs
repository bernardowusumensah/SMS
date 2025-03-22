namespace sms.Models
{
    public class Courses
    {

        public int courseid { get; set; }

        public string? coursecode { get; set; }

        public int teacherid { get; set; }

        public DateTime? startdate { get; set; }

        public DateTime? finishdate { get; set; }
        public string? coursename { get; set; }
    }
}
