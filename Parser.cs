using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etilic.Text
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Parser<T>
    {
        

        #region Instance members
        private IParsable<T> input;
        #endregion

        public IParsable<T> Input
        {
            get { return this.input; }
        }

        public Parser(IParsable<T> input)
        {
            this.input = input;
        }

        protected ParseResult Success()
        {
            return new ParseResult(true);
        }

        protected ParseResult Failure()
        {
            return new ParseResult(false);
        }

        #region Try
        /// <summary>
        /// Applies <paramref name="parser"/>. If <paramref name="parser"/> is successful,
        /// its result is returned; otherwise, the input is reset to the state it was in
        /// prior to executing the parser. 
        /// </summary>
        /// <typeparam name="A"></typeparam>
        /// <param name="parser"></param>
        /// <returns></returns>
        protected ParseResult<A> Try<A>(Func<ParseResult<A>> parser)
        {
            using (IRestorePoint restorePoint = this.input.CreateRestorePoint())
            {
                ParseResult<A> result = parser();

                if (result.Success)
                {
                    return result;
                }
            }

            return new ParseResult<A>();
        }
        #endregion

        #region Token
        /// <summary>
        /// Parses the next token from the input.
        /// </summary>
        /// <returns>
        /// Returns the next token with its position if successful; otherwise, nothing.
        /// </returns>
        protected ParseResult<T> Token()
        {
            // return failure if we have reached the end of the input
            if (this.input.EndOfInput)
                return new ParseResult<T>();

            // otherwise, consume the token and return it
            var token = this.input.NextToken();
            return new ParseResult<T>(token);
        }
        /// <summary>
        /// Parses the next token from the input if it matches a given predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>
        /// Returns the next token with its position if available and the predicate is applied successfully; otherwise, nothing.
        /// </returns>
        protected ParseResult<T> Token(Func<T,Boolean> predicate)
        {
            // return failure if we have reached the end of the input
            if (this.input.EndOfInput)
                return new ParseResult<T>();

            // otherwise, peek at the next token and apply the predicate to it
            if (!predicate(this.input.PeekToken().Value))
                return new ParseResult<T>();

            return new ParseResult<T>(this.input.NextToken());
        }

        protected ParseResult<T> Token(T value) 
        {
            return this.Token(x => x.Equals(value));
        }
        #endregion

        protected ParseResult<T> Peek()
        {
            // return failure if we have reached the end of the input
            if (this.input.EndOfInput)
                return new ParseResult<T>();

            return new ParseResult<T>(this.input.PeekToken());
        }

        protected ParseResult<A> Or<A>(params Func<ParseResult<A>>[] parsers)
        {
            foreach(var parser in parsers)
            {
                var result = parser();

                if (result.Success)
                    return result;
            }

            return new ParseResult<A>();
        }

        protected ParseResult<ICollection<Located<A>>> Sequence<A>(params Func<ParseResult<A>>[] parsers)
        {
            Position pos = this.input.CurrentPosition;
            List<Located<A>> results = new List<Located<A>>();

            foreach (var parser in parsers)
            {
                var result = parser();

                if (!result.Success)
                    return new ParseResult<ICollection<Located<A>>>();

                results.Add(result.Result);
            }

            return new ParseResult<ICollection<Located<A>>>(results, pos);
        }

        /// <summary>
        /// Parses tokens from the input until the predicate succeeds.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        protected ParseResult<ICollection<Located<T>>> While(Func<T, Boolean> predicate)
        {
            Position pos = this.input.CurrentPosition;
            List<Located<T>> results = new List<Located<T>>();

            ParseResult<T> result;

            do
            {
                result = this.Token(x => predicate(x));

                if(result.Success)
                {
                    results.Add(result.Result);
                }
            } while (result.Success);

            return new ParseResult<ICollection<Located<T>>>(
                new Located<ICollection<Located<T>>>(results, pos));
        }

        protected ParseResult<ICollection<Located<T>>> Until(Func<T, Boolean> predicate)
        {
            return this.While(x => !predicate(x));
        }

        protected ParseResult<ICollection<Located<T>>> Until(T value)
        {
            return this.Until(x => x.Equals(value));
        }

        protected ParseResult<ICollection<Located<A>>> Many<A>(Func<ParseResult<A>> parser)
        {
            Position pos = this.Input.CurrentPosition;
            List<Located<A>> results = new List<Located<A>>();

            ParseResult<A> result;

            do
            {
                result = parser();

                if(result.Success)
                {
                    results.Add(result.Result);
                }
            } while (result.Success);

            return new ParseResult<ICollection<Located<A>>>(
                new Located<ICollection<Located<A>>>(results, pos));
        }

        protected ParseResult Many(Func<ParseResult> parser)
        {
            Position pos = this.Input.CurrentPosition;

            ParseResult result;

            do
            {
                result = parser();
            } while (result.Success);

            return new ParseResult(true);
        }

        #region SepBy
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="A"></typeparam>
        /// <param name="parser"></param>
        /// <param name="sep"></param>
        /// <returns></returns>
        protected ParseResult<IEnumerable<Located<A>>> SepBy<A>(Func<ParseResult<A>> parser, Func<ParseResult> sep)
        {
            Position pos = this.Input.CurrentPosition;
            List<Located<A>> results = new List<Located<A>>();

            ParseResult<A> result;

            do
            {
                if(results.Count > 0)
                {
                    if (!sep().Success)
                        break;
                }

                result = parser();

                if (result.Success)
                {
                    results.Add(result.Result);
                }
            } while (result.Success);

            return new ParseResult<IEnumerable<Located<A>>>(
                new Located<IEnumerable<Located<A>>>(results, pos));
        }
        #endregion

        #region Between
        /// <summary>
        /// Applies <paramref name="open"/>, <paramref name="parser"/>, and then <paramref name="close" />.
        /// </summary>
        /// <typeparam name="A">The result type.</typeparam>
        /// <param name="open">The first parser to apply.</param>
        /// <param name="close">The last parser to apply.</param>
        /// <param name="parser">The second parser to apply.</param>
        /// <returns>
        /// The result of <paramref name="parser"/> if all three parsers are successful; 
        /// otherwise, a result indicating failure.
        /// </returns>
        protected ParseResult<A> Between<A>(
            Func<ParseResult> open, 
            Func<ParseResult> close, 
            Func<ParseResult<A>> parser)
        {
            return open().Then(() => parser().Then(x => close().Return(x)));
        }
        /// <summary>
        /// Applies <paramref name="open"/>, <paramref name="parser"/>, and then <paramref name="close" />.
        /// </summary>
        /// <param name="open">The first parser to apply.</param>
        /// <param name="close">The last parser to apply.</param>
        /// <param name="parser">The second parser to apply.</param>
        /// <returns>
        /// A result indicating whether all three parsers were successful.
        /// </returns>
        protected ParseResult Between(
            Func<ParseResult> open,
            Func<ParseResult> close,
            Func<ParseResult> parser)
        {
            return open().Then(() => parser().Then(close));
        }
        #endregion
    }
}
