using System;

namespace Telecom.UI.Models;

public class TicketViewModel
{
    public Guid Id { get; set; }
    public string CustomerNo { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Priority { get; set; }
    public int Status { get; set; }
    public DateTime? SLADueDate { get; set; }
    public string? ResolutionDetail { get; set; }
    public Guid? CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; }
}
