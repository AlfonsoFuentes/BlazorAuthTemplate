using System.Net.NetworkInformation;

namespace Shared.Dtos.Responses
{
    public class TokenResponse
    {
        public string TokenString { get; set; } = string.Empty;
        public DateTime ValidTo { get; set; }
    }
    public class LoginResponse : StatusResponse
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime? Expiration { get; set; }
        public string Name { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
    }
}
