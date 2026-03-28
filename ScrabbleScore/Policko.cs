using System.Drawing;

namespace ScrabbleScore
{
    public class Policko
    {
        public string Pismeno { get; set; } = "";
        public bool JeZafixovano { get; set; } = false;
        public string Bonus { get; set; } = "zadny";

        public bool JeZolik { get; set; } = false;

        public Color ZiskejBarvu()
        {
            Color zakladni;

            if (Bonus == "3W")
            {
                zakladni = Color.FromArgb(255, 80, 80);
            }
            else if (Bonus == "2W")
            {
                zakladni = Color.FromArgb(255, 180, 180);
            }
            else if (Bonus == "3L")
            {
                zakladni = Color.FromArgb(60, 120, 255);
            }
            else if (Bonus == "2L")
            {
                zakladni = Color.FromArgb(170, 210, 255);
            }
            else if (Bonus == "start")
            {
                zakladni = Color.FromArgb(255, 230, 100);
            }
            else
            {
                zakladni = Color.FromArgb(240, 240, 240);
            }

            if (JeZafixovano == true)
            {
                return Color.FromArgb(
                    (zakladni.R + 255 * 2) / 3,
                    (zakladni.G + 255 * 2) / 3,
                    (zakladni.B + 255 * 2) / 3
                );
            }

            return zakladni;
        }
    }
}