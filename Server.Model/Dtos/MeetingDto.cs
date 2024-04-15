namespace Server.Model.Dtos
{
    public class MeetingDto
    {
        public int Id { set; get; }
        public string Name { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Description { get; set; }
        public string DocumentUrl { get; set; }
    }
}