using RecAll.Infrastructure.Ddd.Domain.SeedWork;

namespace RecAll.Core.List.Domain.AggregateModels.ListAggregate;

/// <summary>
/// List 对应的仓储接口，用于调用对数据库的操作 
/// </summary> <summary>
/// 
/// </summary>
public interface IListRepository : IRepository<List>
{
    List Add(List list);

    Task<List> GetAsync(int listId, string userIdentityGuid);

    // ----- 可以发现需求的增删改查操作只有两个接口（新增和读取），但是实际上有四个操作 -----

    // 修改操作 如何通过上面两个方法进行实现
    // public static async Task Update()
    // {
    //     IListRepository repository;
    //     List list = await repository.GetAsync(1, "123"); // 要修改则先把要修改的 List 读取出来
    //     list.SetDeleted(); // 然后再修改
    //     repository.Add(list); // 最后再保存，但是实际上这里只是把保存这个事情安排上了，并没有实际上执行数据库的写操作 
    //     await repository.UnitOfWork.SaveEntitiesAsync(); // 真正把数据库的写操作执行了
    // }
}
