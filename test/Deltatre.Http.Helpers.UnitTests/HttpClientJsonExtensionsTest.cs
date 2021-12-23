using Moq;
using Moq.Protected;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Deltatre.Http.Helpers.UnitTests
{
  public sealed partial class HttpClientJsonExtensionsTest
  {
    private static HttpClient CreateTarget(Mock<HttpMessageHandler> handlerMock) => 
      new HttpClient(handlerMock.Object);

    private static Mock<HttpMessageHandler> CreateHttpMessageHandlerMock(
      HttpResponseMessage httpResponseMessage,
      int millisecondsDelay = 3)
    {
      var httpMessageHandlerMock = new Mock<HttpMessageHandler>();

      httpMessageHandlerMock
       .Protected()
       .Setup<Task<HttpResponseMessage>>(
          "SendAsync",
          ItExpr.IsAny<HttpRequestMessage>(),
          ItExpr.IsAny<CancellationToken>()
       )
       .ReturnsAsync(
          httpResponseMessage,
          TimeSpan.FromMilliseconds(millisecondsDelay)
       ).Verifiable();

      return httpMessageHandlerMock;
    }

    private static Mock<HttpMessageHandler> CreateFailingHttpMessageHandlerMock(
      Exception exception,
      int millisecondsDelay = 3)
    {
      var httpMessageHandlerMock = new Mock<HttpMessageHandler>();

      httpMessageHandlerMock
       .Protected()
       .Setup<Task<HttpResponseMessage>>(
          "SendAsync",
          ItExpr.IsAny<HttpRequestMessage>(),
          ItExpr.IsAny<CancellationToken>()
       )
       .ThrowsAsync(
          exception,
          TimeSpan.FromMilliseconds(millisecondsDelay)
       ).Verifiable();

      return httpMessageHandlerMock;
    }

    private sealed class Student 
    {
      public string? Name { get; set; }
      public int Age { get; set; }
    }

    private sealed class Metric
    {
      public string? Name { get; set; }
      public double Value { get; set; }
      public MetricKind Kind { get; set; }
    }

    private enum MetricKind 
    {
      None = 0,
      DurationMetric = 1,
      EventMetric = 2
    }

    private sealed class EmptyResponseContent : HttpContent
    {
      protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
      {
        return Task.CompletedTask;
      }

      protected override bool TryComputeLength(out long length)
      {
        length = 0;
        return false;
      }
    }
  }
}
