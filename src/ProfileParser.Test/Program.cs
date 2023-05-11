using ProfileParser.Controller;

namespace ProfileParser.Test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Type type = typeof(Profile);

            foreach (var item in type.GetProperties())
            {
                Console.WriteLine(item);
            }

            using (Handler handler = new Handler("default.profile"))
            {

            }
        }
    }
}