using Staticsoft.Contracts.Abstractions;
using Staticsoft.Flow;
using HttpMethod = Staticsoft.HttpCommunication.Abstractions.HttpMethod;

namespace Staticsoft.TestServer;

public class Api(
    HttpEndpoint<CalculateSumOfSquaresRequest, CalculateSumOfSquaresResponse> calculateSum,
    HttpEndpoint<GetCalculationResultRequest, GetCalculationResultResponse> getCalculationResult,
    HttpEndpoint<GetCalculationStatusRequest, JobStatus> getCalculationStatus
)
{
    [Endpoint(HttpMethod.Post)]
    public HttpEndpoint<CalculateSumOfSquaresRequest, CalculateSumOfSquaresResponse> CalculateSum { get; } = calculateSum;

    [Endpoint(HttpMethod.Post)]
    public HttpEndpoint<GetCalculationResultRequest, GetCalculationResultResponse> GetCalculationResult { get; } = getCalculationResult;

    [Endpoint(HttpMethod.Post)]
    public HttpEndpoint<GetCalculationStatusRequest, JobStatus> GetCalculationStatus { get; } = getCalculationStatus;
}

public class CalculateSumOfSquaresRequest
{
    public required int[] Numbers { get; init; }
}

public class CalculateSumOfSquaresResponse
{
    public required string JobId { get; init; }
}

public class GetCalculationResultRequest
{
    public required string JobId { get; init; }
}

public class GetCalculationResultResponse
{
    public int Sum { get; init; }
}

public class GetCalculationStatusRequest
{
    public required string JobId { get; init; }
}
