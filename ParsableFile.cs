using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Etilic.Text
{
    public class ParsableFile : IParsable<Char>
    {
        class FileRestorePoint : IRestorePoint
        {
            private ParsableFile file;
            private Int32 line;
            private Int32 column;
            private Int64 offset;

            public FileRestorePoint(ParsableFile file)
            {
                this.file = file;
                this.line = file.CurrentLine;
                this.column = file.CurrentColumn;
                this.offset = file.CurrentOffset;
            }

            public void Restore()
            {
                this.file.reader.BaseStream.Seek(this.offset, SeekOrigin.Begin);

                this.file.line = this.line;
                this.file.column = this.column;
                this.file.offset = this.offset;
            }

            public void Dispose()
            {
            }
        }

        #region Instance members
        private StreamReader reader;
        private Int32 line = 1;
        private Int32 column = 0;
        private Int64 offset = 0;
        #endregion

        #region Properties
        public int CurrentLine
        {
            get { return this.line; }
        }

        public int CurrentColumn
        {
            get { return this.column; }
        }

        public Int64 CurrentOffset
        {
            get { return this.offset; }
        }
        public Position CurrentPosition
        {
            get { return new Position(this.line, this.column); }
        }
        /// <summary>
        /// Gets a value indicating whether the end of the file has been reached.
        /// </summary>
        public Boolean EndOfInput
        {
            get { return this.reader.Peek() == -1; }
        }
        #endregion

        #region Constructors
        public ParsableFile(StreamReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            this.reader = reader;
        }
        #endregion

        #region NextToken
        /// <summary>
        /// Consumes the next token from the file.
        /// </summary>
        /// <returns></returns>
        public Located<Char> NextToken()
        {
            // make sure we haven't reached the end of the file
            if (this.EndOfInput)
                throw new Exception("Can't read from the input file, because the end of the file has been reached.");

            Position pos = new Position(this.line, this.column);
            Char c = (Char)this.reader.Read();

            this.offset++;

            // if this is a newline character, increment the line number
            // otherwise, increment the column number
            if(c == '\n')
            {
                line++;
                column = 0;
            }
            else
            {
                column++;
            }

            // return the character with its location
            return new Located<Char>(c, pos);
        }
        #endregion


        public Located<Char> PeekToken()
        {
            // make sure we haven't reached the end of the file
            if (this.EndOfInput)
                throw new Exception("Can't read from the input file, because the end of the file has been reached.");

            Position pos = new Position(this.line, this.column);
            Char c = (Char)this.reader.Peek();

            return new Located<Char>(c, pos);
        }


        public IRestorePoint CreateRestorePoint()
        {
            return new FileRestorePoint(this);
        }
    }
}
