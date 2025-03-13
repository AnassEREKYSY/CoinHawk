using System.IdentityModel.Tokens.Jwt;
using Infrastructure.IServices;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services
{
    public class JwtTokenDecoderService : IJwtTokenDecoderService
    {
        public JwtTokenDecoderService() { }
        public IDictionary<string, object> GetTokenPayload(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentNullException(nameof(token), "Token cannot be null or empty.");
            }
            
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            return jwtToken.Payload;
        }

        public IDictionary<string, object> GetTokenPayloadFromHeaders(HttpRequest request)
        {
            if (!request.Headers.TryGetValue("Authorization", out var authHeader))
                throw new Exception("Authorization header missing");

            var token = authHeader.ToString();
            if (token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                token = token.Substring("Bearer ".Length).Trim();

            return GetTokenPayload(token);
        }

        public string GetTokenFromHeaders(HttpRequest request)
        {
            if (request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                var token = authHeader.ToString();
                if (token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    return token.Substring("Bearer ".Length).Trim();
                }
                return token;
            }
            return null;
        }
    }
}
