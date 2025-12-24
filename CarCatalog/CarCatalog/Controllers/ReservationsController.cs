using System;
using System.Security.Claims;
using System.Threading.Tasks;
using CarCatalog.Models.Requests;
using CarCatalog.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarCatalog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReservationsController : ControllerBase
    {
        private readonly IReservationsService _reservationsService;

        public ReservationsController(IReservationsService reservationsService)
        {
            _reservationsService = reservationsService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateReservation([FromBody] CreateReservationRequest request)
        {
            try
            {
                Console.WriteLine($"=== Получен запрос на бронирование ===");
                Console.WriteLine($"CarId: {request.CarId}");
                Console.WriteLine($"StartDate: {request.StartDate}");
                Console.WriteLine($"EndDate: {request.EndDate}");
                Console.WriteLine($"TotalPrice: {request.TotalPrice}");

                var userId = GetCurrentUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Пользователь не авторизован" });

                Console.WriteLine($"UserId из токена: {userId}");

                var result = await _reservationsService.CreateReservationAsync(userId.Value, request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при создании бронирования: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMyReservations()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized(new { message = "Пользователь не авторизован" });

            var reservations = await _reservationsService.GetUserReservationsAsync(userId.Value);
            return Ok(reservations);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReservation(int id)
        {
            var reservation = await _reservationsService.GetReservationByIdAsync(id);
            if (reservation == null)
                return NotFound(new { message = "Бронь не найдена" });

            return Ok(reservation);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelReservation(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Пользователь не авторизован" });

                await _reservationsService.CancelReservationAsync(id, userId.Value);
                return Ok(new { message = "Бронь успешно отменена" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("car/{carId}")]
        public async Task<IActionResult> GetCarReservations(int carId)
        {
            var reservations = await _reservationsService.GetCarReservationsAsync(carId);
            return Ok(reservations);
        }

        [Authorize(Roles = "admin")]
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateReservationStatus(int id, [FromBody] UpdateReservationStatusRequest statusRequest)
        {
            try
            {
                var result = await _reservationsService.UpdateReservationStatusAsync(id, statusRequest);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("userId") ?? User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                return null;

            return userId;
        }
    }
}