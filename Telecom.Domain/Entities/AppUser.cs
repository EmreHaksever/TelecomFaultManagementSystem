using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Telecom.Domain.Entities;

public class AppUser : IdentityUser<Guid>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    // Email, Id, UserName vb. özellikler IdentityUser'dan miras alınır.
    
    // Kullanıcının atandığı ticket'lar
    public ICollection<Ticket> AssignedTickets { get; set; } = new List<Ticket>();
    
    // Kullanıcının gerçekleştirdiği işlemler (AuditLogs)
    public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
}
