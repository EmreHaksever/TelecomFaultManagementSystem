using System.Threading.Tasks;

namespace Telecom.Domain.Interfaces;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync();
}
