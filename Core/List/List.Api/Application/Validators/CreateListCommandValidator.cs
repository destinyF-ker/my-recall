using FluentValidation;
using RecAll.Core.List.Api.Application.Commands;
using RecAll.Core.List.Domain.AggregateModels;
using RecAll.Infrastructure.Ddd.Domain.SeedWork;

namespace RecAll.Core.List.Api.Application.Validators;


/// <summary>
/// 可以发现 CreateListCommandValidator 继承了 AbstractValidator<CreateListCommand>，
/// 这个类是 FluentValidation 之中的一个类，说明它是对 CreateListCommand 进行验证的
/// </summary> <summary>
/// 
/// </summary>
public class CreateListCommandValidator : AbstractValidator<CreateListCommand>
{
    public CreateListCommandValidator(
        ILogger<CreateListCommandValidator> logger)
    {
        RuleFor(p => p.Name).NotEmpty(); // Name 不能为空
        RuleFor(p => p.TypeId).NotEmpty(); // TypeId 不能为空
        RuleFor(p => p.TypeId)
            .Must(Enumeration.IsValidValue<ListType>) // TypeId 必须是一个有效的 Type ID
            .WithMessage("无效的Type ID");
        logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
    }
}
