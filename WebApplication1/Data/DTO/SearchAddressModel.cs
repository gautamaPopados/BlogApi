using WebApplication1.Data.Enums;

namespace WebApplication1.Data.DTO
{
    public class SearchAddressModel
    {
        public long objectId { get; set; }
        public Guid objectGuid { get; set; }
        public string? text { get; set; }
        public GarAddressLevel objectLevel { get; set; }
        public string? objectLevelText { get; set; }
    }
}
