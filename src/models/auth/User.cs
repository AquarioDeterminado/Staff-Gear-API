
using System;

namespace API.src.auth
{
    public class User
    {
        public int UserId { get; set; }                     // PK auto-increment
        public string Username { get; set; } = null!;       // ex.: email (único)
        public string PasswordHash { get; set; } = null!;   // BCrypt
        public string Role { get; set; } = "Employee";      // 'HR' | 'Employee' | etc.
        public bool IsActive { get; set; } = true;          // estado da conta
        public int? EmployeeId { get; set; }                // FK opcional para HR.Employee (BusinessEntityID)

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
       public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    }

    
}
