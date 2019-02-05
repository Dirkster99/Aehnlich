namespace Menees.Diagnostics
{
	#region Using Directives

	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using System.Reflection;
	using System.Security;
	using System.Text;

	#endregion

	[TransparentLogSource]
	internal static class StackTraceHelper
	{
		#region Public Methods

		/// <summary>
		/// Searches back up the stack to find the first calling type not marked with [TransparentLogSource].
		/// </summary>
		/// <param name="defaultSourceType">The type to return if walking the stack doesn't find a better type.</param>
		/// <returns>The source type.</returns>
		public static Type GetSourceType(Type defaultSourceType)
		{
			Type result = defaultSourceType;

			MethodBase method = FindSourceMethodBase();
			if (method != null)
			{
				Type declaringType = method.DeclaringType;
				if (declaringType != null)
				{
					// In C++/CLI standalone functions don't have a declaring type.
					result = declaringType;
				}
			}

			return result;
		}

		/// <summary>
		/// Gets a string representation of the first method up the stack not marked with [TransparentLogSource].
		/// </summary>
		public static string FindSourceMethod()
		{
			string result = string.Empty;

			MethodBase method = FindSourceMethodBase();
			if (method != null)
			{
				StringBuilder sb = new StringBuilder();
				Append(sb, method);
				result = sb.ToString();
			}

			return result;
		}

		/// <summary>
		/// Gets a string representation of the stack starting at the first method not marked with
		/// [TransparentLogSource].
		/// </summary>
		/// <param name="indentPrefix">A padding string used to indent each stack frame.  This can be empty.</param>
		/// <returns>A string representation of the stack.</returns>
		public static string CaptureSourceStackTrace(string indentPrefix)
		{
			// We can't use Environment.StackTrace because it won't ignore [TransparentLogSource].
			StackFrame[] frames = CaptureSourceStack(true, false);
			StringBuilder sb = new StringBuilder(50 * frames.Length);

			foreach (StackFrame frame in frames)
			{
				sb.Append(indentPrefix);
				Append(sb, frame);
				sb.AppendLine();
			}

			string result = sb.ToString();
			return result;
		}

		/// <summary>
		/// Gets a type's full name in C# format optionally including the generic arguments.
		/// </summary>
		public static string GetFullName(Type type, bool includeGenericArguments)
		{
			// If we have a nested type, then we need to go all the way up to the top-level type.
			Stack<Type> typeStack = new Stack<Type>();
			do
			{
				typeStack.Push(type);
				type = type.DeclaringType;
			}
			while (type != null);
			Debug.Assert(typeStack.Count >= 1, "Type stack must be non-empty.");

			StringBuilder sb = new StringBuilder();

			// Append the root type's namespace, which could be empty.
			type = typeStack.Pop();
			sb.Append(type.Namespace);

			// Append each type's name.
			do
			{
				if (sb.Length > 0)
				{
					sb.Append('.');
				}

				string name = type.Name;
				if (type.IsGenericType)
				{
					// In Reflection, generic type names contain a tick mark and the number of generic parameters.
					// I'm stripping the tick marked portion off because it isn't there in C# declarations or usage.
					int tickPos = name.IndexOf('`');
					if (tickPos >= 0)
					{
						name = name.Substring(0, tickPos);
					}

					sb.Append(name);

					if (includeGenericArguments)
					{
						AppendGenericArguments(sb, type.GetGenericArguments());
					}
				}
				else
				{
					sb.Append(name);
				}

				// Get the next type off the stack.
				type = typeStack.Count > 0 ? typeStack.Pop() : null;
			}
			while (type != null);

			return sb.ToString();
		}

		#endregion

		#region Private Methods For Stack Walking

		/// <summary>
		/// Searches back up the stack to find the first calling method not marked with [TransparentLogSource].
		/// </summary>
		/// <returns>The source method or null if the stack couldn't be walked far enough.</returns>
		private static MethodBase FindSourceMethodBase()
		{
			MethodBase result = null;

			StackFrame[] frames = CaptureSourceStack(false, true);
			if (frames.Length > 0)
			{
				StackFrame frame = frames[0];
				result = frame.GetMethod();
			}

			return result;
		}

		private static StackFrame[] CaptureSourceStack(bool needFileInfo, bool topFrameOnly)
		{
			StackTrace st = new StackTrace(1, needFileInfo);
			int frameCount = st.FrameCount;
			int sourceStartIndex = frameCount;

			for (int i = 0; i < frameCount; i++)
			{
				StackFrame frame = st.GetFrame(i);
				if (frame != null)
				{
					MethodBase method = frame.GetMethod();
					if (method != null)
					{
						Type currentType = method.DeclaringType;
						if (currentType != null)
						{
							object[] attributes = currentType.GetCustomAttributes(typeof(TransparentLogSourceAttribute), false);
							if (attributes.Length == 0)
							{
								sourceStartIndex = i;
								break;
							}
						}
						else
						{
							// In C++/CLI standalone functions don't have a declaring type.
							sourceStartIndex = i;
							break;
						}
					}
				}
			}

			int sourceFrameCount = frameCount - sourceStartIndex;
			if (topFrameOnly && sourceFrameCount > 0)
			{
				sourceFrameCount = 1;
			}

			StackFrame[] result = new StackFrame[sourceFrameCount];
			if (sourceFrameCount > 0)
			{
				StackFrame[] allFrames = st.GetFrames();
				Array.Copy(allFrames, sourceStartIndex, result, 0, sourceFrameCount);
			}

			return result;
		}

		#endregion

		#region Private Methods For String Building

		private static void Append(StringBuilder sb, StackFrame frame)
		{
			// Most of this code was ripped off from System.Diagnostics.StackTrace.ToString()
			// because StackFrame.ToString() returns horrible output.
			MethodBase baseMethod = frame.GetMethod();
			if (baseMethod != null)
			{
				sb.Append("at ");

				Append(sb, baseMethod);

				// Append the debug information if we can get it.
				if (frame.GetILOffset() != -1)
				{
					string fileName = null;
					try
					{
						fileName = frame.GetFileName();
					}
					catch (SecurityException)
					{
						// Pulling the file name requires FileIOPermission, which the caller may not have.
						// Since file information is optional and not available for many methods anyway,
						// it's better for us to eat the security exception than to blow up while we're
						// trying to get the stack trace probably for another exception.
					}

					if (fileName != null)
					{
						sb.Append(" in ").Append(fileName);

						int lineNumber = frame.GetFileLineNumber();
						if (lineNumber > 0)
						{
							sb.Append(":line ").Append(lineNumber);
						}
					}
				}
			}
		}

		private static void Append(StringBuilder sb, MethodBase baseMethod)
		{
			// Most of this code was ripped off from System.Diagnostics.StackTrace.ToString()
			// using Reflector, but I've made a few minor tweaks notated in the comments.

			// Append the type and method names.
			Type type = baseMethod.DeclaringType;
			if (type != null)
			{
				sb.Append(GetFullName(type, true));
				sb.Append(".");
			}

			sb.Append(baseMethod.Name);

			// Append any generic method arguments.
			MethodInfo baseInfo = baseMethod as MethodInfo;
			if (baseInfo != null && baseInfo.IsGenericMethod)
			{
				// Like <T> for Array.Resize<T>.
				AppendGenericArguments(sb, baseInfo.GetGenericArguments());
			}

			// Append the method parameters.
			sb.Append("(");
			ParameterInfo[] methodArgs = baseMethod.GetParameters();
			for (int i = 0; i < methodArgs.Length; i++)
			{
				if (i > 0)
				{
					sb.Append(", ");
				}

				ParameterInfo arg = methodArgs[i];
				string argTypeName = "<UnknownType>";
				Type argType = arg.ParameterType;
				if (argType != null)
				{
					argTypeName = argType.Name;
				}

				sb.Append(argTypeName + " " + arg.Name);
			}

			sb.Append(")");
		}

		private static void AppendGenericArguments(StringBuilder sb, Type[] genericArgs)
		{
			// StackTrace.ToString uses [T] delimiters here rather than <T>.
			// I'll use <T> to be more C#-like.
			sb.Append("<");
			for (int i = 0; i < genericArgs.Length; i++)
			{
				if (i > 0)
				{
					sb.Append(",");
				}

				sb.Append(genericArgs[i].Name);
			}

			sb.Append(">");
		}

		#endregion
	}
}
