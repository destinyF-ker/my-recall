using System.Text.Json;
using RecAll.Infrastructure.Infrastructure.Api.HttpClient;
using MediatR;
using RecAll.Core.List.Api.Application.Queries;
using RecAll.Core.List.Api.Infrastructure.Services;
using RecAll.Core.List.Domain.AggregateModels.ItemAggregate;
using TheSalLab.GeneralReturnValues;

namespace RecAll.Core.List.Api.Application.Commands;

public class CreateItemCommandHandler :
    IRequestHandler<CreateItemCommand, ServiceResult>
{
    private ISetQueryService _setQueryService;
    private readonly IIdentityService _identityService;
    private readonly IContribUrlService _contribUrlService;
    private readonly HttpClient _httpClient;
    private readonly IItemRepository _itemRepository;

    public CreateItemCommandHandler(
        ISetQueryService setQueryService,
        IIdentityService identityService,
        IItemRepository itemRepository,
        IContribUrlService contribUrlService,
        IHttpClientFactory httpClientFactory)
    {
        _setQueryService = setQueryService ??
                           throw new ArgumentNullException(nameof(setQueryService));
        _identityService = identityService ??
                           throw new ArgumentNullException(nameof(identityService));
        _itemRepository = itemRepository ??
                          throw new ArgumentNullException(nameof(itemRepository));
        _contribUrlService = contribUrlService;
        _httpClient = httpClientFactory.CreateDefaultClient();
    }

    /// <summary>
    /// 转发微服务，首先要知道题目对应 Set 的 Type
    /// </summary>
    /// <param name="command"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns> <summary>
    /// 
    /// </summary>
    /// <param name="command"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<ServiceResult> Handle(CreateItemCommand command,
        CancellationToken cancellationToken)
    {
        var set = await _setQueryService.GetAsync(command.SetId,
            _identityService.GetUserIdentityGuid());

        // 需要一个接口，根据获取到的 TypeId 决定应该转发到哪个微服务
        var contribUrl =
            $"{_contribUrlService.GetContribUrl(set.TypeId)}/Item/create";

        // JsonContent：HTTP 传送的 Json 数据形式
        var jsonContent = JsonContent.Create(command.CreateContribJson,
            options: new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = false
            });

        HttpResponseMessage response;
        try
        {
            response = await _httpClient.PostAsync(contribUrl, jsonContent,
                cancellationToken);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception e)
        {
            return ServiceResult.CreateExceptionResult(e,
                $"访问Contrib Url时发生错误。TypeId: {set.TypeId}, ContribUrl: {contribUrl}");
        }

        var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
#pragma warning disable CS8602 // 解引用可能出现空引用。
        var contribResult = JsonSerializer
                            .Deserialize<ServiceResultViewModel<string>>(
                                responseJson,
                                new JsonSerializerOptions
                                {
                                    PropertyNameCaseInsensitive = true
                                }).ToServiceResult();
#pragma warning restore CS8602 // 解引用可能出现空引用。

        if (contribResult.Status != ServiceResultStatus.Succeeded)
        {
            return contribResult;
        }

        var item = _itemRepository.Add(new Item(set.TypeId, command.SetId,
            contribResult.Result, _identityService.GetUserIdentityGuid()));
        var saved =
            await _itemRepository.UnitOfWork.SaveEntitiesAsync(
                cancellationToken);

        if (!saved)
        {
            return ServiceResult.CreateFailedResult();
        }

        return await _itemRepository.UnitOfWork.SaveEntitiesAsync(
            cancellationToken)
            ? ServiceResult.CreateSucceededResult()
            : ServiceResult.CreateFailedResult();
    }
}
