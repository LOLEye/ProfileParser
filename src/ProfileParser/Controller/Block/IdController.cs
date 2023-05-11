using ProfileParser.Controller.Single;
using ProfileParser.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ProfileParser.HttpPost.Client;
using static ProfileParser.Server;

namespace ProfileParser.Controller.Block
{
    internal class IdController : BlockController
    {
        public override BlockKeyword BlockTag => BlockKeyword.Id;

        public override Token Execute(MemoryStream ms)
        {
            profileData = ms.ToArray();

            Token token = new Token();
            token.TokenType = TokenType.IdBlock;

            Id id = new();
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
                    case 'p':   // 解析parameter
                        {
                            byte[] parameter = new byte[9];
                            Array.Copy(profileData, ms.Position - 1, parameter, 0, 9);
                            if (Encoding.UTF8.GetString(parameter) == "parameter")
                            {
                                ms.Position += 8;
                                int index = 0;
                                List<byte> data_print = new List<byte>();
                                while (index != ';')
                                {
                                    index = ms.ReadByte();
                                    if (index == '\n') { throw new Exception("parameter parse miss ;"); }
                                    data_print.Add(Convert.ToByte(index));
                                }
                                id.Name = Encoding.UTF8.GetString(data_print.ToArray()).TrimEnd(';').Replace("\"","");
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
            token.Value = id;
            token.ValueUTF8 = Encoding.UTF8.GetString(data.ToArray());

            return token;
        }
    }
}
