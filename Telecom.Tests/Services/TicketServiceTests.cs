using System;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using Telecom.Business.DTOs.Ticket;
using Telecom.Business.Mappings;
using Telecom.Business.Services.Implementations;
using Telecom.Domain.Entities;
using Telecom.Domain.Enums;
using Telecom.Domain.Interfaces;
using Xunit;

namespace Telecom.Tests.Services;

public class TicketServiceTests
{
    private readonly Mock<IRepository<Ticket>> _mockTicketRepo;
    private readonly Mock<IRepository<AuditLog>> _mockAuditLogRepo;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly IMapper _mapper;
    private readonly TicketService _ticketService;

    public TicketServiceTests()
    {
        _mockTicketRepo = new Mock<IRepository<Ticket>>();
        _mockAuditLogRepo = new Mock<IRepository<AuditLog>>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();

        // AutoMapper gerçek konfigürasyonu kullansın
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new MappingProfile());
        });
        _mapper = mapperConfig.CreateMapper();

        _ticketService = new TicketService(
            _mockTicketRepo.Object,
            _mockAuditLogRepo.Object,
            _mockUnitOfWork.Object,
            _mapper
        );
    }

    [Fact]
    public async Task CreateTicketAsync_Should_Calculate_SLA_Correctly_And_Add_AuditLog()
    {
        // Arrange
        var currentUserId = Guid.NewGuid();
        var dto = new CreateTicketDto
        {
            CustomerNo = "TEST-123",
            Title = "Test Ticket",
            Description = "Description",
            Priority = Priority.Critical // Critical means +4 hours SLA
        };

        // Act
        var result = await _ticketService.CreateTicketAsync(dto, currentUserId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(TicketStatus.Open, result.Status);
        
        // SLA Due Date test (Critical -> +4 hours from now)
        Assert.NotNull(result.SLADueDate);
        var expectedSla = DateTime.UtcNow.AddHours(4);
        var difference = Math.Abs((result.SLADueDate.Value - expectedSla).TotalMinutes);
        Assert.True(difference < 1, "SLA Due Date Critical priority için yanlış hesaplandı!"); // 1 dakikalık tolerans
        
        // Repositories called?
        _mockTicketRepo.Verify(r => r.AddAsync(It.IsAny<Ticket>()), Times.Once);
        _mockAuditLogRepo.Verify(r => r.AddAsync(It.Is<AuditLog>(a => a.ActionType == "TicketCreated" && a.UserId == currentUserId)), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task AssignTechnicianAsync_Should_Return_False_When_Ticket_Not_Found()
    {
        // Arrange
        var currentUserId = Guid.NewGuid();
        var dto = new AssignTechnicianDto
        {
            TicketId = Guid.NewGuid(),
            TechnicianId = Guid.NewGuid()
        };

        _mockTicketRepo.Setup(r => r.GetByIdAsync(dto.TicketId)).ReturnsAsync((Ticket?)null);

        // Act
        var result = await _ticketService.AssignTechnicianAsync(dto, currentUserId);

        // Assert
        Assert.False(result);
        _mockTicketRepo.Verify(r => r.Update(It.IsAny<Ticket>()), Times.Never);
        _mockAuditLogRepo.Verify(r => r.AddAsync(It.IsAny<AuditLog>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task AssignTechnicianAsync_Should_Assign_And_Add_AuditLog()
    {
        // Arrange
        var currentUserId = Guid.NewGuid();
        var dto = new AssignTechnicianDto
        {
            TicketId = Guid.NewGuid(),
            TechnicianId = Guid.NewGuid()
        };

        var ticket = new Ticket
        {
            Id = dto.TicketId,
            Title = "Existing Ticket",
            Status = TicketStatus.Open
        };

        _mockTicketRepo.Setup(r => r.GetByIdAsync(dto.TicketId)).ReturnsAsync(ticket);

        // Act
        var result = await _ticketService.AssignTechnicianAsync(dto, currentUserId);

        // Assert
        Assert.True(result);
        Assert.Equal(dto.TechnicianId, ticket.AssignedTechnicianId);

        _mockTicketRepo.Verify(r => r.Update(ticket), Times.Once);
        _mockAuditLogRepo.Verify(r => r.AddAsync(It.Is<AuditLog>(a => a.ActionType == "TechnicianAssigned" && a.UserId == currentUserId)), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }
}
