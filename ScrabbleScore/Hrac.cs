using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrabbleScore
{
    public class Hrac
    {
        public string Jmeno { get; set; }
        public int Skore { get; set; }

        public Hrac(string jmeno)
        {
            Jmeno = jmeno;
            Skore = 0;
        }

        public override string ToString()
        {
            return Jmeno + ": " + Skore + " bodů";
        }
    }
}
