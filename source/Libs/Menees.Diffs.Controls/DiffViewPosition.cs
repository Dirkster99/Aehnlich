namespace Menees.Diffs.Controls
{
	#region Using Directives

	using System;
	using System.Diagnostics;

	#endregion

	[DebuggerDisplay("Line = {Line}, Column = {Column}")]
	internal struct DiffViewPosition
	{
		#region Public Fields

		public static readonly DiffViewPosition Empty = new DiffViewPosition(-100000, -100000);

		#endregion

		#region Constructors

		public DiffViewPosition(int line, int column)
			: this()
		{
			this.Line = line;
			this.Column = column;
		}

		#endregion

		#region Public Properties

		public int Column { get; }

		public int Line { get; }

		#endregion

		#region Public Operators

		public static bool operator !=(DiffViewPosition position1, DiffViewPosition position2)
		{
			bool result = !position1.Equals(position2);
			return result;
		}

		public static bool operator <(DiffViewPosition position1, DiffViewPosition position2)
		{
			bool result = position1.CompareTo(position2) < 0;
			return result;
		}

		public static bool operator <=(DiffViewPosition position1, DiffViewPosition position2)
		{
			bool result = position1.CompareTo(position2) <= 0;
			return result;
		}

		public static bool operator ==(DiffViewPosition position1, DiffViewPosition position2)
		{
			bool result = position1.Equals(position2);
			return result;
		}

		public static bool operator >(DiffViewPosition position1, DiffViewPosition position2)
		{
			bool result = position1.CompareTo(position2) > 0;
			return result;
		}

		public static bool operator >=(DiffViewPosition position1, DiffViewPosition position2)
		{
			bool result = position1.CompareTo(position2) >= 0;
			return result;
		}

		#endregion

		#region Public Methods

		public int CompareTo(DiffViewPosition position)
		{
			int result = this.Line - position.Line;

			if (result == 0)
			{
				result = this.Column - position.Column;
			}

			return result;
		}

		public override bool Equals(object value)
		{
			bool result = false;

			DiffViewPosition? position = value as DiffViewPosition?;
			if (position != null)
			{
				result = this.CompareTo(position.Value) == 0;
			}

			return result;
		}

		public override int GetHashCode()
		{
			int result = unchecked((this.Line << 16) + this.Column);
			return result;
		}

		#endregion
	}
}
