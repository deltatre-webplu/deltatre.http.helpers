using System;
using System.Runtime.Serialization;

namespace Deltatre.Http.Helpers.Exceptions
{
  /// <summary>
  ///  The exception which is thrown when the response returned by the invoked JSON web-api
  ///  has a response media type of application/json, but an error occurred while deserializing
  ///  the JSON response content. This usually occurs when the response content contains invalid JSON
  ///  or a string content which is not JSON.
  /// </summary>
  [Serializable]
  public class JsonDeserializationException : JsonApiRequestException
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="JsonDeserializationException"/> class.
    /// </summary>
    public JsonDeserializationException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonDeserializationException" /> class
    /// with a specified error message.</summary>
    /// <param name="message">
    /// The message that describes the error.
    /// </param>
    public JsonDeserializationException(
      string? message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonDeserializationException" /> class 
    /// with a specified error message and a reference to the inner exception 
    /// that is the cause of this exception.
    /// </summary>
    /// <param name="message">
    /// The error message that explains the reason for the exception.
    /// </param>
    /// <param name="innerException">
    /// The exception that is the cause of the current exception, 
    /// or a null reference (<see langword="Nothing" /> in Visual Basic) if no inner exception is specified.
    /// </param>
    public JsonDeserializationException(
      string? message,
      Exception? innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonDeserializationException" /> class
    /// with serialized data.</summary>
    /// <param name="info">
    /// The <see cref="SerializationInfo" /> that holds the serialized object data about 
    /// the exception being thrown.
    /// </param>
    /// <param name="context">
    /// The <see cref="StreamingContext" /> that contains contextual information 
    /// about the source or destination.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// When <paramref name="info" /> is <see langword="null" />.
    /// </exception>
    /// <exception cref="SerializationException">
    /// The class name is <see langword="null" /> or <see cref="Exception.HResult" /> is zero (0).
    /// </exception>
    protected JsonDeserializationException(
      SerializationInfo info,
      StreamingContext context) : base(info, context)
    {
    }
  }
}
