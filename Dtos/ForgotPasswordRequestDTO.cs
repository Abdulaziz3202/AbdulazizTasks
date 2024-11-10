namespace KPMGTask.Dtos
{
    public class ForgotPasswordRequestDTO
    {
        public string Email { get; set; }
        public string? Scheme { get; set; }
        public string? Host { get; set; }
    }
}
