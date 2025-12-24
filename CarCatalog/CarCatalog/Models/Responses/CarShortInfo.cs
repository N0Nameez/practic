namespace CarCatalog.Models.Responses
{
    public class CarShortInfo
    {
        public int Id { get; set; }
        public string? BrandName { get; set; }
        public string? ModelName { get; set; }
        public int Year { get; set; }
        public string? Color { get; set; }
        public decimal? Price { get; set; }
        public string? PhotoUrl { get; set; }
    }
}
