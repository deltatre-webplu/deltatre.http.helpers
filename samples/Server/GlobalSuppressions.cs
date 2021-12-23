// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Performance", "CA1810:Initialize reference type static fields inline", Justification = "Using a static constructor makes this code simpler to understand. See https://stackoverflow.com/questions/1494735/initialization-order-of-static-fields-in-static-class/1494745#1494745", Scope = "member", Target = "~M:Server.Services.Database.#cctor")]
[assembly: SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task", Justification = "No need to do that in ASP.NET core. See https://blog.stephencleary.com/2017/03/aspnetcore-synchronization-context.html", Scope = "module")]
[assembly: SuppressMessage("Design", "RCS1090:Add call to 'ConfigureAwait' (or vice versa).", Justification = "No need to do that in ASP.NET core. See https://blog.stephencleary.com/2017/03/aspnetcore-synchronization-context.html", Scope = "module")]
[assembly: SuppressMessage("Design", "CA1056:URI-like properties should not be strings", Justification = "String is fine. I want to keep it simple.", Scope = "member", Target = "~P:Server.Models.StudentListItemViewModel.SelfUrl")]
