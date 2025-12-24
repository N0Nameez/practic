using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarCatalog.Models
{
    public class Reservation
    {
        [Key]
        public int Id { get; set; }

        [Column("car_id")]
        public int CarId { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("start_date")]
        public DateTime StartDate { get; set; }

        [Column("end_date")]
        public DateTime EndDate { get; set; }

        [Column("total_price")]
        public decimal TotalPrice { get; set; }

        [Column("status")]
        public string Status { get; set; } = "pending"; // pending, confirmed, cancelled, completed

        [Column("comment")]
        public string? Comment { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Навигационные свойства
        [ForeignKey("CarId")]
        public virtual Car Car { get; set; } = null!;

        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;
    }
}