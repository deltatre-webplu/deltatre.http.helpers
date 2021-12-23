using Deltatre.Http.Helpers.Exceptions;
using NUnit.Framework;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Deltatre.Http.Helpers.UnitTests.Exceptions
{
  [TestFixture]
  [SuppressMessage("Security", "CA2301:Do not call BinaryFormatter.Deserialize without first setting BinaryFormatter.Binder", Justification = "It's fine this is just test data")]
  [SuppressMessage("Security", "CA2300:Do not use insecure deserializer BinaryFormatter", Justification = "It's fine this is just test data")]
  public sealed class UnexpectedResponseMediaTypeExceptionTest
  {
    [Test]
    public void UnexpectedResponseMediaTypeException_Can_Be_Serialized_And_Deserialized_When_ResponseMediaType_Is_Not_Null()
    {
      // ARRANGE
      const string innerExceptionMessage = "You attempted an invalid operation";
      var innerException = new InvalidOperationException(innerExceptionMessage);

      const string message = "The API returned text/html response media type instead of application/json";
      const string responseMediaType = "text/html";
      var target = new UnexpectedResponseMediaTypeException(message, innerException)
      {
        ResponseMediaType = responseMediaType
      };

      // ACT
      var formatter = new BinaryFormatter();

      object? result = null;

      using (var stream = new MemoryStream())
      {
        formatter.Serialize(stream, target);

        stream.Seek(0, SeekOrigin.Begin);
        result = formatter.Deserialize(stream);
      }

      // ASSERT
      Assert.IsNotNull(result);
      Assert.IsInstanceOf<UnexpectedResponseMediaTypeException>(result);

      var exception = (UnexpectedResponseMediaTypeException)result;
      Assert.AreEqual(message, exception.Message);

      Assert.IsNotNull(exception.InnerException);
      Assert.IsInstanceOf<InvalidOperationException>(exception.InnerException);
      Assert.AreEqual(innerExceptionMessage, exception.InnerException!.Message);

      Assert.AreEqual(responseMediaType, exception.ResponseMediaType);
    }

    [Test]
    public void UnexpectedResponseMediaTypeException_Can_Be_Serialized_And_Deserialized_When_ResponseMediaType_Is_Null()
    {
      // ARRANGE
      const string innerExceptionMessage = "You attempted an invalid operation";
      var innerException = new InvalidOperationException(innerExceptionMessage);

      const string message = "The Content-Type HTTP response header is missing";
      
      var target = new UnexpectedResponseMediaTypeException(message, innerException)
      {
        ResponseMediaType = null
      };

      // ACT
      var formatter = new BinaryFormatter();

      object? result = null;

      using (var stream = new MemoryStream())
      {
        formatter.Serialize(stream, target);

        stream.Seek(0, SeekOrigin.Begin);
        result = formatter.Deserialize(stream);
      }

      // ASSERT
      Assert.IsNotNull(result);
      Assert.IsInstanceOf<UnexpectedResponseMediaTypeException>(result);

      var exception = (UnexpectedResponseMediaTypeException)result;
      Assert.AreEqual(message, exception.Message);

      Assert.IsNotNull(exception.InnerException);
      Assert.IsInstanceOf<InvalidOperationException>(exception.InnerException);
      Assert.AreEqual(innerExceptionMessage, exception.InnerException!.Message);

      Assert.IsNull(exception.ResponseMediaType);
    }
  }
}
