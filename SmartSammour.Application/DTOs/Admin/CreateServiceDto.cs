using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSammour.Application.DTOs.Admin
{
    public class CreateServiceDto
    {
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public decimal BasePrice { get; set; }
    }
}
