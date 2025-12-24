using System;
using System.Collections.Generic;

namespace CarCatalog.Models;

public partial class Car
{
    public int Id { get; set; }

    public int ModelId { get; set; }

    public string? Vin { get; set; }

    public int Year { get; set; }

    public string? Color { get; set; }

    public decimal? Price { get; set; }

    public int? Mileage { get; set; }

    public string? Status { get; set; }

    public string? Description { get; set; }

    public string? PhotoUrl { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual Model Model { get; set; } = null!;

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
