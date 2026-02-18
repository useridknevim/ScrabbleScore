using System.Drawing;

namespace ScrabbleScore
{
    public class Policko
    {
        // Písmeno, které je na políčku napsané
        public string Pismeno { get; set; } = "";

        // Pokud je true, písmeno bylo položeno v minulých kolech a nelze ho měnit
        public bool JeZafixovano { get; set; } = false;

        // Typ bonusu (zadny, 2L, 3L, 2W, 3W, start)
        public string Bonus { get; set; } = "zadny";

        // Metoda, která určí barvu políčka na základě bonusu a stavu zafixování
        public Color ZiskejBarvu()
        {
            Color zakladni;

            // Určení syté barvy podle typu bonusu
            if (Bonus == "3W")
            {
                zakladni = Color.FromArgb(255, 80, 80); // Červená
            }
            else if (Bonus == "2W")
            {
                zakladni = Color.FromArgb(255, 180, 180); // Růžová
            }
            else if (Bonus == "3L")
            {
                zakladni = Color.FromArgb(60, 120, 255); // Sytě modrá
            }
            else if (Bonus == "2L")
            {
                zakladni = Color.FromArgb(170, 210, 255); // Světle modrá
            }
            else if (Bonus == "start")
            {
                zakladni = Color.FromArgb(255, 230, 100); // Zlatá pro střed
            }
            else
            {
                zakladni = Color.FromArgb(240, 240, 240); // Standardní šedobílá
            }

            // Pokud je políčko zafixované, chceme, aby barva byla "vybledlá"
            if (JeZafixovano)
            {
                // Zprůměrujeme barvu s bílou (255, 255, 255), aby byla světlejší
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