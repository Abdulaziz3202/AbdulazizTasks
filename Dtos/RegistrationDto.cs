namespace KPMGTask.Dtos
{
    public class RegistrationDTO
    {

        public string Message { get; set; }
        public string Email { get; set; }
        public bool IsAuthenticated { get; set; }
        public List<string> Roles { get; set; }
        public string UserName { get; set; }


    }
}
