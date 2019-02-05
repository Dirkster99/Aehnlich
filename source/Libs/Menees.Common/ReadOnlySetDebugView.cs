namespace Menees
{
	#region Using Directives

	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using System.Text;

	#endregion

	internal sealed class ReadOnlySetDebugView<T>
	{
		#region Private Data Members

		private ISet<T> set;

		#endregion

		#region Constructors

		public ReadOnlySetDebugView(ReadOnlySet<T> set)
		{
			if (set == null)
			{
				throw Exceptions.NewArgumentNullException("set");
			}

			this.set = set;
		}

		#endregion

		#region Public Properties

		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		public T[] Items
		{
			get
			{
				T[] array = new T[this.set.Count];
				this.set.CopyTo(array, 0);
				return array;
			}
		}

		#endregion
	}
}
