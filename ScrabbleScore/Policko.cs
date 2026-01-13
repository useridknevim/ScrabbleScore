using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrabbleScore
{
    internal class Policko
    {
        public string Pismeno { get; set; } = "";

        // Pomocná vlastnost, abychom věděli, co hráč přidal v tomto tahu
        public bool JeZafixovano { get; set; } = false;

        // Typ bonusu: "zadny", "2L", "3L", "2W", "3W"
        public string Bonus { get; set; } = "zadny";

        public Color ZiskejBarvu()
        {
            if (JeZafixovano) return Color.LightGray; // Písmena z minulých kol

            if (Bonus == "3W") return Color.Red;
            else if (Bonus == "2W") return Color.Pink;
            else if (Bonus == "3L") return Color.DarkBlue;
            else if (Bonus == "2L") return Color.LightBlue;
            else return Color.Green;
        }
    }
}