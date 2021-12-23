# HTTP helpers for .NET core applications
This library contains a set of helpers to simplify working with JSON web apis from **.NET core 3.1 applications**.

To serialize and deserialize JSON we make use of the [Newtonsoft.Json NUGET package](https://www.nuget.org/packages/Newtonsoft.Json/). 

Each provided helper is implemented as an extension method for the [HttpClient class](https://docs.microsoft.com/en-us/dotnet/api/system.net.http.httpclient?view=netcore-3.1).

Samples of usage are provided in the `samples` folder.

## GET a response from a JSON web api
If you need to issue a GET request to a JSON web API, you can use the `GetJsonAsync` extension method.

Here is a sample:

```C#
using var httpClient = new HttpClient();

List<Car>? cars = null;

var uri = new Uri("https://foo.example.net/api/cars");

try 
{
  cars = await httpClient.GetJsonAsync<List<Car>>(uri).ConfigureAwait(false);
}
catch (JsonApiRequestException exception) 
{
  Console.Writeline("An error occurred while fetching cars: {0}", exception.Message);
  return;
}

if (cars is null) 
{
  Console.Writeline("No cars data available");
  return;
}

foreach (var car in cars) 
{
  Console.Writeline("Car model: {0} - Year: {1} - Owner: {2}", car.Model, car.Year, car.Owner);
}
```

### Exception handling
All the exceptions thrown by the library derives from the `JsonApiRequestException` base class. So, if you don't need to differentiate 
your exception handling strategy, you can simply catch the `JsonApiRequestException` exception type.

Here is an example:

```C#
try 
{
  cars = await httpClient.GetJsonAsync<List<Car>>(uri).ConfigureAwait(false);
}
catch (JsonApiRequestException exception) 
{
  Console.Writeline("An error occurred while fetching cars: {0}", exception.Message);
  return;
}
```

If you need to, you can provide different exception handlers based on the specif type of error:

```C#
try 
{
  cars = await httpClient.GetJsonAsync<List<Car>>(uri).ConfigureAwait(false);
}
catch (HttpInfrastructureException exception) 
{
  // handle infrastructure-level errors such as missing network connectivity or DNS-level failures
}
catch (HttpRequestTimeoutException exception) 
{
  // handle HTTP request timeout
}
catch (NonSuccessStatusCodeException exception) 
{
  // handle non success response status code.
  // We consider all the status codes outside of the range between 200 and 299 as non success
}
catch (EmptyResponseBodyException exception) 
{
  // you get here if the called endpoint returns an empty response body
  // we consider this an error because you are expecting a non-empty JSON response body instead
}
catch (UnexpectedResponseMediaTypeException exception) 
{
  // you get here if the response media type is other than application/json
}
catch (JsonDeserializationException exception)
{
  // handle errors related with the deserialization of the JSON response content
  // (e.g: the response content contains invalid JSON for any reason)
}
```

### Cancellation support
The `GetJsonAsync` extension method supports cancellation. 
The pattern is the usual cancellation pattern: you simply need to provide a `CancellationToken` instance at the call site.

Here is a sample:

```C#
using var source = new CancellationTokenSource();

source.Cancel();

var token = source.Token;

try 
{
  cars = await httpClient
    .GetJsonAsync<List<Car>>(uri, cancellationToken: token)
    .ConfigureAwait(false);
}
catch (OperationCanceledException exception) 
{
  // handle GET request cancellation here
}
```

**IMPORTANT NOTE**: when a `CancellationToken` is provided to the `GetJsonAsync` extension method and cancellation is requested
by the cancellation token source owner, we do **not** raise the custom `HttpRequestTimeoutException` exception.
The `HttpRequestTimeoutException` is raised **only when** the HTTP request times out, because the invoked web service is slow. 
In order to set the timeout of the `HttpClient` instance the [`HttpClient.Timeout` property](https://docs.microsoft.com/en-us/dotnet/api/system.net.http.httpclient.timeout?view=netcore-3.1) must be used.

### Customize JSON deserialization process
The `GetJsonAsync` extension method allows the caller to customize the JSON deserialization by providing an instance
of the [JsonSerializerSettings class](https://www.newtonsoft.com/json/help/html/T_Newtonsoft_Json_JsonSerializerSettings.htm).

You can refer to the [Newtonsoft.Json documentation](https://www.newtonsoft.com/json/help/html/Introduction.htm) to get more information about how to customize
the serialization and deserialization process.
