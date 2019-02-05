namespace Menees
{
	#region Using Directives

	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Diagnostics.CodeAnalysis;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Text;
	using Menees.Diagnostics;

	#endregion

	/// <summary>
	/// Provides several methods for checking preconditions and postconditions
	/// (i.e., design-by-contract).
	/// </summary>
	public static class Conditions
	{
		#region Public Methods

		/// <summary>
		/// Requires that the argument's state is valid (i.e., true).
		/// </summary>
		/// <param name="argState">The argument's state.</param>
		/// <param name="explanation">The name of the arg to put in the exception.</param>
		/// <exception cref="ArgumentException">If <paramref name="argState"/> is false.</exception>
		public static void RequireArgument(bool argState, string explanation)
		{
			RequireArgument(argState, explanation, (string)null);
		}

		/// <summary>
		/// Requires that the named argument's state is valid (i.e., true).
		/// </summary>
		/// <param name="argState">The argument's state.</param>
		/// <param name="explanation">The explanation to put in the exception.</param>
		/// <param name="argName">The name of the arg to put in the exception.</param>
		/// <exception cref="ArgumentException">If <paramref name="argState"/> is false.</exception>
		public static void RequireArgument(bool argState, string explanation, string argName)
		{
			if (!argState)
			{
				throw Exceptions.NewArgumentException(explanation, argName);
			}
		}

		/// <summary>
		/// Requires that the named argument's state is valid (i.e., true).
		/// </summary>
		/// <param name="argState">The argument's state.</param>
		/// <param name="explanation">The explanation to put in the exception.</param>
		/// <param name="argExpression">An expression that returns the arg, which will be used to get the arg name.</param>
		/// <exception cref="ArgumentException">If <paramref name="argState"/> is false.</exception>
		[SuppressMessage(
			"Microsoft.Design",
			"CA1006:DoNotNestGenericTypesInMemberSignatures",
			Justification = "The expression argument is built by the C# compiler.")]
		public static void RequireArgument<T>(bool argState, string explanation, Expression<Func<T>> argExpression)
		{
			if (!argState)
			{
				string argName = argExpression != null ? ReflectionUtility.GetNameOf(argExpression) : null;
				throw Exceptions.NewArgumentException(explanation, argName);
			}
		}

		/// <summary>
		/// Requires that a reference is non-null.
		/// </summary>
		/// <param name="reference">The reference to check.</param>
		/// <param name="argName">The arg name to put in the exception.</param>
		/// <exception cref="ArgumentNullException">If <paramref name="reference"/> is null.</exception>
		public static void RequireReference<T>(T reference, string argName)
			where T : class
		{
			if (reference == null)
			{
				throw Exceptions.NewArgumentNullException(argName);
			}
		}

		/// <summary>
		/// Requires that a reference is non-null.
		/// </summary>
		/// <param name="reference">The reference to check.</param>
		/// <param name="argExpression">An expression that returns the arg, which will be used to get the arg name.</param>
		/// <exception cref="ArgumentNullException">If <paramref name="reference"/> is null.</exception>
		[SuppressMessage(
			"Microsoft.Design",
			"CA1006:DoNotNestGenericTypesInMemberSignatures",
			Justification = "The expression argument is built by the C# compiler.")]
		public static void RequireReference<T>(T reference, Expression<Func<T>> argExpression)
			where T : class
		{
			if (reference == null)
			{
				string argName = argExpression != null ? ReflectionUtility.GetNameOf(argExpression) : null;
				throw Exceptions.NewArgumentNullException(argName);
			}
		}

		/// <summary>
		/// Requires that the given state is valid (i.e., true).
		/// </summary>
		/// <param name="state">The state to check.</param>
		/// <param name="explanation">The explanation to put in the exception.</param>
		/// <exception cref="InvalidOperationException">If <paramref name="state"/> is false.</exception>
		public static void RequireState(bool state, string explanation)
		{
			if (!state)
			{
				throw Exceptions.NewInvalidOperationException(explanation);
			}
		}

		/// <summary>
		/// Requires that a string is non-null and non-empty.
		/// </summary>
		/// <param name="arg">The string to check.</param>
		/// <param name="argName">The name of the arg to put in the exception.</param>
		/// <exception cref="ArgumentException">If <paramref name="arg"/> is null or empty.</exception>
		public static void RequireString(string arg, string argName)
		{
			RequireArgument(!string.IsNullOrEmpty(arg), "The string must be non-empty.", argName);
		}

		/// <summary>
		/// Requires that a string is non-null and non-empty.
		/// </summary>
		/// <param name="arg">The string to check.</param>
		/// <param name="argExpression">An expression that returns the arg, which will be used to get the arg name.</param>
		/// <exception cref="ArgumentException">If <paramref name="arg"/> is null or empty.</exception>
		[SuppressMessage(
			"Microsoft.Design",
			"CA1006:DoNotNestGenericTypesInMemberSignatures",
			Justification = "The expression argument is built by the C# compiler.")]
		public static void RequireString(string arg, Expression<Func<string>> argExpression)
		{
			RequireArgument(!string.IsNullOrEmpty(arg), "The string must be non-empty.", argExpression);
		}

		#endregion
	}
}
