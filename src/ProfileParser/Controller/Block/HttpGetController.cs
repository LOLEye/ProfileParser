using ProfileParser.Controller.Single;
using ProfileParser.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProfileParser.Controller.Block
{
    internal class HttpGetController : BlockController
    {
        public override BlockKeyword BlockTag => BlockKeyword.HttpGet;

        public override Token Execute(MemoryStream ms)
        {
            profileData = ms.ToArray();

            Token token = new Token();
            token.TokenType = TokenType.HttpGetBlock;

            HttpGet httpget = new();
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
                    case 's':   // 解析set
                        {
                            byte[] set = new byte[3];
                            Array.Copy(profileData, ms.Position - 1, set, 0, 3);
                            if (Encoding.UTF8.GetString(set) == "set")
                            {
                                ms.Position += 2;
                                SubController single_set = new SinglePropertyController();
                                var tmp = single_set.Execute(ms);
                                var results = tmp.ValueUTF8.Trim().Split().Where(x => !String.IsNullOrEmpty(x)).ToArray();
                                DynamicProperty.Set(httpget, results, String.Join(' ', results.Skip(1)).TrimEnd(';').Replace("\"","").Split());
                            }

                            byte[] server = new byte[6];
                            Array.Copy(profileData, ms.Position - 1, server, 0, 6);

                            if (Encoding.UTF8.GetString(server) == "server")
                            {
                                SubController single_set = new ServerController();
                                var tmp = single_set.Execute(ms);
                                httpget.Server = tmp.Value as Server;
                            }
                        }
                        break;
                    case 'c':
                        {
                            byte[] client = new byte[6];
                            Array.Copy(profileData, ms.Position - 1, client, 0, 6);

                            if (Encoding.UTF8.GetString(client) == "client")
                            {
                                SubController single_set = new HttpGetClientController();
                                var tmp = single_set.Execute(ms);
                                httpget.client = tmp.Value as HttpGet.Client;
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
            token.Value = httpget;
            token.ValueUTF8 = Encoding.UTF8.GetString(data.ToArray());

            return token;
        }
    }
}
