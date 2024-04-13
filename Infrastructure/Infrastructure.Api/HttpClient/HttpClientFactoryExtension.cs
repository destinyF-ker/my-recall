namespace RecAll.Infrastructure.Infrastructure.Api.HttpClient;

/// <summary>
/// 命名的 HttpClient 工厂扩展
/// </summary> <summary>
/// 
/// </summary>
public static class HttpClientFactoryExtension
{
    public const string DefaultClient = nameof(DefaultClient);

    public static System.Net.Http.HttpClient CreateDefaultClient(
        this IHttpClientFactory httpClientFactory) =>
        httpClientFactory.CreateClient(DefaultClient);
}
