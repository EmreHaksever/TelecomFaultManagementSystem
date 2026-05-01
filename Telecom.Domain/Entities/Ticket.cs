using System;
using Telecom.Domain.Enums;

namespace Telecom.Domain.Entities;

public class Ticket : BaseEntity
{
    public string CustomerNo { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    public Priority Priority { get; set; }
    public TicketStatus Status { get; set; }
    
    public DateTime? SLADueDate { get; set; }
    public string? ResolutionDetail { get; set; }
    
    // Navigation Properties
    public Guid? AssignedTechnicianId { get; set; }
    public AppUser? AssignedTechnician { get; set; }
}
