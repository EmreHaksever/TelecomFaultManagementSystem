using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telecom.Domain.Entities;

namespace Telecom.Domain.Interfaces;

public interface ITicketRepository : IRepository<Ticket>
{
    /// <summary>
    /// Teknisyen (AssignedTechnician) bilgisi dahil tüm biletleri getirir.
    /// </summary>
    Task<IEnumerable<Ticket>> GetAllWithTechnicianAsync();
    
    /// <summary>
    /// Teknisyen bilgisi dahil tek bir bileti getirir.
    /// </summary>
    Task<Ticket?> GetByIdWithTechnicianAsync(Guid id);
}
