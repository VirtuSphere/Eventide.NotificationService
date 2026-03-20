namespace NotificationService.Models
{
    public class UserCreated
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
    }
}
