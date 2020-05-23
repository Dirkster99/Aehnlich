namespace AehnlichViewModelsLib.ViewModels
{
	using System.Diagnostics;

	/// <summary>
	/// Defines the public properties and methods of the diff view position object
	/// which can be used to locate a location in text (via columns and lines).
	/// </summary>
	[DebuggerDisplay("Line = {Line}, Column = {Column}")]
	internal class DiffViewPosition : Base.ViewModelBase, IDiffViewPosition
	{
		#region Public Fields
		public static readonly DiffViewPosition Empty = new DiffViewPosition(-100000, -100000);
		private int _Line;
		private int _Column;
		#endregion

		#region Constructors
		/// <summary>
		/// Class constructor
		/// </summary>
		/// <param name="line"></param>
		/// <param name="column"></param>
		public DiffViewPosition(int line, int column)
			: this()
		{
			this.Line = line;
			this.Column = column;
		}

		/// <summary>
		/// Hidden standard ctor
		/// </summary>
		protected DiffViewPosition()
		{
		}
		#endregion

		#region Public Properties
		/// <summary>
		/// Gets/sets the column of a display position.
		/// </summary>
		public int Column
		{
			get
			{
				return _Column;
			}

			set
			{
				if (_Column != value)
				{
					_Column = value;
					NotifyPropertyChanged(() => Column);
				}
			}
		}

		/// <summary>
		/// Gets/sets the line of a display position.
		/// </summary>
		public int Line
		{
			get
			{
				return _Line;
			}

			set
			{
				if (_Line != value)
				{
					_Line = value;
					NotifyPropertyChanged(() => Line);
				}
			}
		}
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
		/// <summary>
		/// Gets a number equal zero if both positions refer to the same location.
		/// Returns otherwise a non-zero integer value.
		/// </summary>
		/// <param name="position"></param>
		/// <returns></returns>
		public int CompareTo(IDiffViewPosition position)
		{
			int result = this.Line - position.Line;

			if (result == 0)
			{
				result = this.Column - position.Column;
			}

			return result;
		}

		/// <summary>
		/// Gets whether a the positions is equal to this position or not.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public bool Equals(IDiffViewPosition value)
		{
			if (value == null)
				return false;

			if (value.Column != this.Column || value.Line != this.Line)
				return false;

			return true;
		}

		/// <summary>
		/// Gets whether a the positions is equal to this position or not.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			var other = obj as DiffViewPosition;

			return Equals(other);
		}

		/// <summary>
		/// Serves as the default hash function.
		/// </summary>
		/// <returns>A hash code for the current object.</returns>
		public override int GetHashCode()
		{
			base.GetHashCode();
			int result = unchecked((this.Line << 16) + this.Column);
			return result;
		}

		internal void SetPosition(int line, int column)
		{
			Line = line;

		}
		#endregion
	}
}