using System;
using System.ComponentModel.DataAnnotations;

namespace CarCatalog.Models.Requests
{
    public class CreateReservationRequest
    {
        [Required]
        public int CarId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        public string? Comment { get; set; }

        [Required]
        [Range(0.01, 1000000)]
        public decimal TotalPrice { get; set; }
    }
}