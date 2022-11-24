﻿namespace WildlifeMortalities.PosseExampleApi.Features.Clients.GetByEnvClientId;

public class Endpoint : Endpoint<GetClientByEnvClientIdRequest, GetClientByEnvClientIdResponse>
{
    public override void Configure()
    {
        Get("/clients/{EnvClientId}");
        Policies("ApiKey");
        Description(b => b
            .Produces<GetClientByEnvClientIdResponse>()
            .Produces(404));
        Summary(s =>
        {
            s.ResponseExamples[200] = new GetClientByEnvClientIdResponse
            {
                ClientDetails = new ClientDetails(
                    new[] { "432032" },
                    "John",
                    "Doe",
                    new DateOnly(1994, 11, 25),
                    DateTimeOffset.Now)
            };
        });
    }

    public override async Task HandleAsync(GetClientByEnvClientIdRequest req, CancellationToken ct)
    {
        var response = new GetClientByEnvClientIdResponse
        {
            ClientDetails = new ClientDetails(
                new[] { "43203" },
                "John",
                "Doe",
                new DateOnly(1984, 11, 25),
                DateTimeOffset.Now)
        };

        await SendAsync(response);
    }
}

public class GetClientByEnvClientIdRequest
{
    public string? EnvClientId { get; set; }
}

public class GetClientByEnvClientIdResponse
{
    public ClientDetails ClientDetails { get; set; }
}

public record ClientDetails(
    IEnumerable<string> PreviousEnvClientIds,
    string FirstName,
    string LastName,
    DateOnly BirthDate,
    DateTimeOffset LastModifiedDateTime
);