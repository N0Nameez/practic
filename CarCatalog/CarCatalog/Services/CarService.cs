using CarCatalog.Models.Requests;
using CarCatalog.Models.Responses;
using CarCatalog.Models;
using CarCatalog.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CarCatalog.Services
{
    public class CarService : ICarService
    {
        private readonly CarCatalogDbContext _context;

        public CarService(CarCatalogDbContext context)
        {
            _context = context;
        }

        public async Task<List<CarResponse>> GetCarsAsync(CarFilters filters)
        {
            var query = _context.Cars
                .Include(c => c.Model)
                .ThenInclude(m => m.Brand)
                .AsQueryable();

            if (filters.BrandId.HasValue)
                query = query.Where(c => c.Model.BrandId == filters.BrandId);

            if (filters.ModelId.HasValue)
                query = query.Where(c => c.ModelId == filters.ModelId);

            if (filters.MinYear.HasValue)
                query = query.Where(c => c.Year >= filters.MinYear);

            if (filters.MaxYear.HasValue)
                query = query.Where(c => c.Year <= filters.MaxYear);

            if (filters.MinPrice.HasValue)
                query = query.Where(c => c.Price >= filters.MinPrice);

            if (filters.MaxPrice.HasValue)
                query = query.Where(c => c.Price <= filters.MaxPrice);

            if (!string.IsNullOrEmpty(filters.Status))
                query = query.Where(c => c.Status == filters.Status);

            if (!string.IsNullOrEmpty(filters.Color))
                query = query.Where(c => c.Color == filters.Color);

            var cars = await query.ToListAsync();

            return cars.Select(c => new CarResponse
            {
                Id = c.Id,
                BrandName = c.Model.Brand.Name,
                ModelName = c.Model.Name,
                Year = c.Year,
                Color = c.Color,
                Price = c.Price,
                Mileage = c.Mileage,
                Status = c.Status,
                PhotoUrl = c.PhotoUrl
            }).ToList();
        }

        public async Task<CarResponse> GetCarAsync(int id)
        {
            var car = await _context.Cars
                .Include(c => c.Model)
                .ThenInclude(m => m.Brand)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (car == null) return null;

            return new CarResponse
            {
                Id = car.Id,
                BrandName = car.Model.Brand.Name,
                ModelName = car.Model.Name,
                Year = car.Year,
                Color = car.Color,
                Vin = car.Vin,
                Price = car.Price,
                Mileage = car.Mileage,
                Status = car.Status,
                Description = car.Description,
                PhotoUrl = car.PhotoUrl,
                CreatedAt = car.CreatedAt
            };
        }

        public async Task<CarResponse> CreateCarAsync(CreateCarRequest request, int userId)
        {
            var car = new Car
            {
                ModelId = request.ModelId,
                Vin = request.Vin,
                Year = request.Year,
                Color = request.Color,
                Price = request.Price,
                Mileage = request.Mileage,
                Status = request.Status,
                Description = request.Description,
                PhotoUrl = request.PhotoUrl,
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Cars.Add(car);
            await _context.SaveChangesAsync();

            return await GetCarAsync(car.Id);
        }

        public async Task<bool> DeleteCarAsync(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car == null) return false;

            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<CarResponse> UpdateCarAsync(int id, UpdateCarRequest request)
        {
            try
            {
                var car = await _context.Cars.FindAsync(id);
                if (car == null) throw new KeyNotFoundException($"Автомобиль с ID {id} не найден.");

                car.ModelId = request.ModelId;
                car.Vin = request.Vin;
                car.Year = request.Year;
                car.Color = request.Color;
                car.Price = request.Price;
                car.Mileage = request.Mileage;
                car.Status = request.Status;
                car.Description = request.Description;
                car.PhotoUrl = request.PhotoUrl;

                await _context.SaveChangesAsync();

                return await GetCarAsync(car.Id);
            }
            catch (DbUpdateException ex)
            {
                var innerMessage = ex.InnerException?.Message ?? ex.Message;
                throw new Exception($"Ошибка сохранения в БД: {innerMessage}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка сохранения в БД: {ex.Message}");
            }
        }
    }
}
