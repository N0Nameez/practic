using System;
using System.Collections.Generic;

namespace CarCatalog.Models;

public partial class User
{
    public int Id { get; set; }
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string? Role { get; set; }
    public string? Username { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? AvatarUrl { get; set; }
    public string? Phone { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Car> Cars { get; set; } = new List<Car>();
    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}