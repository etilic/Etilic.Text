using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etilic.Text
{
    /// <summary>
    /// Represents parse results of type void. Essentially the Maybe monad, specialised over the unit type.
    /// </summary>
    public class ParseResult
    {
        #region Instance members
        /// <summary>
        /// A value indicating whether the computation for which this is the result was successful.
        /// </summary>
        private Boolean success;
        #endregion

        #region Properties
        /// <summary>
        /// Gets a value indicating whether the computation for which this is the result was successful.
        /// </summary>
        public Boolean Success
        {
            get { return this.success; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Constructs a new result for a parser.
        /// </summary>
        /// <param name="success">
        /// A value indicating whether the parser has been successful.
        /// </param>
        public ParseResult(Boolean success)
        {
            this.success = success;
        }
        #endregion

        #region Then
        /// <summary>
        /// If the parser which created this result was successful,
        /// <paramref name="parser"/> will be executed.
        /// </summary>
        /// <typeparam name="B">
        /// The type of the result returned by <paramref name="parser"/>.
        /// </typeparam>
        /// <param name="parser">
        /// The parser to execute if the parser which created this result was
        /// successful.
        /// </param>
        /// <returns>
        /// Returns the result of <paramref name="parser"/> if the parser which
        /// created this result was successful; otherwise a result indicating
        /// failure is returned.
        /// </returns>
        public ParseResult<B> Then<B>(Func<ParseResult<B>> parser)
        {
            if (!this.success)
                return new ParseResult<B>();

            return parser();
        }
        /// <summary>
        /// If the parser which created this result was successful,
        /// <paramref name="parser"/> will be executed.
        /// </summary>
        /// <param name="parser">
        /// The parser to execute if the parser which created this result was
        /// successful.
        /// </param>
        /// <returns>
        /// Returns a result indicating whether <paramref name="parser"/> was successful
        /// if the parser which created this result was successful; otherwise a result 
        /// indicating failure is returned.
        /// </returns>
        public ParseResult Then(Func<ParseResult> parser)
        {
            if (!this.success)
                return new ParseResult(false);

            return parser();
        }
        #endregion

        #region Return
        /// <summary>
        /// Returns <paramref name="value"/> wrapped into a ParseResult object.
        /// </summary>
        /// <typeparam name="B"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public ParseResult<B> Return<B>(Located<B> value)
        {
            return new ParseResult<B>(value);
        }
        #endregion
    }

    /// <summary>
    /// Represents parse results. Essentially the Maybe monad.
    /// </summary>
    /// <typeparam name="R"></typeparam>
    public class ParseResult<R> : ParseResult
    {
        #region Instance members
        /// <summary>
        /// The value + position of this result.
        /// </summary>
        private Located<R> value;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the result + position. This is null unless the parser was successful.
        /// </summary>
        public Located<R> Result
        {
            get { return this.value; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Constructs a new result for a failed parser.
        /// </summary>
        public ParseResult()
            : base(false)
        {
        }
        /// <summary>
        /// Constructs a new result for a successful parser.
        /// </summary>
        /// <param name="value">The value that was obtained from parsing.</param>
        public ParseResult(Located<R> value)
            : base(true)
        {
            this.value = value;
        }
        /// <summary>
        /// Constructs a new result for a successful parser.
        /// </summary>
        /// <param name="result">The value that was obtained from parsing.</param>
        /// <param name="position">The position of the value in the input.</param>
        public ParseResult(R result, Position position)
            : this(new Located<R>(result, position))
        { }
        #endregion

        #region Then
        /// <summary>
        /// If the parser which created this result was successful,
        /// <paramref name="parser"/> will be executed.
        /// </summary>
        /// <typeparam name="B">
        /// The type of the result returned by <paramref name="parser"/>.
        /// </typeparam>
        /// <param name="parser">
        /// The parser to execute if the parser which created this result was
        /// successful.
        /// </param>
        /// <returns>
        /// Returns the result of <paramref name="parser"/> if the parser which
        /// created this result was successful; otherwise a result indicating
        /// failure is returned.
        /// </returns>
        public ParseResult<B> Then<B>(Func<Located<R>, ParseResult<B>> parser)
        {
            if (!this.Success)
                return new ParseResult<B>();

            return parser(this.value);
        }
        /// <summary>
        /// If the parser which created this result was successful,
        /// <paramref name="parser"/> will be executed.
        /// </summary>
        /// <param name="parser">
        /// The parser to execute if the parser which created this result was
        /// successful.
        /// </param>
        /// <returns>
        /// Returns a result indicating whether <paramref name="parser"/> was successful
        /// if the parser which created this result was successful; otherwise a result 
        /// indicating failure is returned.
        /// </returns>
        public ParseResult Then(Func<Located<R>, ParseResult> fun)
        {
            if (!this.Success)
                return new ParseResult(false);

            return fun(this.value);
        }
        #endregion

        #region Map
        public ParseResult<B> Map<B>(Func<R, B> f)
        {
            if (!this.Success)
                return new ParseResult<B>();

            return new ParseResult<B>(
                (Located<B>)this.value.Map<B>(f));
        }
        #endregion
    }
}
