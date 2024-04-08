using RecAll.Core.List.Domain.Events;
using RecAll.Core.List.Domain.Exceptions;
using RecAll.Infrastructure.Ddd.Domain.SeedWork;

namespace RecAll.Core.List.Domain.AggregateModels.ListAggregate;

// 首先它是一个实体，应为 List 能够增删改查 
// 其次它是一个聚合根，因为它有对应的数据库操作 

// 有三种情况需要写数据：
// 1. 当 创建一个错题本的时候（new）
// 2. 当 修改一个错题本的时候（错题本的名字，错题本的类型，用户 ID）
// 3. 当 删除一个错题本的时候（IsDeleted = true）

/// <summary>
/// List 是错题本的一个实体，同时也是一个聚合根。
/// 可以说是一门课所有错题的集合
/// </summary> <summary>
/// 
/// </summary>
public class List : Entity, IAggregateRoot
{
    // 这里还有一个从 Entity 之中继承过来的 Id（主键 ）
    // protected int _id;

    // 在一般的设计之中 name 都是 public 的(不然就无法读写)，但是在这里是 private 的
    // 这是因为 领域驱动设计之中“读写分离”的原则，这里只负责写数据，不涉及读数据(在另外一个类之中，该类不在领域驱动设计之中) :to last line
    // 这里涉及到一个领域驱动设计的思想：DDD 只负责处理数据发生变化的部分，读数据并不会改变数据，所以不需要在领域驱动设计之中
    private string _name; // Save

    private int _typeId; // Save

    // 后续也许需要读出对应的类型，但是在数据库之中存储的是 typeId，所以在读取出的时候要做一个转换 
    /// <summary>
    /// <example> 
    /// 实际上就是：
    /// <code 
    /// public ListType Type
    /// {
    ///     get
    ///     {
    ///         if (_typeId == ListType.TextId)
    ///        {
    ///            return ListType.Text;
    ///        }

    ///         throw new ListDomainException("未知的错题本类型。");
    ///     }
    /// }
    /// </code>
    /// </example> 
    /// 但是实际上我们可以让数据库来帮我们做这方面的工作：通过主外键关联，我们可以直接读取出对应的类型
    /// 和上一次提交相比，需要一些改变 ，要把 ListType 存到数据库之中(专门添加一个类 ListTypeConfiguration.cs 来配置)
    public ListType Type { get; private set; }

    private string _userIdentityGuid; // Save

    public string UserIdentityGuid => _userIdentityGuid;

    private bool _isDeleted; // Save

    public bool IsDeleted => _isDeleted;

    // ----- 到这里可以发现，要存到数据库之中的字段都是 private 的，而要读取出来的字段都是 public 的 -----

    // ----- 下面的每一个操作都要对应领域事件  -----
    // 操作和领域事件之间的关系是“多对多”，一个操作可以对应多个领域事件(可以为 0)，一个领域事件也可以对应多个操作
    // 领域事件并不一定要被处理
    private List() { }

    public List(string name, int typeId, string userIdentityGuid) : this()
    {
        _name = name;
        _typeId = typeId;
        _userIdentityGuid = userIdentityGuid;

        var listCreatedDomainEvent = new ListCreatedDomainEvent(this);
        AddDomainEvent(listCreatedDomainEvent);
    }

    public void SetDeleted()
    {
        if (_isDeleted)
        {
            ThrowDeletedException();
        }

        _isDeleted = true;

        var listDeletedDomainEvent = new ListDeletedDomainEvent(this);
        AddDomainEvent(listDeletedDomainEvent);
    }

    // 约定了只能改名字 
    public void SetName(string name)
    {
        if (_isDeleted)
        {
            ThrowDeletedException();
        }

        _name = name;
    }

    private void ThrowDeletedException() =>
        throw new ListDomainException("列表已删除。");

}
// 但是为什么要公开 ListType 以及 UserIdentityGuid 、IsDeleted呢？
// 关于 UserIdentityGuid 和 IsDeleted，这是因为安全性的问题，我们需要在外部进行判断，这样做比较方便 
// 关于 ListType，应为后续要根据这个类型创建子 Set，所以需要公开这个字段