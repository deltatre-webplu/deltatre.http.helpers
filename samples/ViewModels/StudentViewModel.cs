using System;

namespace ViewModels
{
  public sealed class StudentViewModel
  {
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public int Age { get; set; }
    public string Country { get; set; } = default!;
    public bool IsActive { get; set; }
    public double Credits { get; set; }
  }
}
