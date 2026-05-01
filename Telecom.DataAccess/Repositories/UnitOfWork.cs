using System.Threading.Tasks;
using Telecom.DataAccess.Contexts;
using Telecom.Domain.Interfaces;

namespace Telecom.DataAccess.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
