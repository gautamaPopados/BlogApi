﻿using System.ComponentModel.DataAnnotations;
using WebApplication1.Data.Entities;
using WebApplication1.Validators;

namespace WebApplication1.Data.DTO.Post
{
    public class PostDto
    {
        public Guid id { get; set; }
        public DateTime createTime { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public int readingTime { get; set; }
        public string? image { get; set; }

        public Guid authorId { get; set; }
        public string author { get; set; }
        public Guid? communityId { get; set; }
        public string? communityName { get; set; }
        public Guid? addressId { get; set; }
        public int likes { get; set; }
        public bool hasLike { get; set; }
        public int commentsCount { get; set; }
        public List<TagDto> tags { get; set; }
    }
}
