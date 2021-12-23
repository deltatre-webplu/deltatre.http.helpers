using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Deltatre.Http.Helpers.Exceptions
{
  /// <summary>
  /// This is the exception thrown when the invoked JSON web-api returns a non success status code.
  /// Successfull status codes are the ones in the range between 200 and 299.
  /// </summary>
  [Serializable]
  public class NonSuccessStatusCodeException : JsonApiRequestException
  {
    /// <summary>
    /// Gets or sets the HTTP status code returned by the invoked JSON web-api
    /// </summary>
    public int HttpStatusCode { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="NonSuccessStatusCodeException"/> class.
    /// </summary>
    public NonSuccessStatusCodeException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NonSuccessStatusCodeException" /> class with a specified error message.
    /// </summary>
    /// <param name="message">
    /// The message that describes the error.
    /// </param>
    public NonSuccessStatusCodeException(string? message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NonSuccessStatusCodeException" /> class with serialized data.
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
    protected NonSuccessStatusCodeException(
      SerializationInfo info,
      StreamingContext context) : base(info, context)
    {
      this.HttpStatusCode = info.GetInt32(nameof(this.HttpStatusCode));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NonSuccessStatusCodeException" /> class 
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
    public NonSuccessStatusCodeException(
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

      info.AddValue(nameof(this.HttpStatusCode), this.HttpStatusCode);
      
      base.GetObjectData(info, context);
    }
  }
}
