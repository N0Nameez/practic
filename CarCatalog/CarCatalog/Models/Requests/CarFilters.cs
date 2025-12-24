namespace CarCatalog.Models.Requests
{
    public class CarFilters
    {
        public int? BrandId { get; set; }
        public int? ModelId { get; set; }
        public int? MinYear { get; set; }
        public int? MaxYear { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? Status { get; set; }
        public string? Color { get; set; }
    }
}
