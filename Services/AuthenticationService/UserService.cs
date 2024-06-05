namespace MVCRESTAPI.Services.AuthenticationService
{
    public class UserService:IUserService
    {
        private IConfiguration _configuration;
        public UserService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public bool ValidateCredentials(string username, string password)
        {
            var correctUsername = _configuration["Username"];
            var correctPassword = _configuration["Password"];

            return correctUsername.Equals(username, StringComparison.CurrentCultureIgnoreCase) && correctPassword.Equals(password);
        }
    }
}
