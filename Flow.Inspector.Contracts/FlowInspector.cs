using Staticsoft.Contracts.Abstractions;
using Staticsoft.HttpCommunication.Abstractions;
using System;

namespace Staticsoft.Flow.Inspector.Contracts;

public class FlowInspector(
    HttpEndpoint<EmptyRequest, FlowInspector.ListJobsResponse> listJobs
)
{
    [Endpoint(HttpMethod.Get)]
    public HttpEndpoint<EmptyRequest, ListJobsResponse> ListJobs { get; } = listJobs;

    public class ListJobsResponse
    {
        public required Job[] Jobs { get; init; }

        public class Job
        {
            public required DateTime CreatedAt { get; init; }
            public required string Id { get; init; }
            public required string Name { get; init; }
            public required string Input { get; init; }
            public required string Output { get; init; }
            public required string Stats { get; init; }
        }
    }
}
