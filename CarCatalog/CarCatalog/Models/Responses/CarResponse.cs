namespace CarCatalog.Models.Responses
{
    public class CarResponse
    {
        public int Id { get; set; }
        public string BrandName { get; set; }
        public string ModelName { get; set; }
        public string? Vin { get; set; }
        public int? Year { get; set; }
        public string Color { get; set; }
        public decimal? Price { get; set; }
        public int? Mileage { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public string PhotoUrl { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
