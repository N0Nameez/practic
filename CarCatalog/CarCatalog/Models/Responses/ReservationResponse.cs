using System;

namespace CarCatalog.Models.Responses
{
    public class ReservationResponse
    {
        public int Id { get; set; }
        public CarResponse Car { get; set; }
        public int UserId { get; set; }
        public string UserEmail { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal? TotalPrice { get; set; }
        public string Status { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}