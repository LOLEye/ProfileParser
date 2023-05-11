using ProfileParser.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ProfileParser.Server;

namespace ProfileParser.Controller.Block
{
    internal class ServerController : BlockController
    {
        public override BlockKeyword BlockTag => BlockKeyword.Server;

        public override Token Execute(MemoryStream ms)
        {
            profileData = ms.ToArray();

            Token token = new Token();
            token.TokenType = TokenType.ServerBlock;

            Server server = new();
            int item = 0;
            List<byte> data = new List<byte>();
            List<HeaderContent> headerContents = new();
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
                    // header "key" "value";
                    case 'h':   // 解析 header
                        {
                            byte[] header = new byte[6];
                            Array.Copy(profileData, ms.Position - 1, header, 0, 6);
                            if (Encoding.UTF8.GetString(header) == "header")
                            {
                                ms.Position += 5;
                                int index = 0;
                                List<byte> data_print = new List<byte>();
                                while (index != ';')
                                {
                                    index = ms.ReadByte();
                                    if (index == '\n') { throw new Exception("header parse miss ;"); }
                                    data_print.Add(Convert.ToByte(index));
                                }
                                string[] headers = Encoding.UTF8.GetString(data_print.ToArray()).Trim(' ').Split(' ');

                                headerContents.Add(new HeaderContent()
                                {
                                    HeaderKey = headers[0].Replace("\"", ""),
                                    HeaderValue = headers[1].Replace("\"", "").TrimEnd(';')
                                });
                            }
                        }
                        break;
                    // output block
                    case 'o':
                        {
                            byte[] output = new byte[6];
                            Array.Copy(profileData, ms.Position - 1, output, 0, 6);
                            if (Encoding.UTF8.GetString(output) == "output")
                            {
                                ms.Position += 5;
                                server.output = new OutputController().Execute(ms).Value as Output;
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
            server.Header = headerContents.ToArray();

            token.Value = server;
            token.ValueUTF8 = Encoding.UTF8.GetString(data.ToArray());

            return token;
        }
    }
}
