using Microsoft.EntityFrameworkCore;
using RecAll.Core.List.Domain.AggregateModels.ListAggregate;
using RecAll.Infrastructure.Ddd.Domain.SeedWork;

namespace RecAll.Core.List.Infrastructure.Repositories;

/// <summary>
/// 实现了 IListRepository 接口，提供两种功能：<br/>
/// 1. 添加一个错题本<br/>
/// 2. 读取一个错题本<br/>  
/// 保存实际上不归这个类管，而是归 UnitOfWork 管
/// </summary> <summary>
/// 
/// </summary>
public class ListRepository : IListRepository
{
    private readonly ListContext _context;

    public IUnitOfWork UnitOfWork => _context;

    public ListRepository(ListContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    // ----- 实现了两个接口，完全委托给 DbContext -----
    public Domain.AggregateModels.ListAggregate.List Add(
        Domain.AggregateModels.ListAggregate.List list) =>
        _context.Lists.Add(list).Entity;

    public async Task<Domain.AggregateModels.ListAggregate.List> GetAsync(
        int listId, string userIdentityGuid)
    {
        var list =
            await _context.Lists.FirstOrDefaultAsync(p =>
                p.Id == listId && p.UserIdentityGuid == userIdentityGuid &&
                !p.IsDeleted) ?? _context.Lists.Local.FirstOrDefault(p =>
                p.Id == listId && p.UserIdentityGuid == userIdentityGuid &&
                !p.IsDeleted);

        if (list is null)
        {
            return null;
        }

        // 在 List 之中有一个 Type 属性，但是在数据库之中存储的是 typeId，所以在读取出的时候要做一个转换（通过 Reference）
        await _context.Entry(list)
            .Reference(p => p.Type).LoadAsync();
        return list;
        // 实际上完全可以在内存之中直接硬编码 Enumeration 类型，但是为了更加通用，所以还是通过数据库读取（取决于在领域驱动设计之中所做的决策）
    }
}