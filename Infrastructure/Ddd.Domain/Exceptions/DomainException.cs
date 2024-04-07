namespace Recall.Infrastructure.Ddd.Domain.Exceptions;

/// <summary>
/// 此类实际上就是一个标记类，用于标记领域层的异常。
/// </summary> 
public class DomainException : Exception
{
    public DomainException() { }

    public DomainException(string message) : base(message) { }

    public DomainException(string message, Exception innerException) : base(
        message, innerException)
    { }
}

// 此外，这个项目中还用到了一个Nuget包：MediatR