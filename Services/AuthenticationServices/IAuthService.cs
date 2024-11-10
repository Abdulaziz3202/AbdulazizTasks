

using KPMGTask.Dtos;

namespace KPMGTask.Services.AuthenticationServices
{
    public interface IAuthService
    {

        Task<RegistrationDTO> RegisterAsync(RegisterDto model);
        Task<AuthDto> GetTokenAsync(TokenRequestDto model);
        Task<string> AddRoleAsync(AddRoleDto model);
        Task<AuthDto> RefreshTokenAsync(string token);

        Task<bool> RevokeTokenAsync(string token);
        Task<ResetForgotPasswordResultDTO> ForgotPassword(ForgotPasswordRequestDTO request);
        Task<ResetForgotPasswordResultDTO> ResetPassword(ResetPasswordRequestDTO request);
        Task<bool> SetNewPassword(SetNewPasswordForForgotPasswordDTO request, bool forgotPassword);


        Task<AuthDto> CheckCredential(TokenRequestDto model);




    }
}
