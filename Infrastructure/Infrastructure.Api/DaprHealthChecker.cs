﻿using Dapr.Client;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace RecAll.Infrastructure.Infrastructure.Api;

public class DaprHealthChecker : IHealthCheck
{
    private readonly DaprClient _daprClient;

    public DaprHealthChecker(DaprClient daprClient)
    {
        _daprClient = daprClient;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        var healthy = await _daprClient.CheckHealthAsync(cancellationToken);

        if (healthy)
        {
            return HealthCheckResult.Healthy("Dapr sidecar is healthy.");
        }

        return new HealthCheckResult(context.Registration.FailureStatus,
            "Dapr sidecar is unhealthy.");
    }

}
