using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ChatBeet.Data;

public interface IApplicationRepository
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    EntityEntry Entry(object entity);
    EntityEntry Attach(object entity);
}