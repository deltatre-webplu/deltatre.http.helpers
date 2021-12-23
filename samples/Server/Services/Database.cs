using Server.Models;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Server.Services
{
  public static class Database
  {
    private static readonly Student bob;
    private static readonly Student alice;
    private static readonly Student jack;

    public static ImmutableDictionary<Guid, Student> Students { get; }

    static Database()
    {
      bob = new Student(
        Guid.NewGuid(),
        "Bob",
        34,
        "Italy",
        true,
        45.56);

      alice = new Student(
          Guid.NewGuid(),
          "Alice",
          25,
          "France",
          true,
          34.98);

      jack = new Student(
        Guid.NewGuid(),
        "Jack",
        49,
        "Germany",
        false,
        76.78);

      Students =
        new Dictionary<Guid, Student>
        {
          { bob.Id, bob },
          { alice.Id, alice },
          { jack.Id, jack }
        }.ToImmutableDictionary();
    }
  }
}
