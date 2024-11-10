using Microsoft.AspNetCore.Mvc;
using System.Text;
using KPMGTask.Middlewares;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using KPMGTask.Helpers;
using Microsoft.Extensions.Options;

namespace KPMGTask.Controllers
{
    public class WebSocketController : Controller
    {
        private readonly Middlewares.WebSocketManager _socketManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly JWT _jwt;
        public WebSocketController(IConfiguration configuration, IOptions<JWT> jwt, Middlewares.WebSocketManager socketManager, IHttpContextAccessor httpContextAccessor)
        {
          
            _jwt = configuration.GetSection("JWT").Get<JWT>();  // Directly read configuration

            _socketManager = socketManager;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet("/ws")]
        public async Task<IActionResult> HandleWebSocketConnection()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                // Manually parse and validate token
                var token = HttpContext.Request.Query["token"];

                var test = _jwt;
                var userId =  ValidateToken(token);  // Token validation function

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User is not authenticated.");
                }

                var socket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                _socketManager.AddSocket(userId, socket);

                await HandleWebSocketCommunication(socket, userId);
            }
            else
            {
                return BadRequest("WebSocket request expected.");
            }

            return Ok();
        }


        private async Task HandleWebSocketCommunication(System.Net.WebSockets.WebSocket socket, string userId)
        {
            var buffer = new byte[1024 * 4];
            var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            while (!result.CloseStatus.HasValue)
            {
                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                Console.WriteLine($"Received message: {message}");

                result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            _socketManager.RemoveSocket(userId);
            await socket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }


        public string ValidateToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return null;

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var test = _jwt;

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = _jwt.Issuer,
                    ValidAudience = _jwt.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key)),
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
                var userId = principal.FindFirst("UserId")?.Value;

                return userId;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Token validation failed: {ex.Message}");
                return null;
            }
        }

    }
}
