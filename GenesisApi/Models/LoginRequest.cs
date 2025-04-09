namespace GenesisApi.Models
{
    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty; // Or use Token
        public string Password { get; set; } = string.Empty; // Optional if Token is used
        public string Language { get; set; } = "de";
    }
}
