using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Telecom.Business.DTOs.Ticket;
using Telecom.Business.Services.Interfaces;
using Telecom.Domain.Entities;
using Telecom.Domain.Enums;
using Telecom.Domain.Interfaces;

namespace Telecom.Business.Services.Implementations;

public class TicketService : ITicketService
{
    private readonly IRepository<Ticket> _ticketRepository;
    private readonly IRepository<AuditLog> _auditLogRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public TicketService(
        IRepository<Ticket> ticketRepository, 
        IRepository<AuditLog> auditLogRepository, 
        IUnitOfWork unitOfWork, 
        IMapper mapper)
    {
        _ticketRepository = ticketRepository;
        _auditLogRepository = auditLogRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<TicketResponseDto> CreateTicketAsync(CreateTicketDto dto, Guid currentUserId)
    {
        var ticket = _mapper.Map<Ticket>(dto);
        ticket.Status = TicketStatus.Open;
        ticket.SLADueDate = CalculateSLA(dto.Priority);

        await _ticketRepository.AddAsync(ticket);

        var auditLog = new AuditLog
        {
            UserId = currentUserId,
            Ticket = ticket, // EF Core reference assignment is safer before SaveChanges since ID is empty
            ActionType = "TicketCreated",
            Details = $"Ticket '{ticket.Title}' created with priority {ticket.Priority}."
        };
        await _auditLogRepository.AddAsync(auditLog);

        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<TicketResponseDto>(ticket);
    }

    public async Task<TicketResponseDto?> GetTicketByIdAsync(Guid id)
    {
        var ticket = await _ticketRepository.GetByIdAsync(id);
        if (ticket == null) return null;
        
        return _mapper.Map<TicketResponseDto>(ticket);
    }

    public async Task<IEnumerable<TicketResponseDto>> GetAllTicketsAsync(Guid currentUserId, string role)
    {
        var tickets = await _ticketRepository.GetAllAsync();

        // Teknisyen ise sadece kendisine atanan biletleri görsün
        if (role == "Technician")
        {
            var techTickets = tickets.Where(t => t.AssignedTechnicianId == currentUserId);
            return _mapper.Map<IEnumerable<TicketResponseDto>>(techTickets);
        }

        // Admin veya Agent ise tüm biletleri görsün
        return _mapper.Map<IEnumerable<TicketResponseDto>>(tickets);
    }

    public async Task<bool> AssignTechnicianAsync(AssignTechnicianDto dto, Guid currentUserId)
    {
        var ticket = await _ticketRepository.GetByIdAsync(dto.TicketId);
        if (ticket == null) return false;

        ticket.AssignedTechnicianId = dto.TechnicianId;
        ticket.Status = TicketStatus.InProgress; // Atandığında durum otomatik güncellenir
        
        _ticketRepository.Update(ticket);

        var auditLog = new AuditLog
        {
            UserId = currentUserId,
            TicketId = ticket.Id,
            ActionType = "TechnicianAssigned",
            Details = $"Technician {dto.TechnicianId} assigned. Status updated to InProgress."
        };
        await _auditLogRepository.AddAsync(auditLog);

        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ChangeStatusAsync(ChangeTicketStatusDto dto, Guid currentUserId)
    {
        var ticket = await _ticketRepository.GetByIdAsync(dto.TicketId);
        if (ticket == null) return false;

        ticket.Status = dto.Status;
        _ticketRepository.Update(ticket);

        var auditLog = new AuditLog
        {
            UserId = currentUserId,
            TicketId = ticket.Id,
            ActionType = "StatusChanged",
            Details = $"Ticket status changed to {dto.Status}."
        };
        await _auditLogRepository.AddAsync(auditLog);

        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    private DateTime CalculateSLA(Priority priority)
    {
        return priority switch
        {
            Priority.Critical => DateTime.UtcNow.AddHours(4),
            Priority.High => DateTime.UtcNow.AddHours(12),
            Priority.Medium => DateTime.UtcNow.AddHours(24),
            Priority.Low => DateTime.UtcNow.AddHours(48),
            _ => DateTime.UtcNow.AddHours(24) // Default
        };
    }
}
