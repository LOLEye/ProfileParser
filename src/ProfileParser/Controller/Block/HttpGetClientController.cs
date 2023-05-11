using ProfileParser.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ProfileParser.HttpGet;
using static ProfileParser.HttpGet.Client;

namespace ProfileParser.Controller.Block
{
    internal class HttpGetClientController : BlockController
    {
        public override BlockKeyword BlockTag => BlockKeyword.HttpGetClient;

        public override Token Execute(MemoryStream ms)
        {
            profileData = ms.ToArray();

            Token token = new Token();
            token.TokenType = TokenType.ClientBlock;

            Client client = new();
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
                    // metadata
                    case 'm':
                        {
                            byte[] metadata = new byte[8];
                            Array.Copy(profileData, ms.Position - 1, metadata, 0, 8);
                            if (Encoding.UTF8.GetString(metadata) == "metadata")
                            {
                                client.metadata = new MetadataController().Execute(ms).Value as Metadata;
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
