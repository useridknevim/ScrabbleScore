using System.Drawing;

namespace ScrabbleScore
{
    public class Policko
    {
        public string Pismeno { get; set; } = "";
        public bool JeZafixovano { get; set; } = false;
        public string Bonus { get; set; } = "zadny";

        public Color ZiskejBarvu()
        {
            if (JeZafixovano)
            {
                return Color.FromArgb(220, 220, 210);
            }

            if (Bonus == "3W") return Color.FromArgb(255, 100, 100); // Červená
            else if (Bonus == "2W") return Color.FromArgb(255, 180, 180); // Růžová
            else if (Bonus == "3L") return Color.FromArgb(100, 150, 255); // Sytě modrá
            else if (Bonus == "2L") return Color.FromArgb(180, 210, 255); // Světle modrá
            else if (Bonus == "start") return Color.FromArgb(200, 190, 150); // Jiná barva pro střed (bez bonusu)
            else return Color.FromArgb(240, 240, 240); // Prázdné pole
        }
    }
}