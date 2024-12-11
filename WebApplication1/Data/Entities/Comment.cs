using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Data.Entities
{
    public class Comment
    {
        public Guid Id { get; set; }
        public Guid? ParentId { get; set; }

        [DataType(DataType.Date)]
        public DateTime CreateTime { get; set; }
        public string Content { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ModifiedDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DeleteDate { get; set; } 
        public Guid AuthorId { get; set; }
        public string Author {  get; set; }
        public int SubComments { get; set; } = 0;
        public List<Comment> SubcommentsList { get; set; } = new List<Comment>();
    }
}
