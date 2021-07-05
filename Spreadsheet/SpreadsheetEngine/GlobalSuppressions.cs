// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

// Surpressing these two warning messages because the fields in this case should be protected not private.
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "Member should be protected not private", Scope = "member", Target = "~F:CptS321.Cell.text")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "Member should be protected not private", Scope = "member", Target = "~F:CptS321.Cell.value")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "<Member needs to be access by the spreadsheet class>", Scope = "member", Target = "~F:CptS321.ExpressionTree.VariableNames")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "<need to use object>", Scope = "member", Target = "~F:CptS321.Spreadsheet.Invoker")]
