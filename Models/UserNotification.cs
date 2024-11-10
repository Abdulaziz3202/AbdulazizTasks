namespace KPMGTask.Models
{
    public class UserNotification
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public DateTime CreationDateTime { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}
