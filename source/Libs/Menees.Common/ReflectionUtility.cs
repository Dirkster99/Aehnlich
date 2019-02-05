namespace Menees
{
	#region Using Directives

	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Diagnostics.CodeAnalysis;
	using System.IO;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;
	using System.Runtime.CompilerServices;
	using System.Text;

	#endregion

	/// <summary>
	/// Methods for getting metadata about assemblies, types, members, etc.
	/// </summary>
	public static class ReflectionUtility
	{
		#region Public Methods

		/// <summary>
		/// Gets the assembly's copyright information.
		/// </summary>
		/// <param name="assembly">The assembly to get the copyright from.</param>
		/// <returns>User-friendly copyright information.</returns>
		public static string GetCopyright(Assembly assembly)
		{
			Conditions.RequireReference(assembly, "assembly");

			string result = null;

			var copyrights = (AssemblyCopyrightAttribute[])assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), true);
			if (copyrights.Length > 0)
			{
				result = copyrights[0].Copyright;
			}

			return result;
		}

		/// <summary>
		/// Gets the name of the item returned by <paramref name="expression"/>.
		/// </summary>
		/// <param name="expression">An expression that returns a member, method call, or variable.</param>
		/// <returns>The name of the returned item.</returns>
		[SuppressMessage(
			"Microsoft.Design",
			"CA1011:ConsiderPassingBaseTypesAsParameters",
			Justification = "The base LambdaExpression is too general.  We need a strongly-typed lambda.")]
		[SuppressMessage(
			"Microsoft.Design",
			"CA1006:DoNotNestGenericTypesInMemberSignatures",
			Justification = "The expression argument is built by the C# compiler.")]
		public static string GetNameOf<T>(Expression<Func<T>> expression)
		{
			Conditions.RequireReference(expression, "expression");
			string result = GetNameOf(expression.Body);
			return result;
		}

		/// <summary>
		/// Gets the name of the void method invoked by <paramref name="expression"/>.
		/// </summary>
		/// <param name="expression">An expression that calls a method that returns void.</param>
		/// <returns>The name of the invoked method.</returns>
		[SuppressMessage(
			"Microsoft.Design",
			"CA1011:ConsiderPassingBaseTypesAsParameters",
			Justification = "The base LambdaExpression is too general.  We need a strongly-typed lambda.")]
		public static string GetNameOf(Expression<Action> expression)
		{
			Conditions.RequireReference(expression, "expression");
			string result = GetNameOf(expression.Body);
			return result;
		}

		/// <summary>
		/// Gets the name of type <typeparamref name="T"/>.
		/// </summary>
		/// <typeparam name="T">The type to get the name of.</typeparam>
		/// <returns>The name of the type.</returns>
		[SuppressMessage(
			"Microsoft.Design",
			"CA1004:GenericMethodsShouldProvideTypeParameter",
			Justification = "This method only applies to types.")]
		public static string GetNameOf<T>()
		{
			string result = typeof(T).Name;
			return result;
		}

		/// <summary>
		/// Gets the name of the method or property that calls this method.
		/// </summary>
		/// <param name="callerName">Do not pass a value for this parameter.
		/// The C# compiler will set it automatically.</param>
		/// <returns>The name of the calling method or property.</returns>
		[SuppressMessage(
			"Microsoft.Design",
			"CA1026:DefaultParametersShouldNotBeUsed",
			Justification = "The C# compiler's support for the CallerMemberName attribute requires the use of a parameter with a default.")]
		public static string GetNameOfCaller([CallerMemberName] string callerName = null)
		{
			// This precondition depends on the C# compiler providing the caller's name.
			// If an explicit caller passes in a null, they'll get an error.
			Conditions.RequireString(callerName, "callerName");
			return callerName;
		}

		/// <summary>
		/// Gets the UTC build timestamp from the assembly's IMAGE_FILE_HEADER structure.
		/// </summary>
		/// <param name="assembly">The assembly to get the timestamp from.</param>
		/// <returns>The linker's timestamp as a UTC datetime if the assembly was linked and loaded from disk.
		/// Returns null for a generated, in-memory assembly or an assembly loaded from a byte array.</returns>
		public static DateTime? GetBuildTime(Assembly assembly)
		{
			Conditions.RequireReference(assembly, () => assembly);

			DateTime? result = null;

			// Use Assembly.Location because we want the timestamp from the specified
			// assembly even if it is a shadow copy, and we want the ability to detect
			// in-memory-only assemblies (which we can't if we use the CodeBase).
			string location = assembly.Location;
			if (!string.IsNullOrEmpty(location) && File.Exists(location))
			{
				// Pull the TimeDateStamp from the IMAGE_FILE_HEADER, which is in Unix timestamp format.
				// http://stackoverflow.com/questions/2050396/getting-the-date-of-a-net-assembly/3634544#3634544
				using (FileStream s = File.OpenRead(location))
				{
					const int BufferSize = 2048;
					var buffer = new byte[BufferSize];
					int readCount = s.Read(buffer, 0, buffer.Length);
					const int PeHeaderLocationOffset = 60;
					if (readCount >= PeHeaderLocationOffset + sizeof(uint))
					{
						int headerLocation = BitConverter.ToInt32(buffer, PeHeaderLocationOffset);
						const int LinkerTimestampOffset = 8;
						int timestampLocation = headerLocation + LinkerTimestampOffset;
						if (headerLocation >= 0 && readCount >= timestampLocation + sizeof(uint))
						{
							uint timestamp = BitConverter.ToUInt32(buffer, timestampLocation);
							const int UnixTimeStartYear = 1970;
							result = new System.DateTime(UnixTimeStartYear, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(timestamp);
						}
					}
				}
			}

			return result;
		}

		#endregion

		#region Private Methods

		private static string GetNameOf(Expression expression)
		{
			string result;

			// The motivation and implementation for this came from a couple of sources.  First I saw this:
			// http://ivanz.com/2009/12/04/how-to-avoid-passing-property-names-as-strings-using-c-3-0-expression-trees/
			// And that led me to an implementation I liked better:
			// http://blogs.microsoft.co.il/blogs/arik/archive/2010/11/17/no-more-magic-strings-presenting-string-of.aspx
			switch (expression.NodeType)
			{
				case ExpressionType.MemberAccess:
					// Note: Local variable and parameter expressions come through as MemberAccess instead of Parameter.
					var memberExpression = (MemberExpression)expression;
					var supername = GetNameOf(memberExpression.Expression);

					if (string.IsNullOrEmpty(supername))
					{
						result = memberExpression.Member.Name;
					}
					else
					{
						result = string.Concat(supername, '.', memberExpression.Member.Name);
					}

					break;

				case ExpressionType.Call:
					var callExpression = (MethodCallExpression)expression;
					result = callExpression.Method.Name;
					break;

				case ExpressionType.Convert:
					var unaryExpression = (UnaryExpression)expression;
					result = GetNameOf(unaryExpression.Operand);
					break;

				case ExpressionType.Constant:
				case ExpressionType.Parameter:
					// Note: A "Parameter" type refers to a lambda parameter, which we shouldn't encounter.
					result = string.Empty;
					break;

				default:
					throw Exceptions.NewArgumentExceptionFormat(
						"The expression type {0} is not supported for GetNameOf.", expression.NodeType);
			}

			return result;
		}

		#endregion
	}
}
