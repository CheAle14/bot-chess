using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ChessInstaller
{
    public class ClientVersion : IComparable<ClientVersion>
    {
        public int Major { get; set; }
        public int Minor { get; set; }
        public int AlphaV { get; set; }
        public int BetaV { get; set; }
        public int HotFixV { get; set; }
        public ClientVersion(int major, int minor, int alphav = 0, int betav = 0, int hotfixv = 0)
        {
            Major = major;
            Minor = minor;
            AlphaV = alphav;
            BetaV = betav;
            HotFixV = hotfixv;
        }
        public override string ToString()
        {
            string v = $"v{Major}.{Minor}";
            if (AlphaV > 0)
                v += "-alpha" + AlphaV.ToString("00");
            if (BetaV > 0)
                v += "-beta" + BetaV.ToString("00");
            if (HotFixV > 0)
                v += "-hotfix" + HotFixV.ToString("00");
            return v;
        }

        public int CompareTo(ClientVersion other)
        {
            if(Major > other.Major)
            {
                return 1;
            } else if (Major < other.Major)
            {
                return -1;
            }

            if(Minor > other.Minor)
            {
                return 1;
            } else if (Minor < other.Minor)
            {
                return -1;
            }

            if(AlphaV > other.AlphaV)
            {
                return 1;
            } else if (AlphaV < other.AlphaV)
            {
                return -1;
            }

            if(BetaV > other.BetaV)
            {
                return 1;
            } else if (BetaV < other.BetaV)
            {
                return -1;
            }

            if(HotFixV > other.HotFixV)
            {
                return 1;
            } else if (HotFixV < other.HotFixV)
            {
                return -1;
            }

            return 0;
        }

        const string mainRegex = "v(\\d+)\\.(\\d+)";
        const string addOnRegexes = "(\\d{1,2})";
        public ClientVersion(string version)
        {
            if (version.StartsWith("v") == false)
                throw new ArgumentException("Version invalid format: 'v0.0...'");
            if (version.Contains(".") == false)
                throw new ArgumentException("Version invalid format: must have point: 'v0.0'");
            var majorMinor = new Regex(mainRegex);
            var match = majorMinor.Match(version);
            Major = int.Parse(match.Groups[1].Value);
            Minor = int.Parse(match.Groups[2].Value);
            foreach(var addon in new string[] { "alpha", "beta", "hotfix"})
            {
                var pattern = $"-{addon}{addOnRegexes}";
                var rgx = new Regex(pattern);
                var mtc = rgx.Match(version);
                if(mtc.Success)
                {
                    int val = int.Parse(mtc.Groups[1].Value);
                    if (addon[0] == 'a')
                        AlphaV = val;
                    else if (addon[0] == 'b')
                        BetaV = val;
                    else
                        HotFixV = val;
                }
            }
        }
    
        
    }
}
