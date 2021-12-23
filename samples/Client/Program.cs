using Deltatre.Http.Helpers;
using Deltatre.Http.Helpers.Exceptions;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ViewModels;

namespace Client
{
  public static class Program
  {
    private const string StudentsListUrl = "http://localhost:5000/api/students";

    private static readonly HttpClient _httpClient = new HttpClient();

    public static async Task Main(string[] args)
    {
      var uri = new Uri(StudentsListUrl);

      List<StudentListItemViewModel>? students = null;

      try
      {
        students = await _httpClient.GetJsonAsync<List<StudentListItemViewModel>>(uri).ConfigureAwait(false);
      }
      catch (JsonApiRequestException exception)
      {
        Console.WriteLine("An error occurred while calling {0}: {1}", uri.AbsoluteUri, exception);
        return;
      }

      if (students == null)
      {
        Console.WriteLine("No students have been returned by calling {0}", uri.AbsoluteUri);
        return;
      }

      Console.WriteLine("There are {0} students", students.Count);
      Console.WriteLine();

      foreach (var student in students)
      {
        await TryFetchStudentDetails(student).ConfigureAwait(false);
        Console.WriteLine();
      }
    }

    private static async Task TryFetchStudentDetails(StudentListItemViewModel student)
    {
      Console.WriteLine("Fetching details for student {0}", student.Id);

      StudentViewModel? studentDetails = null;

      try
      {
        studentDetails = await _httpClient
          .GetJsonAsync<StudentViewModel>(new Uri(student.SelfUrl))
          .ConfigureAwait(false);
      }
      catch (JsonApiRequestException exception)
      {
        Console.WriteLine("An error occurred while fetching details for student {0}: {1}", student.Id, exception);
        return;
      }

      if (studentDetails is null)
      {
        Console.WriteLine("No data is available for student {0}", student.Id);
        return;
      }

      Print(studentDetails);

      static void Print(StudentViewModel student)
      {
        Console.WriteLine(
          "Id: {0} - Name: {1} - Age: {2} - Country: {3} - IsActive: {4} - Credits: {5}",
          student.Id,
          student.Name,
          student.Age,
          student.Country,
          student.IsActive,
          student.Credits
        );
      }
    }
  }
}
