using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telecom.Business.DTOs.Ticket;

namespace Telecom.Business.Services.Interfaces;

public interface ITicketService
{
    Task<TicketResponseDto> CreateTicketAsync(CreateTicketDto dto, Guid currentUserId);
    Task<TicketResponseDto?> GetTicketByIdAsync(Guid id);
    Task<IEnumerable<TicketResponseDto>> GetAllTicketsAsync();
    Task<bool> AssignTechnicianAsync(AssignTechnicianDto dto, Guid currentUserId);
    Task<bool> ChangeStatusAsync(ChangeTicketStatusDto dto, Guid currentUserId);
}
