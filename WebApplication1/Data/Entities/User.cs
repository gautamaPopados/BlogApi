using System.ComponentModel.DataAnnotations;
using WebApplication1.Validators;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data.Enums;
using Microsoft.AspNetCore.Identity;

namespace WebApplication1.Data.Entities
{
    public class User : IdentityUser<Guid>
    {
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public Gender Gender { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpireTime { get; set; }

        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime CreateTime { get; set; }

        [DataType(DataType.EmailAddress)]
        public override string Email { get; set; }

        public List<Community> Communities { get; set; } = new List<Community>();
        public List<CommunityUser> CommunityUsers { get; set; } = new List<CommunityUser>();
        public List<Post> Posts { get; set; } = new List<Post>();
        public List<PostUserLike> UserLikes { get; set; } = new List<PostUserLike>();

    }
}
