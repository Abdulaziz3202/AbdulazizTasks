using System.Text.Json.Serialization;

namespace KPMGTask.Dtos
{
    public class AuthDto
    {
        public string Message { get; set; }
        public bool IsAuthenticated { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public List<string> Roles { get; set; }

        public string PhoneNumber { get; set; }

        public string AccessToken { get; set; }
        public DateTime ExpiresOn { get; set; }

        [JsonIgnore]//once you return AuthModel this property will be ignored
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiration { get; set; }


        public string Id { get; set; }
        public string ApplicationOrganizationId { get; set; }
        public string ApplicationOrganizationName { get; set; }
        public string initialLoad { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }





    }
}
