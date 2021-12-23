using System;

namespace ViewModels
{
  public sealed class StudentListItemViewModel
  {
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public int Age { get; set; }
    public string SelfUrl { get; set; } = default!;
  }
}
