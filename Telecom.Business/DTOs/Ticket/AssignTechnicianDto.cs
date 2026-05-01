using System;

namespace Telecom.Business.DTOs.Ticket;

public class AssignTechnicianDto
{
    public Guid TicketId { get; set; }
    public Guid TechnicianId { get; set; }
}
