using System;

namespace Server.Models
{
  public sealed class Student
  {
    public Guid Id { get; }
    public string Name { get; }
    public int Age { get; }
    public string Country { get; }
    public bool IsActive { get; }
    public double Credits { get; }

    public Student(
      Guid id,
      string name,
      int age,
      string country,
      bool isActive,
      double credits)
    {
      this.Id = id;
      this.Name = name;
      this.Age = age;
      this.Country = country;
      this.IsActive = isActive;
      this.Credits = credits;
    }
  }
}
