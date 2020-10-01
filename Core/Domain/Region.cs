using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AirandWebAPI.Core.Domain
{
    public class Region : BaseEntity
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
    }
}
