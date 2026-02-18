using System.Collections.Generic;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ScrabbleScore
{
    public class Hrac
    {
        public string Jmeno { get; set; }
        public int Skore { get; set; }
        public List<Tah> HistorieTahu { get; set; }
        public Hrac(string jmeno) { Jmeno = jmeno; Skore = 0; HistorieTahu = new List<Tah>(); }
        public override string ToString() => Jmeno + ": " + Skore + " bodů";
    }
}