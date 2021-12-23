using Server.Models;
using System;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;

namespace Server.Services
{
  public interface IStudentRepository
  {
    Task<ImmutableArray<Student>> GetAll(CancellationToken cancellationToken = default);
    Task<Student?> GetById(Guid id, CancellationToken cancellationToken = default);
  }
}
