using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etilic.Text
{
    /// <summary>
    /// Represents an interface for things that can be parsed.
    /// </summary>
    /// <typeparam name="Token"></typeparam>
    public interface IParsable<Token>
    {
        #region Properties
        /// <summary>
        /// If implemented by a deriving class, this property gets the current line number.
        /// </summary>
        Int32 CurrentLine
        {
            get;
        }
        /// <summary>
        /// If implemented by a deriving class, this property gets the current column number.
        /// </summary>
        Int32 CurrentColumn
        {
            get;
        }
        /// <summary>
        /// If implemented by a deriving class, this property gets the current position in the input.
        /// </summary>
        Position CurrentPosition
        {
            get;
        }
        /// <summary>
        /// If implemented by a deriving class, this property gets a value indicating whether
        /// the end of the input has been reached.
        /// </summary>
        Boolean EndOfInput
        {
            get;
        }
        #endregion

        #region NextToken
        /// <summary>
        /// Consumes the next token.
        /// </summary>
        /// <returns>Returns the next token in the input.</returns>
        Located<Token> NextToken();
        /// <summary>
        /// Returns the next token, without consuming it.
        /// </summary>
        /// <returns></returns>
        Located<Token> PeekToken();
        #endregion

        #region CreateRestorePoint
        /// <summary>
        /// Creates an instance of <see cref="Etilic.Text.IRestorePoint"/> which saves the
        /// current state of this object so that it can be restored later.
        /// </summary>
        /// <returns></returns>
        IRestorePoint CreateRestorePoint();
        #endregion
    }
}
