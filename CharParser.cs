using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etilic.Text
{
    /// <summary>
    /// Provides convenience parser combinators for parsers which
    /// work directly on chars.
    /// </summary>
    public class CharParser : Parser<Char>
    {
        #region Constructor
        public CharParser(IParsable<Char> input)
            : base(input)
        { }
        #endregion

        private ParseResult<String> ToString(ParseResult<ICollection<Located<Char>>> results)
        {
            return (ParseResult<String>)results.Map<String>(result =>
                new String(result.Select(x => x.Value).ToArray()));
        }

        #region Whitespace
        /// <summary>
        /// Parses whitespace. This parser will always succeed.
        /// </summary>
        /// <returns></returns>
        protected ParseResult<String> Whitespace()
        {
            var result = this.While(x =>
                x == ' ' ||
                x == '\t' ||
                x == '\n' ||
                x == '\r');

            return ToString(result);
        }
        #endregion


        protected ParseResult<String> Digits()
        {
            return ToString(While(x => x >= '0' && x <= '9'));
        }

        protected ParseResult<String> LowerCase()
        {
            return ToString(While(x => x >= 'a' && x <= 'z'));
        }

        protected ParseResult<String> UpperCase()
        {
            return ToString(While(x => x >= 'A' && x <= 'Z'));
        }

        protected ParseResult<String> MixedCase()
        {
            return Or(LowerCase, UpperCase);
        }

        protected ParseResult<String> MixedCaseAndDigits()
        {
            return Or(MixedCase, Digits);
        }

        protected ParseResult<String> StrUntil(Func<Char, Boolean> predicate)
        {
            var result = this.Until(predicate);

            return (ParseResult<String>)result.Map<String>(results => 
                new String(results.Select(x => x.Value).ToArray()));
        }

        protected ParseResult<String> StrUntil(params Char[] c)
        {
            return this.StrUntil(x => {
                foreach(Char y in c)
                {
                    if (x == y)
                        return true;
                }

                return false;
            });
        }
    }
}
