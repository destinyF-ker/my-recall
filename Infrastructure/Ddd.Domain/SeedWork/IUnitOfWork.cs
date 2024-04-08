namespace RecAll.Infrastructure.Ddd.Domain.SeedWork;

/// <summary>
/// 可以理解为真正的工作单元，实际上进行数据库操作
/// </summary> <summary>
/// 
/// </summary>
public interface IUnitOfWork : IDisposable
{
    // Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default);
}
