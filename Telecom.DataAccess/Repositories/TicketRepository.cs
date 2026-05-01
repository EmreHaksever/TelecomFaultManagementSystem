using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Telecom.DataAccess.Contexts;
using Telecom.Domain.Entities;
using Telecom.Domain.Interfaces;

namespace Telecom.DataAccess.Repositories;

public class TicketRepository : Repository<Ticket>, ITicketRepository
{
    public TicketRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Ticket>> GetAllWithTechnicianAsync()
    {
        return await _context.Tickets
            .Include(t => t.AssignedTechnician)
            .ToListAsync();
    }

    public async Task<Ticket?> GetByIdWithTechnicianAsync(Guid id)
    {
        return await _context.Tickets
            .Include(t => t.AssignedTechnician)
            .FirstOrDefaultAsync(t => t.Id == id);
    }
}
