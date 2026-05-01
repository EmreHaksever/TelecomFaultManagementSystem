using System;
using Telecom.Domain.Enums;

namespace Telecom.Business.DTOs.Ticket;

public class TicketResponseDto
{
    public Guid Id { get; set; }
    public string CustomerNo { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Priority Priority { get; set; }
    public TicketStatus Status { get; set; }
    public DateTime? SLADueDate { get; set; }
    // 🔴 FIX: Eksik alanlar eklendi
    public string? ResolutionDetail { get; set; }
    public Guid? AssignedTechnicianId { get; set; }
    public Guid? CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; }
}
