using System.ComponentModel.DataAnnotations;

namespace CarCatalog.Models.Requests
{
    public class UpdateCarRequest
    {
        [Required]
        public int ModelId { get; set; }

        [StringLength(17)]
        public string? Vin { get; set; }

        [Range(2000, 2100)]
        public int Year { get; set; }

        [Required]
        [StringLength(50)]
        public string Color { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue)]
        public int Mileage { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; }

        public string? Description { get; set; }

        [Url]
        [StringLength(500)]
        public string? PhotoUrl { get; set; }
    }
}
