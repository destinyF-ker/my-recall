using MediatR;
using TheSalLab.GeneralReturnValues;

namespace RecAll.Core.List.Api.Application.Commands;

public class CreateSetCommand : IRequest<ServiceResult>
{
    // 这里只有 ListId，没有 TypeId，因为永远不要详细用户提供的信息
    public string Name { get; set; }

    public int ListId { get; set; }

    public CreateSetCommand(string name, int listId)
    {
        Name = name;
        ListId = listId;
    }
}
