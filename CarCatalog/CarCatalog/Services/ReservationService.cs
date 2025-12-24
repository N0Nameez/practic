using CarCatalog.Models;
using CarCatalog.Models.Requests;
using CarCatalog.Models.Responses;
using CarCatalog.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CarCatalog.Services
{
    public class ReservationsService : IReservationsService
    {
        private readonly CarCatalogDbContext _context;

        public ReservationsService(CarCatalogDbContext context)
        {
            _context = context;
        }

        public async Task<ReservationResponse> CreateReservationAsync(int userId, CreateReservationRequest request)
        {
            Console.WriteLine($"=== Создание бронирования ===");
            Console.WriteLine($"UserId: {userId}");
            Console.WriteLine($"CarId: {request.CarId}");
            Console.WriteLine($"StartDate: {request.StartDate}");
            Console.WriteLine($"EndDate: {request.EndDate}");
            Console.WriteLine($"TotalPrice: {request.TotalPrice}");
            Console.WriteLine($"Comment: {request.Comment}");

            // Проверяем, существует ли автомобиль
            var car = await _context.Cars
                .Include(c => c.Model)
                .ThenInclude(m => m.Brand)
                .FirstOrDefaultAsync(c => c.Id == request.CarId);

            if (car == null)
            {
                Console.WriteLine("Ошибка: Автомобиль не найден");
                throw new Exception("Автомобиль не найден");
            }

            Console.WriteLine($"Найден автомобиль: {car.Model?.Brand?.Name} {car.Model?.Name}, Статус: {car.Status}");

            // Проверяем, доступен ли автомобиль для бронирования
            if (car.Status != "available")
            {
                Console.WriteLine("Ошибка: Автомобиль недоступен для бронирования");
                throw new Exception("Автомобиль недоступен для бронирования");
            }

            // Проверяем, нет ли пересечений с существующими бронированиями
            var overlappingReservations = await _context.Reservations
                .Where(r => r.CarId == request.CarId &&
                           r.Status != "cancelled" &&
                           ((r.StartDate <= request.StartDate && r.EndDate >= request.StartDate) ||
                            (r.StartDate <= request.EndDate && r.EndDate >= request.EndDate) ||
                            (r.StartDate >= request.StartDate && r.EndDate <= request.EndDate)))
                .AnyAsync();

            if (overlappingReservations)
            {
                Console.WriteLine("Ошибка: Автомобиль уже забронирован на выбранные даты");
                throw new Exception("Автомобиль уже забронирован на выбранные даты");
            }

            // Проверяем даты
            if (request.EndDate <= request.StartDate)
            {
                Console.WriteLine("Ошибка: Дата окончания должна быть позже даты начала");
                throw new Exception("Дата окончания должна быть позже даты начала");
            }

            if ((request.EndDate - request.StartDate).TotalDays > 7)
            {
                Console.WriteLine("Ошибка: Максимальный срок бронирования - 7 дней");
                throw new Exception("Максимальный срок бронирования - 7 дней");
            }

            // Проверяем, что бронирование начинается сегодня
            var today = DateTime.UtcNow.Date;
            if (request.StartDate.Date != today)
            {
                Console.WriteLine($"Ошибка: Дата начала должна быть сегодня ({today}), а выбрана {request.StartDate.Date}");
                throw new Exception("Бронирование должно начинаться сегодня");
            }

            // Проверяем, что бронирование минимум на 1 день
            if ((request.EndDate - request.StartDate).TotalDays < 1)
            {
                Console.WriteLine("Ошибка: Минимальный срок бронирования - 1 день");
                throw new Exception("Минимальный срок бронирования - 1 день");
            }

            // Создаем бронирование
            var reservation = new Reservation
            {
                CarId = request.CarId,
                UserId = userId,
                StartDate = request.StartDate.Date, // Убеждаемся, что храним только дату
                EndDate = request.EndDate.Date,     // Убеждаемся, что храним только дату
                Comment = request.Comment,
                TotalPrice = request.TotalPrice,
                Status = "confirmed",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Обновляем статус автомобиля
            car.Status = "reserved";

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            Console.WriteLine("Бронирование успешно создано");

            return await GetReservationResponseAsync(reservation.Id);
        }

        public async Task<List<ReservationResponse>> GetUserReservationsAsync(int userId)
        {
            var reservations = await _context.Reservations
                .Include(r => r.Car)
                .ThenInclude(c => c.Model)
                .ThenInclude(m => m.Brand)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return reservations.Select(r => MapToResponse(r)).ToList();
        }

        public async Task<ReservationResponse> GetReservationByIdAsync(int id)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Car)
                .ThenInclude(c => c.Model)
                .ThenInclude(m => m.Brand)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reservation == null)
                return null;

            return MapToResponse(reservation);
        }

        public async Task CancelReservationAsync(int reservationId, int userId)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Car)
                .FirstOrDefaultAsync(r => r.Id == reservationId && r.UserId == userId);

            if (reservation == null)
                throw new Exception("Бронь не найдена");

            // Проверяем, можно ли отменить бронь
            var timeUntilStart = reservation.StartDate - DateTime.UtcNow;
            if (timeUntilStart.TotalHours < 24)
                throw new Exception("Отмена возможна не позднее чем за 24 часа до начала бронирования");

            // Обновляем статус брони
            reservation.Status = "cancelled";
            reservation.UpdatedAt = DateTime.UtcNow;

            // Освобождаем автомобиль
            var car = reservation.Car;
            car.Status = "available";

            await _context.SaveChangesAsync();
        }

        public async Task<List<ReservationResponse>> GetCarReservationsAsync(int carId)
        {
            var reservations = await _context.Reservations
                .Include(r => r.User)
                .Where(r => r.CarId == carId && r.Status != "cancelled")
                .OrderBy(r => r.StartDate)
                .ToListAsync();

            return reservations.Select(r => MapToResponse(r)).ToList();
        }

        public async Task<ReservationResponse> UpdateReservationStatusAsync(int id, UpdateReservationStatusRequest statusRequest)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Car)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reservation == null)
                throw new Exception("Бронь не найдена");

            // Обновляем статус (явно указываем параметр statusRequest)
            reservation.Status = statusRequest.NewStatus;
            reservation.UpdatedAt = DateTime.UtcNow;

            // Если бронь отменена или завершена, освобождаем автомобиль
            if (statusRequest.NewStatus == "cancelled" || statusRequest.NewStatus == "completed")
            {
                reservation.Car.Status = "available";
            }

            await _context.SaveChangesAsync();

            return await GetReservationResponseAsync(reservation.Id);
        }

        private async Task<ReservationResponse> GetReservationResponseAsync(int reservationId)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Car)
                .ThenInclude(c => c.Model)
                .ThenInclude(m => m.Brand)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == reservationId);

            if (reservation == null)
                return null;

            return MapToResponse(reservation);
        }

        private ReservationResponse MapToResponse(Reservation reservation)
        {
            return new ReservationResponse
            {
                Id = reservation.Id,
                Car = new CarResponse
                {
                    Id = reservation.Car.Id,
                    BrandName = reservation.Car.Model?.Brand?.Name,
                    ModelName = reservation.Car.Model?.Name,
                    Year = reservation.Car.Year,
                    Color = reservation.Car.Color,
                    Price = reservation.Car.Price,
                    PhotoUrl = reservation.Car.PhotoUrl
                },
                UserId = reservation.UserId,
                UserEmail = reservation.User?.Email,
                StartDate = reservation.StartDate,
                EndDate = reservation.EndDate,
                TotalPrice = reservation.TotalPrice,
                Status = reservation.Status,
                Comment = reservation.Comment,
                CreatedAt = reservation.CreatedAt,
                UpdatedAt = reservation.UpdatedAt
            };
        }
    }
}