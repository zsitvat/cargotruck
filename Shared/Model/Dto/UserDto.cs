﻿using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;

namespace Cargotruck.Shared.Model.Dto
{
    public class UserDto
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string NormalizedUserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string NormalizedEmail { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
