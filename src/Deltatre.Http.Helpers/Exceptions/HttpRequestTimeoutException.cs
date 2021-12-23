using System;
using System.Net.Http;
using System.Runtime.Serialization;

namespace Deltatre.Http.Helpers.Exceptions
{
  /// <summary>
  /// The exception that is thrown when the HTTP request times out.
  /// The timeout time can be set by using the <see cref="HttpClient.Timeout"/> property.
  /// </summary>
  [Serializable]
  public class HttpRequestTimeoutException : JsonApiRequestException
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="HttpRequestTimeoutException"/> class.
    /// </summary>
    public HttpRequestTimeoutException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpRequestTimeoutException" /> class with a specified error message.
    /// </summary>
    /// <param name="message">
    /// The message that describes the error.
    /// </param>
    public HttpRequestTimeoutException(
      string? message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpRequestTimeoutException" /> class with serialized data.
    /// </summary>
    /// <param name="info">
    /// The <see cref="SerializationInfo" /> that holds the serialized object data about the exception being thrown.
    /// </param>
    /// <param name="context">
    /// The <see cref="StreamingContext" /> that contains contextual information about the source or destination.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// When <paramref name="info" /> is <see langword="null" />.
    /// </exception>
    /// <exception cref="SerializationException">
    /// The class name is <see langword="null" /> or <see cref="Exception.HResult" /> is zero (0).
    /// </exception>
    protected HttpRequestTimeoutException(
      SerializationInfo info,
      StreamingContext context) : base(info, context)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpRequestTimeoutException" /> class 
    /// with a specified error message and a reference to the inner exception
    /// that is the cause of this exception.
    /// </summary>
    /// <param name="message">
    /// The error message that explains the reason for the exception.
    /// </param>
    /// <param name="innerException">
    /// The exception that is the cause of the current exception,
    /// or a null reference (<see langword="Nothing" /> in Visual Basic)
    /// if no inner exception is specified.
    /// </param>
    public HttpRequestTimeoutException(
      string? message,
      Exception? innerException) : base(message, innerException)
    {
    }
  }
}
