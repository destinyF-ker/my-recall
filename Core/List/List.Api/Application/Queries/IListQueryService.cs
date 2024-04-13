namespace RecAll.Core.List.Api.Application.Queries;

public interface IListQueryService
{
    // ViewModels为了方便前端展示（隐藏一部分内容），不同于Domain Models
    Task<(IEnumerable<ListViewModel>, int)> ListAsync(int skip, int take,
       string userIdentityGuid);

    Task<(IEnumerable<ListViewModel>, int)> ListAsync(int typeId, int skip,
        int take, string userIdentityGuid);


    Task<ListViewModel> GetAsync(int id, string userIdentityGuid);
}
