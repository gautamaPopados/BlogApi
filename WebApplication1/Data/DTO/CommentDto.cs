using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Data.DTO
{
    public class CommentDto
    {
        public Guid id { get; set; }
        public DateTime createTime { get; set; }
        public string content { get; set; }
        public DateTime? modifiedDate { get; set; }
        public DateTime? deleteDate { get; set; }
        public Guid authorId { get; set; }
        public string author { get; set; }
        public int subComments { get; set; }

    }
}
