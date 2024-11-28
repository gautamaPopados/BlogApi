using System.Numerics;
using WebApplication1.Data.Enums;

namespace WebApplication1.Data.Entities
{
    public class AddressElement
    {
        public long id { get; set; }
        public long objectid { get; set; }
        public Guid objectguid { get; set; }
        public string name { get; set; }
        public string typename { get; set; }
        public string level { get; set; }
        public int isactive { get; set; }
    }
}
