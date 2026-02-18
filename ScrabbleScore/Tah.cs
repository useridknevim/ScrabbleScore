using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrabbleScore
{
    public class Tah
    {
        public string Popis { get; set; }
        public int Body { get; set; }
        public Tah(string popis, int body) { Popis = popis; Body = body; }
        public override string ToString() => Popis + " (+" + Body + " b.)";
    }
}