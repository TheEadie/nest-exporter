using System.Net;

namespace NestExporter.Tests;

public class MockHttpMessageHandler : HttpMessageHandler
{
    private readonly Queue<Response> _responses;

    public MockHttpMessageHandler() =>
        _responses = new Queue<Response>();

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = _responses.Dequeue();
        return Task.FromResult(new HttpResponseMessage { StatusCode = response.StatusCode, Content = new StringContent(response.Message) });
    }

    public void AddResponse(HttpStatusCode statusCode, string message) =>
        _responses.Enqueue(new Response(statusCode, message));
}

internal record Response(HttpStatusCode StatusCode, string Message);
