using ProfileParser.Controller.Single;
using ProfileParser.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ProfileParser.Controller.SingleController;

namespace ProfileParser.Controller
{
    internal abstract class SubController
    {
        public abstract TokenLevel TokenLevel { get; }

        public byte[] profileData { get; set; }

        public int GetLines(int position)
        {
            byte[] data = new byte[position];
            if (position > profileData.Length || position <= 0) { throw new ArgumentNullException(); }
            Array.Copy(profileData, 0, data, 0, position);
            return data.Where(x => x == '\n').Count();
        }


        public abstract Token Execute(MemoryStream ms);

    }

    public enum TokenLevel
    {
        Single,
        Block
    }

    internal abstract class SingleController : SubController
    {
        public override TokenLevel TokenLevel { get; } = TokenLevel.Single;

        public abstract SingleKeyword SingleTag { get; }

        public enum SingleKeyword
        {
            Comment,
            Set,
            Unset,
            Reset,
        }
    }

    internal abstract class BlockController : SubController
    {
        public override TokenLevel TokenLevel { get; } = TokenLevel.Block;

        public abstract BlockKeyword BlockTag { get; }

        public const char comment = '#';

        public List<Token> BlockComments = new();

        public void ParseComment(MemoryStream ms)
        {
            SubController token = new CommentController();
            BlockComments.Add(token.Execute(ms));
        }

        public enum BlockKeyword
        {
            Id,
            Stage,
            HttpGet,
            HttpPost,
            HttpGetClient,
            HttpPostClient,
            Server,
            Output,
            Metadata
        }
    }
}
