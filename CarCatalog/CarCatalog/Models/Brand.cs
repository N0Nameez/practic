using System;
using System.Collections.Generic;

namespace CarCatalog.Models;

public partial class Brand
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Country { get; set; }

    public virtual ICollection<Model> Models { get; set; } = new List<Model>();
}
