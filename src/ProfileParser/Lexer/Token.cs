using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProfileParser.Lexer
{
    public struct Token
    {
        public int Line { get; set; }

        public int Column { get; set; }

        public int ErrorColumn { get; set; }

        public int EndColumnSize { get; set; }

        public TokenType TokenType { get; set; }

        public object Value { get; set; }

        public string ValueUTF8 { get; set; }
    }
}
