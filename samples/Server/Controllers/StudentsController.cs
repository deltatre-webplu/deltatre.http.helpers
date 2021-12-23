using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Server.Models;
using Server.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ViewModels;

namespace Server.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class StudentsController : ControllerBase
  {
    private readonly IStudentRepository _repository;

    public StudentsController(IStudentRepository repository)
    {
      _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    [HttpGet("")]
    public async Task<IEnumerable<StudentListItemViewModel>> GetAll(CancellationToken cancellationToken)
    {
      var students = await _repository.GetAll(cancellationToken);

      return students
        .Select(ToViewModel)
        .ToList();

      StudentListItemViewModel ToViewModel(Student student)
      {
        if (student is null)
        {
          throw new ArgumentNullException(nameof(student));
        }

        var context = new UrlActionContext
        {
          Action = nameof(this.GetById),
          Controller = "Students",
          Values = new { id = student.Id },
          Host = this.Request.Host.Value,
          Protocol = this.Request.Scheme
        };

        var selfUrl = this.Url.Action(context);

        return new StudentListItemViewModel
        {
          Id = student.Id,
          Name = student.Name,
          Age = student.Age,
          SelfUrl = selfUrl,
        };
      }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<StudentViewModel>> GetById(
      Guid id,
      CancellationToken cancellationToken)
    {
      var student = await _repository.GetById(id, cancellationToken);
      if (student == null) 
      {
        return this.NotFound();
      }

      return FromStudent(student);

      static StudentViewModel FromStudent(Student student)
      {
        if (student is null)
        {
          throw new ArgumentNullException(nameof(student));
        }

        return new StudentViewModel
        {
          Id = student.Id,
          Name = student.Name,
          Age = student.Age,
          Country = student.Country,
          IsActive = student.IsActive,
          Credits = student.Credits,
        };
      }
    }
  }
}
