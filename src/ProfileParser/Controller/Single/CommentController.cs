using ProfileParser.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProfileParser.Controller.Single
{
    internal class CommentController : SingleController
    {
        public override SingleKeyword SingleTag => SingleKeyword.Comment;

        public override Token Execute(MemoryStream ms)
        {
            profileData = ms.ToArray();

            Token token = new Token();
            token.TokenType = TokenType.Comment;

            int item = 0;
            List<byte> data = new List<byte>();
            while (item != '\n')
            {
                item = ms.ReadByte();
                data.Add(Convert.ToByte(item));
            }
            token.Column = (int)ms.Position;
            token.Line = GetLines(token.Column);
            token.EndColumnSize = data.Count;

            token.Value = data;
            token.ValueUTF8 = Encoding.UTF8.GetString(data.ToArray());

            return token;
        }
    }
}
