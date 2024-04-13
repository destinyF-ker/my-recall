using RecAll.Core.List.Domain.Exceptions;
using RecAll.Infrastructure.Ddd.Domain.SeedWork;

namespace RecAll.Core.List.Domain.AggregateModels.ItemAggregate;

/// <summary>
/// 题目数据的主体并不在这里保存，只是起到一个关联的作用
/// 将题目的头部（ID）保存在 List.Api 之中，而题目的主体保存在自己的 Contrib 子目录之中
/// 这样就可以根据不同的需求，将题目的主体保存在不同的微服务之中，增强了系统的灵活性
/// 这里实际上就有一个问题：谁来保存题目的主体数据？
/// 程序流程应该如下： 
/// 1. 用户在 List.Api 之中创建题目，将题目的头部数据保存在自己的数据库表之中 
/// 2. List.Api 将数据的主体转发给 Contrib.Api 之中，Contrib.Api 保存数据的主体
/// 3. 在 Contrib.Api 将数据保存完之后，返回数据的主键（主键）给 List.Api
/// 4. List.Api 将数据的主键（主键）保存在自己的数据库表之中
/// </summary> <summary>
/// 
/// </summary>
public class Item : Entity, IAggregateRoot
{
    // 数据保存在哪个微服务之中
    private int _typeId;

    public ListType Type { get; private set; }

    // 数据保存在哪个集合之中
    private int _setId;

    public int SetId => _setId;

    // 数据主体对应的数据的主键（主键）
    private string _contribId;

    public string ContribId => _contribId;

    private string _userIdentityGuid;

    public string UserIdentityGuid => _userIdentityGuid;

    private bool _isDeleted;

    public bool IsDeleted => _isDeleted;

    private Item() { }

    public Item(int typeId, int setId, string contribId,
        string userIdentityGuid) : this()
    {
        _typeId = typeId;
        _setId = setId;
        _contribId = contribId;
        _userIdentityGuid = userIdentityGuid;

        var itemCreatedDomainEvent = new ItemCreatedDomainEvent(this);
        AddDomainEvent(itemCreatedDomainEvent);
    }

    public void SetSetId(int setId)
    {
        if (_isDeleted)
        {
            ThrowDeletedException();
        }

        _setId = setId;
    }

    public void SetDeleted()
    {
        if (_isDeleted)
        {
            ThrowDeletedException();
        }

        _isDeleted = true;

        var itemDeletedDomainEvent = new ItemDeletedDomainEvent(this);
        AddDomainEvent(itemDeletedDomainEvent);
    }

    private void ThrowDeletedException() =>
        throw new ListDomainException("项目已删除。");

}
