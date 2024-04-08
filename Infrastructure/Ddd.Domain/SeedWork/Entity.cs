using MediatR;

namespace RecAll.Infrastructure.Ddd.Domain.SeedWork;


/// <summary>
/// Entity 是一个抽象类，是所有实体的基类，实体是领域模型的基本组成部分，是领域模型的核心。
/// 由两个部分组成：id 和领域事件（可以有多个，代表自身发生了什么变化）。
/// 有很多种情况：1.一个类是 Entity，也是 AggregateRoot；2.一个类是 Entity，但不是 AggregateRoot。
/// </summary> <summary>
/// 
/// </summary>
public abstract class Entity
{
    protected int _id;

    public virtual int Id
    {
        get => _id;
        protected set => _id = value;
    }

    // 关心某一个事件是否已经发生了，使用的是中介者模式
    private List<INotification> _domainEvents;

    public IReadOnlyCollection<INotification> DomainEvents => _domainEvents?.AsReadOnly();

    public void AddDomainEvent(INotification eventItem)
    {
        _domainEvents ??= [];
        _domainEvents.Add(eventItem);
    }

    public void RemoveDomainEvent(INotification eventItem)
    {
        _domainEvents?.Remove(eventItem);
    }

    public void ClearDomainEvents()
    {
        _domainEvents?.Clear();
    }

    public bool IsTransient => this.Id == default;

    protected int? _requestedHashCode;

    public override bool Equals(object obj)
    {
        if (obj is null || !(obj is Entity))
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (this.GetType() != obj.GetType())
        {
            return false;
        }

        var item = (Entity)obj;

        if (item.IsTransient || this.IsTransient)
        {
            return false;
        }
        else
        {
            return item.Id == this.Id;
        }
    }

    public override int GetHashCode()
    {
        if (IsTransient)
        {
            return base.GetHashCode();
        }

        _requestedHashCode ??= Id.GetHashCode() ^ 31;
        return _requestedHashCode.Value;
    }

    public static bool operator ==(Entity left, Entity right) =>
        Equals(left, null) ? Equals(right, null) : left.Equals(right);

    public static bool operator !=(Entity left, Entity right) =>
        !(left == right);


}
