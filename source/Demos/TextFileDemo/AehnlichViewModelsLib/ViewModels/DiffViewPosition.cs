namespace AehnlichViewModelsLib.ViewModels
{
    using System;
    using System.Diagnostics;

    [DebuggerDisplay("Line = {Line}, Column = {Column}")]
    public class DiffViewPosition : Base.ViewModelBase, IEquatable<DiffViewPosition>
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

        public int CompareTo(DiffViewPosition position)
        {
            int result = this.Line - position.Line;

            if (result == 0)
            {
                result = this.Column - position.Column;
            }

            return result;
        }

        public bool Equals(DiffViewPosition value)
        {
            if (value == null)
                return false;

            if (value.Column != this.Column || value.Line != this.Line)
                return false;

            return true;
        }

        public override bool Equals(object obj)
        {
            var other = obj as DiffViewPosition;

            return Equals(other);
        }

        public override int GetHashCode()
        {
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