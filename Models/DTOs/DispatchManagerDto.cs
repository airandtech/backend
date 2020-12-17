using System;

namespace AirandWebAPI.Models.DTOs
{
    public class DispatchManagerDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int CompanyId { get; set; }
        public DateTime DateCreated {get;set;}
        public DateTime LastModified {get;set;}
    }
}