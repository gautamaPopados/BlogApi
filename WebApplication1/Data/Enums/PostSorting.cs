﻿using System.Text.Json.Serialization;

namespace WebApplication1.Data.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PostSorting
    {
        CreateDesc,
        CreateAsc, 
        LikeAsc, 
        LikeDesc
    }
}
