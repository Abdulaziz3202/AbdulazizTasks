namespace MVCRESTAPI.Services.AuthenticationService
{
    public interface IUserService
    {
        public bool ValidateCredentials(string username, string password);

    }
}
