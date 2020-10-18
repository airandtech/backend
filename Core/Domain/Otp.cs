using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AirandWebAPI.Core.Domain
{
    public class Otp : BaseEntity
    {
        public string Code { get; set; }
        public bool isUsed { get; set; }
        public int UserId { get; set; }
    }
}
