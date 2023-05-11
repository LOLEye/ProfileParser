using ProfileParser.Controller.Single;
using ProfileParser.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProfileParser.Controller.Block
{
    internal class StageBlockController : BlockController
    {
        public override BlockKeyword BlockTag => BlockKeyword.Stage;

        public override Token Execute(MemoryStream ms)
        {
            profileData = ms.ToArray();

            Token token = new Token();
            token.TokenType = TokenType.StageBlock;

            Stage stage = new Stage();
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
                                DynamicProperty.Set(stage, results);
                            }

                            byte[] @string = new byte[6];
                            Array.Copy(profileData, ms.Position - 1, @string, 0, 6);
                            if (Encoding.UTF8.GetString(@string) == "string")
                            {
                                ms.Position += 5;
                                int value = 0;
                                List<byte> contentBytes = new List<byte>();
                                while (value != ';')
                                {
                                    value = ms.ReadByte();
                                    if (value == '\n') { throw new Exception("stage.string parse miss ;"); }
                                    contentBytes.Add(Convert.ToByte(value));
                                }

                                contents.Add(Encoding.UTF8.GetString(contentBytes.ToArray()).Replace(";","").Replace("\"","").Trim());
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
            stage.Contents = contents.ToArray();
            token.Value = stage;
            token.ValueUTF8 = Encoding.UTF8.GetString(data.ToArray());

            return token;
        }
    }
}
