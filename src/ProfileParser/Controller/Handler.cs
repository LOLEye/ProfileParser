using ProfileParser.Controller.Block;
using ProfileParser.Controller.Single;
using ProfileParser.Lexer;
using ProfileParser.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProfileParser.Controller
{
    public class Handler : IDisposable
    {
        private byte[] profileData;

        public Profile Profile { get; set; } = new();

        public List<Token> Tokens = new();

        public event EventHandler DisposeCallback;

        public Handler(string filename)
        {
            if(String.IsNullOrEmpty(filename) || !File.Exists(filename)) { throw new ArgumentNullException(); }
            profileData = File.ReadAllBytes(filename);
            DisposeCallback += Handler_DisposeCallback;
        }


        private void Handler_DisposeCallback(object? sender, EventArgs e)
        {
            Init();
        }


        private void Init()
        {
            using (MemoryStream ms = new MemoryStream(profileData))
            {
                int end = 0;
                while (end != -1)
                {
                    end = ms.ReadByte();

                    switch (end)
                    {
                        case '#':
                            {
                                SubController token = new CommentController();
                                Tokens.Add(token.Execute(ms));
                            }
                            break;
                        case 's':   // 解析set
                            {
                                if(profileData[ms.Position - 2] == '\n')
                                {
                                    byte[] set = new byte[3];
                                    Array.Copy(profileData, ms.Position - 1, set, 0, 3);
                                    if(Encoding.UTF8.GetString(set) == "set")
                                    {
                                        ms.Position += 2;
                                        SubController token = new SinglePropertyController();
                                        var tmp = token.Execute(ms);
                                        var results = tmp.ValueUTF8.Trim().Split().Where(x => !String.IsNullOrEmpty(x)).ToArray();
                                        DynamicProperty.Set(Profile,results);
                                        Tokens.Add(tmp);
                                    }

                                    byte[] stage = new byte[5];
                                    Array.Copy(profileData, ms.Position - 1, stage, 0, 5);
                                    if (Encoding.UTF8.GetString(stage) == "stage")
                                    {
                                        ms.Position -= 1;
                                        SubController token = new StageBlockController();
                                        var tmp = token.Execute(ms);
                                        var results = tmp.ValueUTF8.Trim().Split().Where(x => !String.IsNullOrEmpty(x)).ToArray();
                                        DynamicProperty.Set(Profile, results,tmp.Value);
                                        Tokens.Add(tmp);
                                    }
                                }
                            }
                            break;
                        case 'h':
                            {
                                byte[] http_get = new byte[8];
                                Array.Copy(profileData, ms.Position - 1, http_get, 0, 8);
                                if (Encoding.UTF8.GetString(http_get) == "http-get")
                                {
                                    ms.Position -= 1;
                                    SubController token = new HttpGetController();
                                    var tmp = token.Execute(ms);
                                    var results = tmp.ValueUTF8.Trim().Split().Where(x => !String.IsNullOrEmpty(x)).ToArray();
                                    DynamicProperty.Set(Profile, results, tmp.Value);
                                    Tokens.Add(tmp);
                                }
                                byte[] http_post = new byte[9];
                                Array.Copy(profileData, ms.Position - 1, http_post, 0, 9);
                                if (Encoding.UTF8.GetString(http_post) == "http-post")
                                {
                                    ms.Position -= 1;
                                    SubController token = new HttpPostController();
                                    var tmp = token.Execute(ms);
                                    var results = tmp.ValueUTF8.Trim().Split().Where(x => !String.IsNullOrEmpty(x)).ToArray();
                                    DynamicProperty.Set(Profile, results, tmp.Value);
                                    Tokens.Add(tmp);
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }

                Console.WriteLine("parse over");
            }
        }

        public void Dispose()
        {
            DisposeCallback(this, new EventArgs());
        }
    }
}
