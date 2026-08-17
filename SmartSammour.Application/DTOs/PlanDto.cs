using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSammour.Application.DTOs
{
    public class PlanDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public decimal StartFrom { get; set; }

        public decimal ExtraFee { get; set; }

        public bool IncludesDomainAnalysis { get; set; }

        public bool IncludesHosting { get; set; }

        public bool IncludesDomainRegistration { get; set; }
    }
}
