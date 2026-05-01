namespace Carvia.Core.Persistence;

public interface IUnitOfWork : IAsyncDisposable
{
  Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
