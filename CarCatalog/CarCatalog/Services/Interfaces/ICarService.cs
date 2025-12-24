using CarCatalog.Models.Responses;
using CarCatalog.Models.Requests;

namespace CarCatalog.Services.Interfaces
{
    public interface ICarService
    {
        Task<List<CarResponse>> GetCarsAsync(CarFilters filters);
        Task<CarResponse> GetCarAsync(int id);
        Task<CarResponse> CreateCarAsync(CreateCarRequest request, int userId);
        Task<bool> DeleteCarAsync(int id);
        Task<CarResponse> UpdateCarAsync(int id, UpdateCarRequest request);
    }
}
