using Deltatre.Http.Helpers.Exceptions;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Deltatre.Http.Helpers.UnitTests
{
  [TestFixture]
  public sealed partial class HttpClientJsonExtensionsTest
  {
    [Test]
    public async Task GetJsonAsync_Sends_Get_Request_To_Provided_Uri()
    {
      // ARRANGE
      var jack = new Student { Name = "Jack", Age = 33 };
      var alice = new Student { Name = "Alice", Age = 25 };
      var students = new List<Student> { jack, alice };
      var json = JsonConvert.SerializeObject(students);

      using var httpResponseContent = new StringContent(
        json,
        Encoding.UTF8,
        "application/json");

      using var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
      {
        Content = httpResponseContent
      };

      var httpMessageHandlerMock = CreateHttpMessageHandlerMock(httpResponse);
      using var target = CreateTarget(httpMessageHandlerMock);

      // ACT
      using var cts = new CancellationTokenSource();
      var uri = new Uri("https://foo.com/bar");

      _ = await target.GetJsonAsync<List<Student>>(
        uri,
        cancellationToken: cts.Token).ConfigureAwait(false);

      // ASSERT
      httpMessageHandlerMock
        .Protected()
        .Verify(
          "SendAsync",
          Times.Exactly(1),
          ItExpr.IsAny<HttpRequestMessage>(),
          ItExpr.IsAny<CancellationToken>()
        );

      httpMessageHandlerMock
        .Protected()
        .Verify(
          "SendAsync",
          Times.Exactly(1),
          ItExpr.Is<HttpRequestMessage>(req =>
            req.Method == HttpMethod.Get
            &&
            req.RequestUri == uri
          ),
          ItExpr.IsAny<CancellationToken>()
        );
    }

    [Test]
    public async Task GetJsonAsync_Returns_Deserialized_Response_When_Invoked_Endpoint_Returns_Success_Json_Response()
    {
      // ARRANGE
      var jack = new Student { Name = "Jack", Age = 33 };
      var alice = new Student { Name = "Alice", Age = 25 };
      var students = new List<Student> { jack, alice };
      var json = JsonConvert.SerializeObject(students);

      using var httpResponseContent = new StringContent(
        json,
        Encoding.UTF8,
        "application/json");

      using var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
      {
        Content = httpResponseContent
      };

      var httpMessageHandlerMock = CreateHttpMessageHandlerMock(httpResponse);
      using var target = CreateTarget(httpMessageHandlerMock);

      // ACT
      using var cts = new CancellationTokenSource();
      var uri = new Uri("https://foo.com/bar");

      var result = await target.GetJsonAsync<List<Student>>(
        uri,
        cancellationToken: cts.Token).ConfigureAwait(false);

      // ASSERT
      Assert.IsNotNull(result);
      Assert.AreEqual(2, result!.Count);

      // check result
      var student = result[0];

      Assert.IsNotNull(student);
      Assert.AreEqual("Jack", student.Name);
      Assert.AreEqual(33, student.Age);

      student = result[1];
      Assert.IsNotNull(student);
      Assert.AreEqual("Alice", student.Name);
      Assert.AreEqual(25, student.Age);
    }

    [Test]
    public async Task GetJsonAsync_Returns_Deserialized_Response_When_Invoked_Endpoint_Returns_Success_Json_Response_In_Camel_Case()
    {
      // ARRANGE
      var jack = new Student { Name = "Jack", Age = 33 };
      var alice = new Student { Name = "Alice", Age = 25 };
      var students = new List<Student> { jack, alice };

      var settings = new JsonSerializerSettings
      {
        ContractResolver = new CamelCasePropertyNamesContractResolver()
      };
      var json = JsonConvert.SerializeObject(students, settings);

      using var httpResponseContent = new StringContent(
        json,
        Encoding.UTF8,
        "application/json");

      using var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
      {
        Content = httpResponseContent
      };

      var httpMessageHandlerMock = CreateHttpMessageHandlerMock(httpResponse);
      using var target = CreateTarget(httpMessageHandlerMock);

      // ACT
      using var cts = new CancellationTokenSource();
      var uri = new Uri("https://foo.com/bar");

      var result = await target.GetJsonAsync<List<Student>>(
        uri,
        cancellationToken: cts.Token).ConfigureAwait(false);

      // ASSERT
      Assert.IsNotNull(result);
      Assert.AreEqual(2, result!.Count);

      // check result
      var student = result[0];

      Assert.IsNotNull(student);
      Assert.AreEqual("Jack", student.Name);
      Assert.AreEqual(33, student.Age);

      student = result[1];
      Assert.IsNotNull(student);
      Assert.AreEqual("Alice", student.Name);
      Assert.AreEqual(25, student.Age);
    }

    [Test]
    public async Task GetJsonAsync_Uses_Provided_Json_Serializer_Settings()
    {
      // ARRANGE
      var metric = new Metric
      {
        Name = "Foo",
        Value = 45.67,
        Kind = MetricKind.DurationMetric
      };

      var namingStrategy = new KebabCaseNamingStrategy();
      var enumConverter = new StringEnumConverter(namingStrategy);
      var settings = new JsonSerializerSettings
      {
        Converters = { enumConverter }
      };
      var json = JsonConvert.SerializeObject(metric, settings);

      using var httpResponseContent = new StringContent(
        json,
        Encoding.UTF8,
        "application/json");

      using var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
      {
        Content = httpResponseContent
      };

      var httpMessageHandlerMock = CreateHttpMessageHandlerMock(httpResponse);
      using var target = CreateTarget(httpMessageHandlerMock);

      // ACT
      using var cts = new CancellationTokenSource();
      var uri = new Uri("https://foo.com/bar");

      var result = await target.GetJsonAsync<Metric>(
        uri,
        settings: settings,
        cancellationToken: cts.Token).ConfigureAwait(false);

      // ASSERT
      Assert.IsNotNull(result);
      Assert.AreEqual("Foo", result!.Name);
      Assert.AreEqual(45.67, result.Value);
      Assert.AreEqual(MetricKind.DurationMetric, result.Kind);
    }

    [Test]
    public void GetJsonAsync_Throws_ArgumentNullException_When_HttpClient_Is_Null()
    {
      // ARRANGE
      var uri = new Uri("https://foo.com/bar");

      // ACT
      using var cts = new CancellationTokenSource();

      var exception = Assert.ThrowsAsync<ArgumentNullException>(
        () => HttpClientJsonExtensions.GetJsonAsync<List<Student>>(
        httpClient: null!,
        uri,
        cancellationToken: cts.Token));

      // ASSERT
      Assert.IsNotNull(exception);
      Assert.AreEqual("httpClient", exception!.ParamName);
    }

    [Test]
    public void GetJsonAsync_Throws_ArgumentNullException_When_RequestUri_Is_Null()
    {
      // ARRANGE
      var jack = new Student { Name = "Jack", Age = 33 };
      var alice = new Student { Name = "Alice", Age = 25 };
      var students = new List<Student> { jack, alice };
      var json = JsonConvert.SerializeObject(students);

      using var httpResponseContent = new StringContent(
        json,
        Encoding.UTF8,
        "application/json");

      using var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
      {
        Content = httpResponseContent
      };

      var httpMessageHandlerMock = CreateHttpMessageHandlerMock(httpResponse);
      using var target = CreateTarget(httpMessageHandlerMock);

      // ACT
      using var cts = new CancellationTokenSource();

      var exception = Assert.ThrowsAsync<ArgumentNullException>(
        () => target.GetJsonAsync<List<Student>>(
        requestUri: null!,
        cancellationToken: cts.Token)
      );

      // ASSERT
      Assert.IsNotNull(exception);
      Assert.AreEqual("requestUri", exception!.ParamName);
    }

    [Test]
    public void GetJsonAsync_Does_Not_Throw_When_Settings_Is_Null()
    {
      // ARRANGE
      var jack = new Student { Name = "Jack", Age = 33 };
      var alice = new Student { Name = "Alice", Age = 25 };
      var students = new List<Student> { jack, alice };
      var json = JsonConvert.SerializeObject(students);

      using var httpResponseContent = new StringContent(
        json,
        Encoding.UTF8,
        "application/json");

      using var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
      {
        Content = httpResponseContent
      };

      var httpMessageHandlerMock = CreateHttpMessageHandlerMock(httpResponse);
      using var target = CreateTarget(httpMessageHandlerMock);

      // ACT
      using var cts = new CancellationTokenSource();
      var uri = new Uri("https://foo.com/bar");

      Assert.DoesNotThrowAsync(
        () => target.GetJsonAsync<List<Student>>(
        uri,
        settings: null,
        cancellationToken: cts.Token)
      );
    }

    [Test]
    public void GetJsonAsync_Throws_HttpInfrastructureException_When_Http_Request_Fails_Due_To_Infrastructure_Level_Problems()
    {
      // ARRANGE
      var httpRequestException = new HttpRequestException("Socket level error occurred");
      var httpMessageHandlerMock = CreateFailingHttpMessageHandlerMock(httpRequestException);

      using var target = CreateTarget(httpMessageHandlerMock);

      // ACT
      using var cts = new CancellationTokenSource();
      var uri = new Uri("https://foo.com/bar");

      var exception = Assert.ThrowsAsync<HttpInfrastructureException>(
        () => target.GetJsonAsync<Student>(
          uri,
          cancellationToken: cts.Token)
      );

      // ASSERT
      Assert.IsNotNull(exception);

      Assert.AreEqual(
        "An error has occurred while issuing GET request: 'Socket level error occurred' (request URI: https://foo.com/bar)",
        exception!.Message
      );

      Assert.AreSame(httpRequestException, exception.InnerException);
    }

    [Test]
    public void GetJsonAsync_Throws_HttpRequestTimeoutException_When_Http_Request_Times_Out()
    {
      // ARRANGE
      var taskCanceledException = new TaskCanceledException("HTTP request timed out after 45 seconds");
      var httpMessageHandlerMock = CreateFailingHttpMessageHandlerMock(taskCanceledException);

      using var target = CreateTarget(httpMessageHandlerMock);

      // ACT
      using var cts = new CancellationTokenSource();
      var uri = new Uri("https://foo.com/bar");

      var exception = Assert.ThrowsAsync<HttpRequestTimeoutException>(
        () => target.GetJsonAsync<Student>(
          uri,
          cancellationToken: cts.Token)
      );

      // ASSERT
      Assert.IsNotNull(exception);

      Assert.AreEqual(
        "GET request to https://foo.com/bar timed out",
        exception!.Message
      );

      Assert.AreSame(taskCanceledException, exception.InnerException);
    }

    [Test]
    public void GetJsonAsync_Does_Not_Catch_TaskCanceledException_When_Cancellation_Token_Is_Canceled()
    {
      // ARRANGE
      var taskCanceledException = new TaskCanceledException("HTTP request timed out after 45 seconds");
      var httpMessageHandlerMock = CreateFailingHttpMessageHandlerMock(
        taskCanceledException,
        millisecondsDelay: 500); // throws after 500 milliseconds

      using var target = CreateTarget(httpMessageHandlerMock);

      // ACT
      using var cts = new CancellationTokenSource(millisecondsDelay: 10); // cancellation after 10 milliseconds
      var uri = new Uri("https://foo.com/bar");

      var exception = Assert.ThrowsAsync<TaskCanceledException>(
        () => target.GetJsonAsync<Student>(
          uri,
          cancellationToken: cts.Token)
      );

      // ASSERT
      Assert.AreSame(exception, taskCanceledException);
    }

    [Test]
    public void GetJsonAsync_Throws_NonSuccessStatusCodeException_When_Http_Response_Has_Non_Success_Status_Code()
    {
      // ARRANGE
      const string requestUrl = "https://foo.com/bar";

      using var httpResponse = new HttpResponseMessage(HttpStatusCode.InternalServerError)
      {
        RequestMessage = new HttpRequestMessage(HttpMethod.Get, requestUrl)
      };

      var httpMessageHandlerMock = CreateHttpMessageHandlerMock(httpResponse);
      using var target = CreateTarget(httpMessageHandlerMock);

      // ACT
      using var cts = new CancellationTokenSource();
      var uri = new Uri(requestUrl);

      var exception = Assert.ThrowsAsync<NonSuccessStatusCodeException>(() =>
        target.GetJsonAsync<List<Student>>(
          uri,
          cancellationToken: cts.Token
        )
      );

      // ASSERT
      Assert.IsNotNull(exception);

      Assert.AreEqual(
        "Got 500 status code when issuing HTTP request (request URI: https://foo.com/bar request method: GET)",
        exception!.Message
      );

      Assert.AreEqual(500, exception.HttpStatusCode);
    }

    [Test]
    public void GetJsonAsync_Throws_EmptyResponseBodyException_When_Http_Response_Content_Is_Empty()
    {
      // ARRANGE
      const string requestUrl = "https://foo.com/bar";

      using var httpResponse = new HttpResponseMessage(HttpStatusCode.NoContent)
      {
        RequestMessage = new HttpRequestMessage(HttpMethod.Get, requestUrl),
        Content = new EmptyResponseContent()
      };
      httpResponse.Content.Headers.Add("Content-Length", "0");

      var httpMessageHandlerMock = CreateHttpMessageHandlerMock(httpResponse);
      using var target = CreateTarget(httpMessageHandlerMock);

      // ACT
      using var cts = new CancellationTokenSource();
      var uri = new Uri(requestUrl);

      var exception = Assert.ThrowsAsync<EmptyResponseBodyException>(() =>
        target.GetJsonAsync<List<Student>>(
          uri,
          cancellationToken: cts.Token
        )
      );

      // ASSERT
      Assert.IsNotNull(exception);

      Assert.AreEqual(
        "HTTP request returned empty response body. JSON response body is expected instead (request URI: https://foo.com/bar request method: GET)",
        exception!.Message
      );
    }

    [Test]
    public void GetJsonAsync_Throws_EmptyResponseBodyException_When_Http_Response_Content_Is_Null()
    {
      // ARRANGE
      const string requestUrl = "https://foo.com/bar";

      using var httpResponse = new HttpResponseMessage(HttpStatusCode.NoContent)
      {
        RequestMessage = new HttpRequestMessage(HttpMethod.Get, requestUrl),
        Content = null
      };

      var httpMessageHandlerMock = CreateHttpMessageHandlerMock(httpResponse);
      using var target = CreateTarget(httpMessageHandlerMock);

      // ACT
      using var cts = new CancellationTokenSource();
      var uri = new Uri(requestUrl);

      var exception = Assert.ThrowsAsync<EmptyResponseBodyException>(() =>
        target.GetJsonAsync<List<Student>>(
          uri,
          cancellationToken: cts.Token
        )
      );

      // ASSERT
      Assert.IsNotNull(exception);

      Assert.AreEqual(
        "HTTP request returned empty response body. JSON response body is expected instead (request URI: https://foo.com/bar request method: GET)",
        exception!.Message
      );
    }

    [Test]
    public void GetJsonAsync_Throws_UnexpectedResponseMediaTypeException_When_Response_Media_Type_Is_Not_Application_Json()
    {
      // ARRANGE
      const string requestUrl = "https://foo.com/bar";

      var html = @"
        <!DOCTYPE html>
        <html lang='en'>
        <head>
            <meta charset='UTF-8'>
            <meta http-equiv='X-UA-Compatible' content='IE=edge'>
            <meta name='viewport' content='width=device-width, initial-scale=1.0'>
            <title>Document</title>
        </head>
        <body>
            <p>Hello World!</p>
        </body>
        </html>".Replace('\'', '"');

      using var responseContent = new StringContent(
        html,
        Encoding.UTF8,
        "text/html");

      using var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
      {
        RequestMessage = new HttpRequestMessage(HttpMethod.Get, requestUrl),
        Content = responseContent
      };

      var httpMessageHandlerMock = CreateHttpMessageHandlerMock(httpResponse);
      using var target = CreateTarget(httpMessageHandlerMock);

      // ACT
      using var cts = new CancellationTokenSource();
      var uri = new Uri(requestUrl);

      var exception = Assert.ThrowsAsync<UnexpectedResponseMediaTypeException>(() =>
        target.GetJsonAsync<List<Student>>(
          uri,
          cancellationToken: cts.Token
        )
      );

      // ASSERT
      Assert.IsNotNull(exception);

      Assert.AreEqual(
        "HTTP response content has text/html media type. application/json media type is expected instead (request URI: https://foo.com/bar request method: GET)",
        exception!.Message
      );

      Assert.AreEqual("text/html", exception.ResponseMediaType);
    }

    [Test]
    public void GetJsonAsync_Throws_JsonDeserializationException_When_An_Error_Occurs_While_Deserializing_Response_Content()
    {
      // ARRANGE
      const string requestUrl = "https://foo.com/bar";

      // following JSON is invalid
      // double quotes are missing around the field name and closing curly brace is missing too
      const string invalidJson = "{Name:\"Enrico\", Age: 34";

      using var responseContent = new StringContent(
        invalidJson,
        Encoding.UTF8,
        "application/json");

      using var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
      {
        RequestMessage = new HttpRequestMessage(HttpMethod.Get, requestUrl),
        Content = responseContent
      };

      var httpMessageHandlerMock = CreateHttpMessageHandlerMock(httpResponse);
      using var target = CreateTarget(httpMessageHandlerMock);

      // ACT
      using var cts = new CancellationTokenSource();
      var uri = new Uri(requestUrl);

      var exception = Assert.ThrowsAsync<JsonDeserializationException>(() =>
        target.GetJsonAsync<Student>(
          uri,
          cancellationToken: cts.Token
        )
      );

      // ASSERT
      Assert.IsNotNull(exception);
      Assert.IsFalse(string.IsNullOrWhiteSpace(exception!.Message));
      Assert.IsNotNull(exception.InnerException);
      Assert.That(exception.InnerException, Is.AssignableTo<JsonException>());
    }
  }
}
