using System;

namespace DataAccess.Models
{
    public class Player
    {
       
        public string PlayerId { get; set; } = Guid.NewGuid().ToString();
        
        public string Name { get; set; } = string.Empty;
        
        public string Phone { get; set; } = string.Empty;
        
        public string Email { get; set; } = string.Empty;
        
        public bool Active { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
