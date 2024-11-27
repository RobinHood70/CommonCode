// This namespace houses common elements for all my projects.
namespace RobinHood70.CommonCode;

using System;

#region Public Enumerations

/// <summary>Represents a tristate filter, where the options are to show everything, show only the selected items, or hide the selected items.</summary>
/// <remarks>This is an alias for Tristate, but is much clearer in its intent than having True/False/Unknown values.</remarks>
public enum Filter
{
	/// <summary>Include all results; do not filter.</summary>
	Any,

	/// <summary>Include only these results. Note that in cases where.</summary>
	Only,

	/// <summary>Exclude these results.</summary>
	Exclude,
}

/// <summary>Represents a binary value which also allows for an unknown state.</summary>
/// <remarks>This is used in preference to a <see cref="Nullable{Boolean}" /> due to the fact that it is both smaller and makes the code much clearer.</remarks>
public enum Tristate
{
	/// <summary>An unknown or unassigned value.</summary>
	Unknown = 0,

	/// <summary>The value is True.</summary>
	True,

	/// <summary>The value is false.</summary>
	False
}
#endregion