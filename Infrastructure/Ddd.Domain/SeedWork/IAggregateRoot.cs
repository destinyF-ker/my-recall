namespace RecAll.Infrastructure.Ddd.Domain.SeedWork;

/// <summary>
/// 聚合根接口
/// 什么是聚合根？实际上就是数据库保存数据(操作)的基本单位，一个聚合根可以包含多个实体。(当订单之中的一个商品被删除时，整个订单都发生了变化)
/// 如果一个数据实体要存到数据库之中，并且它有一张独立的表，那么它必须是一个聚合根。
/// </summary>
public interface IAggregateRoot
{

}
