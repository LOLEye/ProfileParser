using ProfileParser.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ProfileParser.HttpGet.Client;
using static ProfileParser.Server;

namespace ProfileParser.Controller.Block
{
    internal class MetadataController : BlockController
    {
        public override BlockKeyword BlockTag => BlockKeyword.Metadata;

        public override Token Execute(MemoryStream ms)
        {
            profileData = ms.ToArray();

            Token token = new Token();
            token.TokenType = TokenType.MetadataBlock;

            Metadata metadata = new();
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
                    // datatype
                    case 'd':
                        {
                            byte[] datatype = new byte[8];
                            Array.Copy(profileData, ms.Position - 1, datatype, 0, 8);
                            if (Encoding.UTF8.GetString(datatype) == "datatype")
                            {
                                ms.Position += 7;
                                int index = 0;
                                List<byte> data_print = new List<byte>();
                                while (index != ';')
                                {
                                    index = ms.ReadByte();
                                    if (index == '\n') { throw new Exception("metadata.header parse miss ;"); }
                                    data_print.Add(Convert.ToByte(index));
                                }
                                try
                                {
                                    metadata.DataType = Enum.Parse<Metadata.Data>(Encoding.UTF8.GetString(data_print.ToArray()).Replace("\"","").TrimEnd(';'));
                                }
                                catch (Exception ex) 
                                {
                                    throw new Exception("metadata datatype parse fault\n" + ex.Message);
                                }

                            }
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
                                    if (index == '\n') { throw new Exception("metadata.header parse miss ;"); }
                                    data_print.Add(Convert.ToByte(index));
                                }
                                metadata.Header = Encoding.UTF8.GetString(data_print.ToArray()).Replace("\"", "").TrimEnd(';').Trim();
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
            token.Value = metadata;
            token.ValueUTF8 = Encoding.UTF8.GetString(data.ToArray());
            return token;
        }
    }
}
