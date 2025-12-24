using CarCatalog.Models.Requests;
using CarCatalog.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CarCatalog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarsController : ControllerBase
    {
        private readonly ICarService _carService;

        public CarsController(ICarService carService)
        {
            _carService = carService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllCars([FromQuery] CarFilters filters)
        {
            var cars = await _carService.GetCarsAsync(filters);
            return Ok(cars);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCar(int id)
        {
            var car = await _carService.GetCarAsync(id);

            if (car == null)
                return NotFound(new { message = "Автомобиль не найден" });

            return Ok(car);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateCar([FromBody] CreateCarRequest request)
        {
            var userId = 1;

            var car = await _carService.CreateCarAsync(request, userId);
            return CreatedAtAction(nameof(GetCar), new { id = car.Id }, car);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteCar(int id)
        {
            var result = await _carService.DeleteCarAsync(id);

            if (!result)
                return NotFound(new { message = "Автомобиль не найден" });

            return NoContent();
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateCar(int id, [FromBody] UpdateCarRequest request)
        {
            var car = await _carService.UpdateCarAsync(id, request);

            if (car == null)
                return NotFound(new { message = "Автомобиль не найден" });

            return Ok(car);
        }
    }
}
