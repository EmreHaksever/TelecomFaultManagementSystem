using System;
using System.Collections.Generic;

namespace Telecom.Domain.Entities;

public class AppUser : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty; // "Admin", "Agent", "Technician"
    
    // Kullanıcının atandığı ticket'lar
    public ICollection<Ticket> AssignedTickets { get; set; } = new List<Ticket>();
    
    // Kullanıcının gerçekleştirdiği işlemler (AuditLogs)
    public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
}
