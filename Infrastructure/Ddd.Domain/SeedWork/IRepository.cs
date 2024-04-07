using RecAll.Infrastructure.Ddd.Domain.SeedWork;

namespace RecAll.Infrastructure.Ddd.Domain;

/// <summary>
/// 在这里 Repository 是仓库的意思，是对数据库的操作的封装，是对数据库的操作的基本单位。
/// 同时通过泛型参数 TAggregateRoot 来约束仓库的操作对象，这样就可以对不同的聚合根进行操作。 
/// 实际上就是在内部调用 IUnitOfWork 的方法来实现对数据库的操作。 
/// </summary> <summary>
/// 
/// </summary>
public interface IRepository<TAggregateRoot>
    where TAggregateRoot : IAggregateRoot
{
    IUnitOfWork UnitOfWork { get; }
}
