using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ProfileParser.Misc
{
    internal class ParserException : Exception
    {
        public ParserException()
        {
        }

        public ParserException(string? message) : base(message)
        {
        }

        public ParserException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected ParserException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
