using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etilic.Text
{
    /// <summary>
    /// Represents a value which has a location within an input source
    /// attached to it.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    public class Located<T> 
    {
        #region Instance members
        /// <summary>
        /// The position of the value.
        /// </summary>
        private Position position;
        /// <summary>
        /// The value.
        /// </summary>
        private T value;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the position of the value in the input source.
        /// </summary>
        public Position Position
        {
            get { return this.position; }
        }
        /// <summary>
        /// Gets the value.
        /// </summary>
        public T Value
        {
            get { return this.value; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="position"></param>
        public Located(T value, Position position)
        {
            this.value = value;
            this.position = position;
        }
        #endregion

        public Located<B> Map<B>(Func<T, B> fun)
        {
            return new Located<B>(fun(this.value), this.position);
        }
    }
}
