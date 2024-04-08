using MediatR;
using TheSalLab.GeneralReturnValues;

namespace RecAll.Core.List.Api.Application.Commands;

public class CreateListCommand(string name, int typeId) : IRequest<ServiceResult>
{
    public string Name { get; set; } = name;

    public int TypeId { get; set; } = typeId;
}

// ----- 和之前编写的 CreateItemCommand 一样，这里也是一个简单的 DTO，用于传递创建列表的请求。 -----
// 1.但是和 CreateItemCommand 不一样的是：这里并没有使用[required]来进行验证
// 这是因为我们将在下一节中使用 FluentValidation 来进行验证（换一种验证数据的方法）。

// 2.这里的 CreateListCommand 类实现了 IRequest<ServiceResult> 接口。(对比起来 CreateItemCommand 之中什么都没有实现 )
// IRequest 和中介者模式有关系，它实际上就只是一个空接口 

// 有了 CreateListCommand 之后就可以去建立对应的 Controller 了