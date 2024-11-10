using AutoMapper;
using KPMGTask.Dtos;
using KPMGTask.EntityFrameworkCore;
using KPMGTask.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace KPMGTask.Services.AuthenticationServices
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly JWT _jwt;
        private readonly AppDBContextContext _context;
        private readonly IMapper _mapper;
        private IConfiguration _configuration;
        private readonly SignInManager<User> _signInManager;


        public AuthService(UserManager<User> userManager,
            RoleManager<Role> roleManager,
            IOptions<JWT> jwt,
            IHttpContextAccessor httpContextAccessor,
            AppDBContextContext context,
            IMapper mapper,
            IConfiguration configuration,
            SignInManager<User> signInManager
            )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwt = jwt.Value;
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
            _signInManager = signInManager;

        }

        public async Task<RegistrationDTO> RegisterAsync(RegisterDto model)
        {

            try
            {

                if (await _userManager.FindByEmailAsync(model.Email) is not null)
                    return new RegistrationDTO { Message = "Email is already registered!" };

                if (await _userManager.FindByNameAsync(model.UserName) is not null)
                    return new RegistrationDTO { Message = "Username is already registered!" };

                var user = new User
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Status = "In Active",
                    PhoneNumber = model.PhoneNumber,
                    CreationDate=DateTime.Now

                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (!result.Succeeded)
                {
                    var errors = string.Empty;

                    foreach (var error in result.Errors)
                        errors += $"{error.Description},";

                    return new RegistrationDTO { Message = errors };
                }


                await _userManager.AddToRoleAsync(user, model.RoleName);
                await _context.FinancialAccount.AddAsync(
                    
                    new FinancialAccount
                    {
                        BalanceAmount=20000,
                        CreationDateTime=DateTime.Now,
                        UserId= user.Id,
                    });
               await _context.SaveChangesAsync();

                return new RegistrationDTO
                {
                    Email = user.Email,
                    //  ExpiresOn = jwtSecurityToken.ValidTo,
                    IsAuthenticated = true,
                    Roles = new List<string> { model.RoleName },
                    // AccessToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                    UserName = user.UserName,
                    // RefreshToken = refreshToken.Token,
                    // RefreshTokenExpiration = refreshToken.ExpiresOn
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }


        public async Task<AuthDto> GetTokenAsync(TokenRequestDto model)
        {


            try
            {
                var user = await _context.Users
                    .FirstAsync(x => x.Email.Trim().ToLower().Equals(model.Email.Trim().ToLower()));

                var authModel = new AuthDto();
                var roleList = await _userManager.GetRolesAsync(user);

               

               


              



                if (user.Status.Equals("In Active"))
                {
                    return new AuthDto { Message = "Contact your Administrator" };
                }


                authModel.IsAuthenticated = true;
                var jwtSecurityToken = await CreateJwtToken(user);


                authModel.IsAuthenticated = true;
                authModel.AccessToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
                authModel.Email = user.Email;
                authModel.UserName = user.UserName;
                authModel.ExpiresOn = jwtSecurityToken.ValidTo;
                authModel.Roles = roleList.ToList();
                authModel.FirstName = user.FirstName;
                authModel.LastName = user.LastName;
                authModel.PhoneNumber = user.PhoneNumber;
                authModel.Id = user.Id.ToString();




                if (user.RefreshTokens.Any(t => t.IsActive))
                {
                    var activeRefreshToken = user.RefreshTokens.FirstOrDefault(t => t.IsActive);
                    authModel.RefreshToken = activeRefreshToken.Token;
                    authModel.RefreshTokenExpiration = activeRefreshToken.ExpiresOn;
                }
                else
                {
                    var refreshToken = GenerateRefreshToken();
                    authModel.RefreshToken = refreshToken.Token;
                    authModel.RefreshTokenExpiration = refreshToken.ExpiresOn;
                    user.RefreshTokens.Add(refreshToken);
                    await _userManager.UpdateAsync(user);

                }

                if (user.NumberOfLogins != null)
                {
                    user.NumberOfLogins += 1;
                }
                else
                {
                    user.NumberOfLogins = 1;
                }
                user.LastLoginTime = DateTime.Now;


                await _context.SaveChangesAsync();

                return authModel;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public async Task<AuthDto> CheckCredential(TokenRequestDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            var authModel = new AuthDto();

            if (user is not null)
            {
                await _signInManager.SignOutAsync();
                Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(user, model.Password, false, true);
                if (result.IsLockedOut)
                {
                    authModel.Message = "Your account is locked out. Kindly wait for 1 hour and try again";
                    return authModel;
                }
            }

            if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                authModel.IsAuthenticated = false;
                authModel.Message = "Email or Password is incorrect!";
                return authModel;
            }
            else
            {
                authModel.IsAuthenticated = true;
            }

            return authModel;
        }
        public async Task<string> AddRoleAsync(AddRoleDto model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            var role = await _roleManager.RoleExistsAsync(model.RoleName);
            if (user is null || !role)
            {
                return "Invalid user ID or Role";
            }

            if (await _userManager.IsInRoleAsync(user, model.RoleName))
            {
                return "user already assigned to this role";
            }

            var result = await _userManager.AddToRoleAsync(user, model.RoleName);

            return result.Succeeded ? "Role has been assigned successfuly" : "Someting went wrong";

        }

        private async Task<JwtSecurityToken> CreateJwtToken(User user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var financialAccountId = await _context.FinancialAccount.Where(x => x.UserId.Equals(user.Id)).Select(x => x.Id).FirstOrDefaultAsync();



            //Below for getting user permsions

            //1- getting user role name (string)
            var userRoleNames = await _userManager.GetRolesAsync(user);






           


            



            var roleClaims = new List<Claim>();
            foreach (var role in userRoleNames)
            {
                roleClaims.Add(new Claim("roles", role));
            }


            var claims = new[]
            {

                new Claim(JwtRegisteredClaimNames.Sub,user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email,user.Email),
                new Claim("FirstName",user.FirstName),
                new Claim("LastName",user.LastName),
                new Claim("UserId",user.Id.ToString()),
                new Claim("FinancialAccountId",financialAccountId.ToString()),
            }.Union(userClaims)
            .Union(roleClaims);

           

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);


            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwt.DurationInMinutes),
                signingCredentials: signingCredentials
                );
            return jwtSecurityToken;
        }


        public async Task<AuthDto> RefreshTokenAsync(string token)
        {
            var authModel = new AuthDto();
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));

            if (user == null)
            {
                authModel.IsAuthenticated = false;
                authModel.Message = "Invalid Token";
                return authModel;
            }

            var refreshToken = user.RefreshTokens.Single(t => t.Token == token);
            if (!refreshToken.IsActive)
            {
                authModel.IsAuthenticated = false;
                authModel.Message = "Inactive Token";
                return authModel;
            }

            refreshToken.RevokedOn = DateTime.UtcNow;
            var newRefreshToken = GenerateRefreshToken();
            user.RefreshTokens.Add(newRefreshToken);
            await _userManager.UpdateAsync(user);

            var jwtToke = await CreateJwtToken(user);

            authModel.IsAuthenticated = true;
            authModel.AccessToken = new JwtSecurityTokenHandler().WriteToken(jwtToke);
            authModel.Email = user.Email;
            authModel.UserName = user.UserName;
            var roles = await _userManager.GetRolesAsync(user);
            authModel.Roles = roles.ToList();
            authModel.RefreshToken = newRefreshToken.Token;
            authModel.RefreshTokenExpiration = newRefreshToken.ExpiresOn;
            authModel.FirstName = user.FirstName;
            authModel.LastName = user.LastName;
            authModel.PhoneNumber = user.PhoneNumber;
            authModel.Id = user.Id.ToString();

            return authModel;
        }


        public async Task<bool> RevokeTokenAsync(string token)
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));

            if (user == null)
            {
                return false;
            }

            var refreshToken = user.RefreshTokens.Single(t => t.Token == token);
            if (!refreshToken.IsActive)
            {
                return false;
            }

            refreshToken.RevokedOn = DateTime.UtcNow;

            await _userManager.UpdateAsync(user);

            return true;
        }



        private RefreshToken GenerateRefreshToken()
        {
            var randomNumer = new byte[32];

            using var generator = new RNGCryptoServiceProvider();
            generator.GetBytes(randomNumer);

            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomNumer),
                ExpiresOn = DateTime.Now.AddMinutes(180),
                CreatedOn = DateTime.Now
            };
        }






        public async Task<ResetForgotPasswordResultDTO> ForgotPassword(ForgotPasswordRequestDTO request)
        {

            try
            {
                var result = new ResetForgotPasswordResultDTO();


                var user = await _userManager.FindByEmailAsync(request.Email);

                if (user == null)
                {
                    result.Status = false; // Return a success result to prevent email enumeration
                    result.Message = "Please Register";
                    return result;
                }

                if (user?.ForgotPasswordRequested != null && (bool)user.ForgotPasswordRequested &&

                                 user.LatestForgotPasswordRequest != null && IsWithinLast24Hours((DateTime)user.LatestForgotPasswordRequest)

                                 )
                {
                    result.Status = false;
                    result.Message = "You can't reset password more than one time within 24 hours.";
                    return result;
                }




                user.ForgotPasswordRequested = true;
                user.LatestForgotPasswordRequest = DateTime.Now;
                await _context.SaveChangesAsync();




                if (user != null && !user.Status.ToLower().Trim().Equals("active"))
                {
                    result.Status = false; // Return a success result to prevent email enumeration
                    result.Message = "Please Conact your administrator";
                    return result;
                }






                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                var scheme = _configuration["SendGridSetPasswordEmailScheme"];
                var host = _configuration["SendGridSetPasswordEmailHost"];

                var resetUrl = $"{scheme}://{host}/resetpassword?email={Uri.EscapeDataString(user.Email)}&token={Uri.EscapeDataString(token)}";
                var message = $"{user.Email}Reset your password Click this link to reset your password: {resetUrl}";




                //result.Status = true;// added temporary

                result.Message = "check your email please";


                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> SetNewPassword(SetNewPasswordForForgotPasswordDTO request, bool forgotPassword)
        {

            try
            {

                var user = await _userManager.FindByEmailAsync(request.Email);


                var isTokenValid = await _userManager.VerifyUserTokenAsync(
user,
_userManager.Options.Tokens.PasswordResetTokenProvider,
UserManager<User>.ResetPasswordTokenPurpose,
request.Token);

                if (user == null || !isTokenValid)
                {
                    return false;
                }

                var resetResult = await _userManager.ResetPasswordAsync(user, request.Token, request.Password);


                if (forgotPassword)
                {
                    user.ForgotPaswordCompleted = true;
                }



                await _context.SaveChangesAsync();
                return resetResult.Succeeded;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }




        public async Task<ResetForgotPasswordResultDTO> ResetPassword(ResetPasswordRequestDTO request)
        {
            try
            {
                var result = new ResetForgotPasswordResultDTO();
                var user = await _userManager.FindByEmailAsync(request.Email);

                var isItCorrectPassowrd = await _userManager.CheckPasswordAsync(user, request.CurrentPassword);

                if (!isItCorrectPassowrd)
                {
                    return new ResetForgotPasswordResultDTO
                    {
                        Status = false,
                        Message = "Incorrect password"
                    };
                }
                if (user == null)
                {
                    result.Status = false;
                    result.Message = "Contact your Administrator";
                }


                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resetResult = await _userManager.ResetPasswordAsync(user, token, request.Password);


                if (!resetResult.Succeeded)
                {
                    result.Status = resetResult.Succeeded;
                    result.Message = resetResult?.Errors?.ElementAt(0).ToString();
                    return result;
                }

                return new ResetForgotPasswordResultDTO
                {
                    Status = true,
                    Message = "Password has been reseted successfuly"
                };

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    

        private bool IsWithinLast24Hours(DateTime date)
        {
            DateTime now = DateTime.Now;
            DateTime twentyFourHoursAgo = now.AddHours(-24);

            return date >= twentyFourHoursAgo && date <= now;
        }
    }
}
