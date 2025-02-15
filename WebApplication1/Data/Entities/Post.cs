﻿using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Data.Entities
{
    public class Post
    {
        public Guid Id { get; set; }

        [DataType(DataType.Date)]
        public DateTime CreateTime { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int ReadingTime { get; set; }
        public string? Image {  get; set; }

        public Guid AuthorId { get; set; }
        public string Author { get; set; }
        public Guid? CommunityId { get; set; }
        public string? CommunityName { get; set; }
        public Guid? AddressId { get; set; }
        public int Likes { get; set; }
        public bool HasLike {  get; set; }
        public int CommentsCount { get; set; }
        public List<Guid> Tags { get; set; }
        public List<Comment> Comments { get; set; }

        public List<User> Users { get; set; } = new List<User>();
        public List<PostUserLike> UserLikes { get; set; } = new List<PostUserLike>();
    }
}
