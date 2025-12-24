namespace CarCatalog.Models.Responses
{
    public class UserProfileResponse
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string? Username { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? AvatarUrl { get; set; }
        public string? Phone { get; set; }
        public string Role { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
