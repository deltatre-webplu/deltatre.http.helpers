using Deltatre.Http.Helpers.Exceptions;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;

namespace Deltatre.Http.Helpers
{
  /// <summary>
  /// A collection of extension methods useful to work with JSON web apis with the <see cref="HttpClient"/> class.
  /// </summary>
  public static class HttpClientJsonExtensions
  {
    /// <summary>
    /// This method invokes a GET endpoint on a JSON web-api and deserialize the HTTP response
    /// by using a provided binding model.
    /// </summary>
    /// <typeparam name="TResponse">
    /// The binding model to be used to deserialize the HTTP response.
    /// </typeparam>
    /// <param name="httpClient">
    /// The <see cref="HttpClient"/> used to issue the GET request.
    /// This parameter cannot be <see langword="null"/>.
    /// </param>
    /// <param name="requestUri">
    /// The Uri the request is sent to. This parameter cannot be <see langword="null"/>.
    /// </param>
    /// <param name="settings">
    /// The <see cref="JsonSerializerSettings"/> used to deserialize the HTTP response.
    /// If this parameter is <see langword="null"/>, default serialization settings will be used.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token that can be used by other objects or threads to cancel the operation.
    /// </param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// When <paramref name="requestUri"/> is <see langword="null"/>
    /// When <paramref name="httpClient"/> is <see langword="null"/>
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// When <paramref name="requestUri"/> is not an absolute Uri and <see cref="HttpClient.BaseAddress"/> is not set.
    /// </exception>
    /// <exception cref="HttpInfrastructureException">
    /// When the issued HTTP request fails due to infrastructure-level errors
    /// (e.g.: missing network connectivity, DNS-level failures, problems with the server certificate validation)
    /// </exception>
    /// <exception cref="HttpRequestTimeoutException">
    /// When the issued HTTP request fails due to timeout
    /// </exception>
    /// <exception cref="NonSuccessStatusCodeException">
    /// When the HTTP response has a status code outside of the range between 200 and 299
    /// </exception>
    /// <exception cref="EmptyResponseBodyException">
    /// When the HTTP response body is empty
    /// </exception>
    /// <exception cref="UnexpectedResponseMediaTypeException">
    /// When the HTTP response has a media type other than application/json or the Content-Type response header is not specified
    /// </exception>
    /// <exception cref="JsonDeserializationException">
    /// When an error occurs during the deserialization of the JSON response content (e.g: the response content contains invalid JSON)
    /// </exception>
    public static async Task<TResponse?> GetJsonAsync<TResponse>(
      this HttpClient httpClient,
      Uri requestUri,
      JsonSerializerSettings? settings = null,
      CancellationToken cancellationToken = default) where TResponse: class
    {
      if (httpClient == null)
      {
        throw new ArgumentNullException(nameof(httpClient));
      }

      if (requestUri == null)
      {
        throw new ArgumentNullException(nameof(requestUri));
      }

      using var response = await SendHttpRequest(httpClient, requestUri, cancellationToken).ConfigureAwait(false);

      EnsureSuccessStatusCode(response);

      EnsureResponseContentIsJson(response);

      return await DeserializeResponseContent<TResponse>(response, settings).ConfigureAwait(false);
    }

    private static async Task<HttpResponseMessage> SendHttpRequest(
      HttpClient httpClient,
      Uri requestUri,
      CancellationToken cancellationToken)
    {
      try
      {
        return await httpClient.GetAsync(
          requestUri,
          HttpCompletionOption.ResponseHeadersRead,
          cancellationToken).ConfigureAwait(false);
      }
      catch (HttpRequestException exception)
      {
        throw BuildHttpInfrastructureException(exception, requestUri);
      }
      catch (TaskCanceledException exception) when (!cancellationToken.IsCancellationRequested)
      {
        throw BuildHttpRequestTimeoutException(exception, requestUri);
      }

      static HttpInfrastructureException BuildHttpInfrastructureException(
        HttpRequestException innerException,
        Uri requestUri)
      {
        var message = FormattableString.Invariant(
          $"An error has occurred while issuing GET request: '{innerException.Message}' (request URI: {requestUri.AbsoluteUri})"
        );
        return new HttpInfrastructureException(message, innerException);
      }

      static HttpRequestTimeoutException BuildHttpRequestTimeoutException(
        TaskCanceledException innerException,
        Uri requestUri)
      {
        var message = FormattableString.Invariant(
          $"GET request to {requestUri.AbsoluteUri} timed out"
        );
        return new HttpRequestTimeoutException(message, innerException);
      }
    }

    private static void EnsureSuccessStatusCode(HttpResponseMessage httpResponse)
    {
      if (!httpResponse.IsSuccessStatusCode)
      {
        var statusCode = (int)httpResponse.StatusCode;

        var message = FormattableString.Invariant(
          $"Got {statusCode} status code when issuing HTTP request (request URI: {httpResponse.RequestMessage?.RequestUri?.AbsoluteUri} request method: {httpResponse.RequestMessage?.Method})"
        );

        throw new NonSuccessStatusCodeException(message)
        {
          HttpStatusCode = statusCode
        };
      }
    }

    private static void EnsureResponseContentIsJson(HttpResponseMessage httpResponse)
    {
      var responseContent = httpResponse.Content;
      var contentType = responseContent?.Headers?.ContentType;

      var isEmptyResponseContent = contentType == null;
      if (isEmptyResponseContent)
      {
        var message = FormattableString.Invariant(
          $"HTTP request returned empty response body. JSON response body is expected instead (request URI: {httpResponse.RequestMessage?.RequestUri?.AbsoluteUri} request method: {httpResponse.RequestMessage?.Method})"
        );

        throw new EmptyResponseBodyException(message);
      }

      var responseMediaType = contentType!.MediaType;
      var isJsonResponse = responseMediaType == MediaTypeNames.Application.Json;
      if (!isJsonResponse)
      {
        var message = FormattableString.Invariant(
          $"HTTP response content has {responseMediaType} media type. application/json media type is expected instead (request URI: {httpResponse.RequestMessage?.RequestUri?.AbsoluteUri} request method: {httpResponse.RequestMessage?.Method})"
        );

        throw new UnexpectedResponseMediaTypeException(message)
        {
          ResponseMediaType = responseMediaType
        };
      }
    }

    private static async Task<TResponse?> DeserializeResponseContent<TResponse>(
      HttpResponseMessage httpResponse,
      JsonSerializerSettings? settings) where TResponse: class
    {
      var stream = await httpResponse.Content.ReadAsStreamAsync().ConfigureAwait(false);

      await using (stream.ConfigureAwait(false))
      {
        using var streamReader = new StreamReader(stream);
        using var jsonReader = new JsonTextReader(streamReader);
        
        var serializer = JsonSerializer.Create(settings);

        try
        {
          return serializer.Deserialize<TResponse>(jsonReader);
        }
        catch (JsonException exception)
        {
          var message = FormattableString.Invariant(
            $"An error has occurred while deserializing JSON content of HTTP response: '{exception.Message}' (request URI: {httpResponse.RequestMessage?.RequestUri?.AbsoluteUri} request method: {httpResponse.RequestMessage?.Method})"
          );
          throw new JsonDeserializationException(message, exception);
        }
      }
    }
  }
}
