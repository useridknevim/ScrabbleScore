using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScrabbleScore
{
    public partial class Form1 : Form
    {
        private const int VelikostDesky = 15;
        private const int VelikostPolicka = 30;

        private TextBox[,] poleUI = new TextBox[VelikostDesky, VelikostDesky];
        private Policko[,] poleLogika = new Policko[VelikostDesky, VelikostDesky];

        public Form1()
        {
            InitializeComponent();
            InicializujDesku();
        }

        private void InicializujDesku()
        {
            Panel panelDeska = new Panel
            {
                Size = new Size(VelikostDesky * VelikostPolicka, VelikostDesky * VelikostPolicka),
                Location = new Point(10, 10),
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(panelDeska);

            for (int radek = 0; radek < VelikostDesky; radek++)
            {
                for (int sloupec = 0; sloupec < VelikostDesky; sloupec++)
                {
                    poleLogika[radek, sloupec] = new Policko();
                    poleLogika[radek, sloupec].Bonus = ZjistiBonus(radek, sloupec);

                    // UI
                    TextBox tb = new TextBox
                    {
                        Size = new Size(VelikostPolicka, VelikostPolicka),
                        Location = new Point(sloupec * VelikostPolicka, radek * VelikostPolicka),
                        TextAlign = HorizontalAlignment.Center,
                        MaxLength = 1,
                        BackColor = poleLogika[radek, sloupec].ZiskejBarvu(),
                        ForeColor = Color.White,
                        Font = new Font("Arial", 12, FontStyle.Bold),
                        CharacterCasing = CharacterCasing.Upper
                    };

                    // Souřadnice
                    tb.Tag = new Point(radek, sloupec);

                    tb.TextChanged += tb_TextChanged;

                    poleUI[radek, sloupec] = tb;
                    panelDeska.Controls.Add(tb);
                }
            }
        }

        private void tb_TextChanged(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;
            Point p = (Point)tb.Tag; // souřadnice políčka
            int radek = p.X;
            int sloupec = p.Y;

            if (string.IsNullOrEmpty(tb.Text))
            {
                poleLogika[radek, sloupec].Pismeno = "";
            }
            else
            {
                char c = tb.Text[0];
                if (char.IsLetter(c))
                    poleLogika[radek, sloupec].Pismeno = c.ToString().ToUpper();
                else
                    tb.Text = "";
            }
        }

        private string ZjistiBonus(int r, int s)
        {
            // trojnásobné slovo (červené rohy + středové)
            if ((r == 0 || r == 7 || r == 14) && (s == 0 || s == 7 || s == 14))
                return "3W";

            // dvojnásobné slovo (růžové diagonály)
            if (r == s || r + s == 14)
                return "2W";

            // trojnásobné písmeno (tmavě modré)
            if ((r == 1 || r == 13) && (s == 5 || s == 9) ||
                (r == 5 || r == 9) && (s == 1 || s == 13) ||
                (r == 5 || r == 9) && (s == 5 || s == 9))
                return "3L";

            // dvojnásobné písmeno (světle modré)
            if ((r == 0 || r == 14) && (s == 3 || s == 11) ||
                (r == 2 || r == 12) && (s == 6 || s == 8) ||
                (r == 3 || r == 11) && (s == 0 || s == 14) ||
                (r == 6 || r == 8) && (s == 2 || s == 12) ||
                (r == 7 && (s == 3 || s == 11)) ||
                (s == 7 && (r == 3 || r == 11)))
                return "2L";

            // normální pole
            return "zadny";
        }
    }
}
