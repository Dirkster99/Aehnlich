namespace Menees
{
	#region Using Directives

	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using System.Text;

	#endregion

	/// <summary>
	/// Provides a way to easily invoke a clean-up method from a using statement.
	/// </summary>
	/// <remarks>
	/// This was inspired by a similar class in Jeffrey Richter's PowerThreading library.
	/// </remarks>
	public sealed class Disposer : IDisposable
	{
		#region Private Data Members

		private Action disposeMethod;

		#endregion

		#region Constructors

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <remarks>
		/// See the <see cref="Disposer"/> class comments for an example of using an anonymous
		/// dispose method.
		/// </remarks>
		/// <param name="disposeMethod">The method to invoke during disposal.</param>
		public Disposer(Action disposeMethod)
		{
			Conditions.RequireReference(disposeMethod, "disposeMethod");
			this.disposeMethod = disposeMethod;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets whether <see cref="Dispose"/> has already been called on this instance.
		/// </summary>
		public bool IsDisposed
		{
			get
			{
				bool result = this.disposeMethod == null;
				return result;
			}
		}

		#endregion

		#region IDisposable Members

		/// <summary>
		/// Calls the dispose method that was passed to the constructor.
		/// </summary>
		public void Dispose()
		{
			if (this.disposeMethod != null)
			{
				this.disposeMethod();
				this.disposeMethod = null;
			}
		}

		#endregion
	}
}
