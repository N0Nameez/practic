namespace CarCatalog.Models.Requests
{
    public class CreateCarRequest
    {
        public int ModelId { get; set; }
        public string Vin { get; set; }
        public int Year { get; set; }
        public string Color { get; set; }
        public decimal Price { get; set; }
        public int Mileage { get; set; }
        public string Status { get; set; } = "available";
        public string Description { get; set; }
        public string PhotoUrl { get; set; }
    }
}
