using System;
using Telecom.Domain.Enums;

namespace Telecom.Business.DTOs.Ticket;

public class ChangeTicketStatusDto
{
    public Guid TicketId { get; set; }
    public TicketStatus Status { get; set; }
}
