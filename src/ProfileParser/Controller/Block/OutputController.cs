using ProfileParser.Controller.Single;
using ProfileParser.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ProfileParser.Server;

namespace ProfileParser.Controller.Block
{
    internal class OutputController : BlockController
    {
        public override BlockKeyword BlockTag => BlockKeyword.Output;

        public override Token Execute(MemoryStream ms)
        {
            profileData = ms.ToArray();

            Token token = new Token();
            token.TokenType = TokenType.OutputBlock;

            Output output = new();
            int item = 0;
            List<byte> data = new List<byte>();
            List<string> contents = new();
            while (item != '}')
            {
                item = ms.ReadByte();
                switch (item)
                {
                    case '#':
                        {
                            ParseComment(ms);
                        }
                        break;
                    case 'p':   // 解析 print
                        {
                            byte[] print = new byte[5];
                            Array.Copy(profileData, ms.Position - 1, print, 0, 5);
                            if (Encoding.UTF8.GetString(print) == "print")
                            {
                                ms.Position += 4;
                                int index = 0;
                                List<byte> data_print = new List<byte>();
                                while (index != ';')
                                {
                                    index = ms.ReadByte();
                                    if (index == '\n') { throw new Exception("print parse miss ;"); }
                                    data_print.Add(Convert.ToByte(index));
                                }
                                output.Response = Encoding.UTF8.GetString(data_print.ToArray()).TrimEnd(';');
                            }
                        }
                        break;
                    default:
                        break;
                }
                data.Add(Convert.ToByte(item));
            }
            token.Column = (int)ms.Position;
            token.Line = GetLines(token.Column);
            token.EndColumnSize = data.Count;
            token.Value = output;
            token.ValueUTF8 = Encoding.UTF8.GetString(data.ToArray());

            return token;
        }
    }
}
