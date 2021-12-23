using Server.Models;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Server.Services
{
  public sealed class StudentRepository: IStudentRepository
  {
    public Task<ImmutableArray<Student>> GetAll(CancellationToken cancellationToken = default)
    {
      var students = Database
        .Students
        .Values
        .OrderBy(student => student.Name)
        .ToImmutableArray();

      return Task.FromResult(students);
    }

    public Task<Student?> GetById(Guid id, CancellationToken cancellationToken = default)
    {
      if (Database.Students.TryGetValue(id, out var student))
      {
        return Task.FromResult<Student?>(student);
      }
      else
      {
        return Task.FromResult<Student?>(default);
      }
    }
  }
}
