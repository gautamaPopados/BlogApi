using System.ComponentModel.DataAnnotations;
using WebApplication1.Validators;

namespace WebApplication1.Data.DTO.Comment
{
    public class UpdateCommentDto
    {
        [MinLength(1)]
        [MaxLength(1000)]
        public string content { get; set; }

    }
}
