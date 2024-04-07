using Recall.Infrastructure.Ddd.Domain.Exceptions;

namespace RecAll.Core.List.Domain.Exceptions;

/// <summary>
/// 异常标识类，用于标识List领域层的异常
/// </summary> <summary>
/// 
/// </summary>
public class ListDomainException : DomainException
{
    public ListDomainException()
    {
    }

    public ListDomainException(string message) : base(message)
    {
    }

    public ListDomainException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
