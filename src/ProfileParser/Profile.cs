using ProfileParser.Misc;

namespace ProfileParser
{
    public sealed class Profile
    {
        #region Optional Property

        [PropertyTag(TagType.Optional)]
        public string Version { get; set; }

        [PropertyTag(TagType.Optional)]
        public string Author { get; set; }

        [PropertyTag(TagType.Optional)]
        public string DateTime { get; set; }

        #endregion


        public int Sleeptime { get; set; }

        public int Jitter { get; set; }

        public int Maxdns { get; set; }

        public string Samplename { get; set; }

        public bool UseSyscall { get; set; }

        public Stage Stage { get; set; }

        public HttpGet HttpGet { get; set; }
        public HttpPost HttpPost { get; set; }
    }

    #region Block

    public sealed class Stage
    {
        public bool StompPe { get; set; }

        public string Name { get; set; }

        public string[] Contents { get;  set; }
    }

    public sealed class HttpGet
    {
        public string[] Uri { get; set; }

        public class Client
        {
            public Metadata metadata { get; set; }

            public class Metadata
            {
                public Data DataType { get; set; }

                public enum Data
                {
                    base64,
                    bin,
                }

                public string Header { get; set; }
            }
        }

        public Client client { get; set; }

        public Server Server { get; set; }

    }

    public sealed class HttpPost
    {
        public string[] Uri { get; set; }

        public class Client
        {
            public HeaderContent[] Header { get; set; }

            public class Id
            {
                public string Name { get; set; }
            }

            public Id parameter { get; set; }

            public Server.Output Output { get; set; }
        }

        public Client client { get; set; }

        public Server Server { get; set; }
    }

    public class HeaderContent
    {
        public string HeaderKey { get; set; }
        public object HeaderValue { get; set; }
    }

    public sealed class Server
    {
        public HeaderContent[] Header { get; set; }

        public Output output { get; set; }

        public class Output
        {
            public string Response { get; set; }
        }
    }

    #endregion
}