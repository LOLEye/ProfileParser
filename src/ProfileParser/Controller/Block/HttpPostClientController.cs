using ProfileParser.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ProfileParser.HttpGet.Client;
using static ProfileParser.HttpPost;
using static ProfileParser.HttpPost.Client;

namespace ProfileParser.Controller.Block
{
    internal class HttpPostClientController : BlockController
    {
        public override BlockKeyword BlockTag => BlockKeyword.HttpPostClient;

        public override Token Execute(MemoryStream ms)
        {
            profileData = ms.ToArray();

            Token token = new();
            token.TokenType = TokenType.ClientBlock;

            Client client = new();
            int item = 0;
            List<byte> data = new List<byte>();
            List<string> contents = new();
            List<HeaderContent> headerContents = new();
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
                    // header
                    case 'h':
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

                            client.Header = headerContents.ToArray();
                        }
                        break;
                    // id
                    case 'i':
                        {
                            byte[] id = new byte[2];
                            Array.Copy(profileData, ms.Position - 1, id, 0, 2);
                            if (Encoding.UTF8.GetString(id) == "id")
                            {
                                ms.Position += 1;
                                client.parameter = new IdController().Execute(ms).Value as Client.Id;
                            }
                        }
                        break;
                    // output
                    case 'o':
                        {
                            byte[] output = new byte[6];
                            Array.Copy(profileData, ms.Position - 1, output, 0, 6);
                            if (Encoding.UTF8.GetString(output) == "output")
                            {
                                ms.Position += 5;
                                client.Output = new OutputController().Execute(ms).Value as Server.Output;
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            token.Column = (int)ms.Position;
            token.Line = GetLines(token.Column);
            token.EndColumnSize = data.Count;
            token.Value = client;
            token.ValueUTF8 = Encoding.UTF8.GetString(data.ToArray());

            return token;
        }
    }
}
