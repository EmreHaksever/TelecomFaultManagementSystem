using System;
using Telecom.Domain.Enums;

namespace Telecom.Business.DTOs.Ticket;

public class CreateTicketDto
{
    public string CustomerNo { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Priority Priority { get; set; }
}
