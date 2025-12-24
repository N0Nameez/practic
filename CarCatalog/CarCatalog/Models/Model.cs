using System;
using System.Collections.Generic;

namespace CarCatalog.Models;

public partial class Model
{
    public int Id { get; set; }

    public int BrandId { get; set; }

    public string Name { get; set; } = null!;

    public int? YearFrom { get; set; }

    public int? YearTo { get; set; }

    public virtual Brand Brand { get; set; } = null!;

    public virtual ICollection<Car> Cars { get; set; } = new List<Car>();
}
