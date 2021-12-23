using Deltatre.Http.Helpers.Exceptions;
using NUnit.Framework;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics.CodeAnalysis;

namespace Deltatre.Http.Helpers.UnitTests.Exceptions
{
  [TestFixture]
  public sealed class NonSuccessStatusCodeExceptionTest
  {
    [Test]
    [SuppressMessage("Security", "CA2301:Do not call BinaryFormatter.Deserialize without first setting BinaryFormatter.Binder", Justification = "It's fine this is just test data")]
    [SuppressMessage("Security", "CA2300:Do not use insecure deserializer BinaryFormatter", Justification = "It's fine this is just test data")]
    public void NonSuccessStatusCodeException_Can_Be_Serialized_And_Deserialized() 
    {
      // ARRANGE
      const string innerExceptionMessage = "You attempted an invalid operation";
      var innerException = new InvalidOperationException(innerExceptionMessage);
      
      const string message = "The API returned 504 HTTP status code";
      const int httpStatusCode = 504;
      var target = new NonSuccessStatusCodeException(message, innerException)
      {
        HttpStatusCode = httpStatusCode
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
      Assert.IsInstanceOf<NonSuccessStatusCodeException>(result);

      var exception = (NonSuccessStatusCodeException)result;
      Assert.AreEqual(message, exception.Message);
      
      Assert.IsNotNull(exception.InnerException);
      Assert.IsInstanceOf<InvalidOperationException>(exception.InnerException);
      Assert.AreEqual(innerExceptionMessage, exception.InnerException!.Message);

      Assert.AreEqual(httpStatusCode, exception.HttpStatusCode);
    }
  }
}
