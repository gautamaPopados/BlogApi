﻿namespace WebApplication1.Data.DTO.Auth
{
    public class AuthResponseDTO
    {
        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }
    }
}
