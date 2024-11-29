namespace WebApplication1.Data.Entities
{
    public class House
    {
        public long id { get; set; }
        public long objectid { get; set; }
        public Guid objectguid { get; set; }
        public string housenum { get; set; }
        public string? addnum1 { get; set; }
        public string? addnum2 { get; set; }
        public int? addtype1 { get; set; }
        public int? addtype2 { get; set; }
        public bool isactive { get; set; }
    }
}
