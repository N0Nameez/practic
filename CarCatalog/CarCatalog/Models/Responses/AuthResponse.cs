namespace CarCatalog.Models.Responses
{
    public class AuthResponse
    {
        public string Token { get; set; }
        public UserResponse User { get; set; }
    }

    public class UserResponse
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }
}
