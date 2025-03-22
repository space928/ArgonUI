// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Style", "IDE0017:Simplify object initialization", Justification = "Often more verbose to do it this way.")]
[assembly: SuppressMessage("Style", "IDE0290:Use primary constructor", Justification = "Doesn't always help with code clarity.")]
[assembly: SuppressMessage("Style", "CS1591:Missing XML comment for publicly visible type or memeber", Justification = "Needs to be addressed on a case by case basis.")]
