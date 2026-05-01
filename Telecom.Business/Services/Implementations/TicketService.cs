using System;
using System.Collections.Generic;
using System.Linq;
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
    private readonly ITicketRepository _ticketRepository;
    private readonly IRepository<AuditLog> _auditLogRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public TicketService(
        ITicketRepository ticketRepository, 
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
        // 🔴 FIX: Bileti açan Agent kayıt altına alınıyor
        ticket.CreatedByUserId = currentUserId;

        await _ticketRepository.AddAsync(ticket);

        var auditLog = new AuditLog
        {
            UserId = currentUserId,
            Ticket = ticket,
            ActionType = "TicketCreated",
            Details = $"Ticket '{ticket.Title}' created with priority {ticket.Priority} by user {currentUserId}."
        };
        await _auditLogRepository.AddAsync(auditLog);

        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<TicketResponseDto>(ticket);
    }

    public async Task<TicketResponseDto?> GetTicketByIdAsync(Guid id)
    {
        var ticket = await _ticketRepository.GetByIdWithTechnicianAsync(id);
        if (ticket == null) return null;
        var dto = _mapper.Map<TicketResponseDto>(ticket);
        if (ticket.AssignedTechnician != null)
            dto.AssignedTechnicianName = $"{ticket.AssignedTechnician.FirstName} {ticket.AssignedTechnician.LastName}".Trim();
        return dto;
    }

    public async Task<IEnumerable<TicketResponseDto>> GetAllTicketsAsync(Guid currentUserId, string role)
    {
        var tickets = await _ticketRepository.GetAllWithTechnicianAsync();

        // Teknisyen ise sadece kendisine atanan biletleri görsün
        if (role == "Technician")
            tickets = tickets.Where(t => t.AssignedTechnicianId == currentUserId);

        return tickets.Select(t =>
        {
            var dto = _mapper.Map<TicketResponseDto>(t);
            if (t.AssignedTechnician != null)
                dto.AssignedTechnicianName = $"{t.AssignedTechnician.FirstName} {t.AssignedTechnician.LastName}".Trim();
            return dto;
        }).ToList();
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

        // 🟡 FIX: Çözülüş bilet tekrar çözülemez veya geri alınamaz
        if (ticket.Status == TicketStatus.Resolved && dto.Status != TicketStatus.Closed)
            return false;

        ticket.Status = dto.Status;
        if (!string.IsNullOrEmpty(dto.ResolutionDetail))
        {
            ticket.ResolutionDetail = dto.ResolutionDetail;
        }

        _ticketRepository.Update(ticket);

        var auditLog = new AuditLog
        {
            UserId = currentUserId,
            TicketId = ticket.Id,
            ActionType = "StatusChanged",
            Details = $"Ticket status changed to {dto.Status}. Resolution: {dto.ResolutionDetail ?? "N/A"}"
        };
        await _auditLogRepository.AddAsync(auditLog);

        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteTicketAsync(Guid ticketId, Guid currentUserId)
    {
        var ticket = await _ticketRepository.GetByIdAsync(ticketId);
        if (ticket == null) return false;

        var auditLog = new AuditLog
        {
            UserId = currentUserId,
            ActionType = "TicketDeleted",
            Details = $"Ticket '{ticket.Title}' (ID: {ticketId}) deleted by user {currentUserId}."
        };
        await _auditLogRepository.AddAsync(auditLog);

        _ticketRepository.Remove(ticket);
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
