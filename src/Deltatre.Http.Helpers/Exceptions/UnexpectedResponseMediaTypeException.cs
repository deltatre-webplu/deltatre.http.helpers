using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Deltatre.Http.Helpers.Exceptions
{
  /// <summary>
  /// The exception which is thrown when the invoked JSON web-api returns a response media type
  /// other than application/json or a response without the Content-Type header.
  /// </summary>
  [Serializable]
  public class UnexpectedResponseMediaTypeException : JsonApiRequestException
  {
    /// <summary>
    /// Get or sets the response media type returned by the invoked JSON web-api.
    /// This property is set to <see langword="null"/> if the response returned by the
    /// invoked JSON web-api does not specify the Content-Type header,
    /// otherwise its value is a string containing the response media type.
    /// </summary>
    public string? ResponseMediaType { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnexpectedResponseMediaTypeException"/> class.
    /// </summary>
    public UnexpectedResponseMediaTypeException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnexpectedResponseMediaTypeException" />
    /// class with a specified error message.
    /// </summary>
    /// <param name="message">
    /// The message that describes the error.
    /// </param>
    public UnexpectedResponseMediaTypeException(
      string? message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnexpectedResponseMediaTypeException" /> 
    /// class with serialized data.
    /// </summary>
    /// <param name="info">
    /// The <see cref="SerializationInfo" /> that holds the serialized object data 
    /// about the exception being thrown.
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
    protected UnexpectedResponseMediaTypeException(
      SerializationInfo info,
      StreamingContext context) : base(info, context)
    {
      this.ResponseMediaType = info.GetString(nameof(this.ResponseMediaType));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnexpectedResponseMediaTypeException" /> class 
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
    public UnexpectedResponseMediaTypeException(
      string? message,
      Exception? innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// When overridden in a derived class, sets the <see cref="SerializationInfo" /> 
    /// with information about the exception.
    /// </summary>
    /// <param name="info">
    /// The <see cref="SerializationInfo" /> that holds the serialized 
    /// object data about the exception being thrown.
    /// </param>
    /// <param name="context">
    /// The <see cref="StreamingContext" /> that contains contextual information
    /// about the source or destination.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// The <paramref name="info" /> parameter is a null reference (<see langword="Nothing" /> in Visual Basic).
    /// </exception>
    [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      if (info == null)
      {
        throw new ArgumentNullException(nameof(info));
      }

      info.AddValue(nameof(this.ResponseMediaType), this.ResponseMediaType);

      base.GetObjectData(info, context);
    }
  }
}
