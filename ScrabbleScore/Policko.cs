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

        // Typ bonusu na tomto políčku
        // "zadny" = běžné pole
        // "2L" = dvojnásobné písmeno
        // "3L" = trojnásobné písmeno
        // "2W" = dvojnásobné slovo
        // "3W" = trojnásobné slovo
        public string Bonus { get; set; } = "zadny";

        public Color ZiskejBarvu()
        {
            if (Bonus == "3W")
            {
                return Color.Red; // trojnásobné slovo
            }
            else if (Bonus == "2W")
            {
                return Color.Pink; // dvojnásobné slovo
            }
            else if (Bonus == "3L")
            {
                return Color.DarkBlue; // trojnásobné písmeno
            }
            else if (Bonus == "2L")
            {
                return Color.LightBlue; // dvojnásobné písmeno
            }
            else
            {
                return Color.Green; // normální pole
            }
        }
    }
}
