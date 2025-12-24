using CarCatalog.Models.Requests;
using CarCatalog.Models.Responses;

namespace CarCatalog.Services.Interfaces
{
    public interface IReservationsService
    {
        Task<ReservationResponse> CreateReservationAsync(int userId, CreateReservationRequest request);
        Task<List<ReservationResponse>> GetUserReservationsAsync(int userId);
        Task<ReservationResponse> GetReservationByIdAsync(int id);
        Task CancelReservationAsync(int reservationId, int userId);
        Task<List<ReservationResponse>> GetCarReservationsAsync(int carId);
        Task<ReservationResponse> UpdateReservationStatusAsync(int id, UpdateReservationStatusRequest statusRequest);
    }
}