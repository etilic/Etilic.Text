using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etilic.Text
{
    /// <summary>
    /// Represents a position in an input source.
    /// </summary>
    public struct Position
    {
        #region Properties
        /// <summary>
        /// Gets the line number.
        /// </summary>
        public Int32 Line
        {
            get;
            private set;
        }
        /// <summary>
        /// Gets the column number.
        /// </summary>
        public Int32 Column
        {
            get;
            private set;
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Constructs a new position from a line and column number.
        /// </summary>
        /// <param name="line">The line number.</param>
        /// <param name="column">The column number.</param>
        public Position(Int32 line, Int32 column) : this()
        {
            this.Line = line;
            this.Column = column;
        }
        #endregion
    }
}
