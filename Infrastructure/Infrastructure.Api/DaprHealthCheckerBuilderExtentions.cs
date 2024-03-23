namespace RecAll.Infrastructure.Infrastructure.Api;

public static class DaprHealthCheckerBuilderExtentions
{
  public static IHealthChecksBuilder
    AddDapr(this IHealthChecksBuilder builder) =>
    builder.AddCheck<DaprHealthChecker>("dapr");
}
