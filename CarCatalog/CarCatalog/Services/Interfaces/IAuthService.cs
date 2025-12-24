using CarCatalog.Models.Requests;
using CarCatalog.Models.Responses;

namespace CarCatalog.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> LoginAsync(LoginRequest request);
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task<UserProfileResponse> GetUserProfileAsync(int userId);
        Task<UserProfileResponse> UpdateUserProfileAsync(int userId, UpdateProfileRequest request);
    }
}
