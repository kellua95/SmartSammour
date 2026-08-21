using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSammour.Application.DTOs.Admin
{
    public class CreateAdminUserDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool IsSuperAdmin { get; set; }
    }
}
