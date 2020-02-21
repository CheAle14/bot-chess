using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChessClient.Classes
{
    public class QueryParser
    {
        public string Argument { get; }
        public QueryParser(string arg)
        {
            Argument = arg;
            parse();
        }

        public string Token { get; private set; }

        public StartMode Mode { get; private set; }

        public string GameId { get; private set; }

        void parse()
        {
            MessageBox.Show(Argument, "Parse");
            var strip = Argument.Substring("chess://".Length);
            var split = strip.Split('/');
            if(Enum.TryParse<StartMode>(split[0], true, out var mode))
            {
                Mode = mode;
                Token = split[1];
            }
        }
    }
}
