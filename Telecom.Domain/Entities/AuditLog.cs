using System;

namespace Telecom.Domain.Entities;

public class AuditLog : BaseEntity
{
    public Guid UserId { get; set; } // İşlemi Yapan
    public string ActionType { get; set; } = string.Empty; // İşlem Tipi (Örn: "TicketCreated", "StatusChanged")
    
    public Guid? TicketId { get; set; } // Etkilenen Ticket (nullable olabilir, her log ticket'a bağlı olmayabilir)
    
    public string Details { get; set; } = string.Empty; // Detay
    
    // Navigation Properties
    public AppUser? User { get; set; }
    public Ticket? Ticket { get; set; }
}
