using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Data.Entities
{
    public class Tag
    {
        public Guid Id { get; set; }
        public DateTime CreateTime { get; set; }
        [MinLength(1)]
        public string Name { get; set; }
    }
}
