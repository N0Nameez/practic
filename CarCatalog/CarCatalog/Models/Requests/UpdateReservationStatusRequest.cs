using System.ComponentModel.DataAnnotations;

namespace CarCatalog.Models.Requests
{
    public class UpdateReservationStatusRequest
    {
        [Required]
        public string NewStatus { get; set; } = "pending"; // pending, confirmed, cancelled, completed
    }
}