using Microsoft.AspNetCore.Http;

namespace Infrastructure.IServices
{
    public interface IJwtTokenDecoderService
    {
        IDictionary<string, object> GetTokenPayload(string token);
        IDictionary<string, object> GetTokenPayloadFromHeaders(HttpRequest request);
        string GetTokenFromHeaders(HttpRequest request);
    }
}