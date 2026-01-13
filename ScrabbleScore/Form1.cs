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
        private const int VelikostPolicka = 38;

        private TextBox[,] poleUI = new TextBox[VelikostDesky, VelikostDesky];
        private Policko[,] poleLogika = new Policko[VelikostDesky, VelikostDesky];
        private ToolTip toolTipPismena = new ToolTip();

        private List<Hrac> hraci = new List<Hrac>();
        private int aktualniHracIndex = 0;

        private ListBox lbHraci;
        private Label lblNaTahu;

        public Form1() { InitializeComponent(); }

        public Form1(List<Hrac> zadaniHraci)
        {
            InitializeComponent();
            this.hraci = zadaniHraci;
            this.Text = "Scrabble Score Master";
            this.Size = new Size(920, 680);
            this.BackColor = Color.FromArgb(30, 30, 30);
            this.StartPosition = FormStartPosition.CenterScreen;

            InicializujDesku();
            InicializujOvladaciPanel();
            AktualizujUIHracu();
        }

        private void InicializujDesku()
        {
            Panel panelDeska = new Panel
            {
                Size = new Size(VelikostDesky * VelikostPolicka, VelikostDesky * VelikostPolicka),
                Location = new Point(20, 20),
                BackColor = Color.FromArgb(40, 80, 40),
                Padding = new Padding(3)
            };
            this.Controls.Add(panelDeska);

            for (int radek = 0; radek < VelikostDesky; radek++)
            {
                for (int sloupec = 0; sloupec < VelikostDesky; sloupec++)
                {
                    poleLogika[radek, sloupec] = new Policko();
                    poleLogika[radek, sloupec].Bonus = ZjistiBonus(radek, sloupec);

                    TextBox tb = new TextBox
                    {
                        Size = new Size(VelikostPolicka - 2, VelikostPolicka - 2),
                        Location = new Point(sloupec * VelikostPolicka, radek * VelikostPolicka),
                        TextAlign = HorizontalAlignment.Center,
                        MaxLength = 1,
                        BorderStyle = BorderStyle.None,
                        BackColor = poleLogika[radek, sloupec].ZiskejBarvu(),
                        Font = new Font("Segoe UI", 14, FontStyle.Bold),
                        CharacterCasing = CharacterCasing.Upper,
                        Tag = new Point(radek, sloupec)
                    };

                    tb.TextChanged += tb_TextChanged;
                    tb.KeyPress += tb_KeyPress;
                    tb.MouseEnter += tb_MouseEnter;

                    poleUI[radek, sloupec] = tb;
                    panelDeska.Controls.Add(tb);
                }
            }
        }

        private void InicializujOvladaciPanel()
        {
            int panelX = (VelikostDesky * VelikostPolicka) + 50;

            lblNaTahu = new Label
            {
                Text = "NA TAHU",
                Location = new Point(panelX, 20),
                Size = new Size(250, 40),
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White
            };
            this.Controls.Add(lblNaTahu);

            lbHraci = new ListBox
            {
                Location = new Point(panelX, 70),
                Size = new Size(250, 120),
                Font = new Font("Segoe UI", 12),
                BackColor = Color.FromArgb(45, 45, 45),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.None
            };
            this.Controls.Add(lbHraci);

            Button btnPotvrdit = new Button
            {
                Text = "POTVRDIT TAH",
                Location = new Point(panelX, 210),
                Size = new Size(250, 50),
                BackColor = Color.FromArgb(0, 150, 136),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnPotvrdit.FlatAppearance.BorderSize = 0;
            btnPotvrdit.Click += PotvrditTah_Click;
            this.Controls.Add(btnPotvrdit);

            Button btnSmazat = new Button
            {
                Text = "VYMAZAT TAH",
                Location = new Point(panelX, 270),
                Size = new Size(250, 40),
                BackColor = Color.FromArgb(200, 80, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10)
            };
            btnSmazat.FlatAppearance.BorderSize = 0;
            btnSmazat.Click += SmazatTah_Click;
            this.Controls.Add(btnSmazat);

            Label lblLegenda = new Label
            {
                Text = "LEGENDA POLÍČEK:\n" +
                       "Červená: 3x Slovo\n" +
                       "Růžová: 2x Slovo\n" +
                       "Tmavě modrá: 3x Písmeno\n" +
                       "Světle modrá: 2x Písmeno\n\n" +
                       "TIP: Bodovou hodnotu písmen\n" +
                       "zjistíš najetím myši na políčko.",
                Location = new Point(panelX, 330),
                Size = new Size(250, 200),
                ForeColor = Color.DarkGray,
                Font = new Font("Segoe UI", 9)
            };
            this.Controls.Add(lblLegenda);
        }

        private void tb_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void tb_MouseEnter(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (!string.IsNullOrEmpty(tb.Text))
            {
                int body = ScrabbleLogika.ZiskejBodovouHodnotu(tb.Text);
                toolTipPismena.SetToolTip(tb, "Písmeno " + tb.Text.ToUpper() + ": " + body + " b.");
            }
        }

        private void tb_TextChanged(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (tb.Tag is Point p)
            {
                poleLogika[p.X, p.Y].Pismeno = tb.Text.ToUpper();
                if (!string.IsNullOrEmpty(tb.Text) && !poleLogika[p.X, p.Y].JeZafixovano)
                    tb.BackColor = Color.FromArgb(255, 248, 200);
                else
                    tb.BackColor = poleLogika[p.X, p.Y].ZiskejBarvu();
            }
        }

        private void AktualizujUIHracu()
        {
            lbHraci.Items.Clear();
            foreach (var h in hraci) lbHraci.Items.Add(h.ToString());

            if (hraci.Count > 0)
            {
                lblNaTahu.Text = "NA TAHU: " + hraci[aktualniHracIndex].Jmeno.ToUpper();
                lblNaTahu.ForeColor = Color.FromArgb(0, 200, 150);
            }
        }

        private void SmazatTah_Click(object sender, EventArgs e)
        {
            for (int r = 0; r < VelikostDesky; r++)
            {
                for (int s = 0; s < VelikostDesky; s++)
                {
                    if (!poleLogika[r, s].JeZafixovano)
                    {
                        poleUI[r, s].Text = "";
                        poleLogika[r, s].Pismeno = "";
                        poleUI[r, s].BackColor = poleLogika[r, s].ZiskejBarvu();
                    }
                }
            }
        }

        private void PotvrditTah_Click(object sender, EventArgs e)
        {
            int bodyZaTah = 0;
            int nasobitelSlova = 1;
            int pocetNovychPismen = 0;

            for (int r = 0; r < VelikostDesky; r++)
            {
                for (int s = 0; s < VelikostDesky; s++)
                {
                    if (!poleLogika[r, s].JeZafixovano && !string.IsNullOrEmpty(poleLogika[r, s].Pismeno))
                    {
                        int hodnotaPismene = ScrabbleLogika.ZiskejBodovouHodnotu(poleLogika[r, s].Pismeno);
                        string bonus = poleLogika[r, s].Bonus;

                        if (bonus == "2L") hodnotaPismene *= 2;
                        else if (bonus == "3L") hodnotaPismene *= 3;

                        if (bonus == "2W") nasobitelSlova *= 2;
                        else if (bonus == "3W") nasobitelSlova *= 3;

                        bodyZaTah += hodnotaPismene;
                        pocetNovychPismen++;
                    }
                }
            }

            if (pocetNovychPismen == 0) return;

            bodyZaTah *= nasobitelSlova;
            if (pocetNovychPismen == 7) bodyZaTah += 50;

            hraci[aktualniHracIndex].Skore += bodyZaTah;
            ZafixujTah();

            aktualniHracIndex = (aktualniHracIndex + 1) % hraci.Count;
            AktualizujUIHracu();
        }

        private void ZafixujTah()
        {
            for (int r = 0; r < VelikostDesky; r++)
            {
                for (int s = 0; s < VelikostDesky; s++)
                {
                    if (!string.IsNullOrEmpty(poleLogika[r, s].Pismeno) && !poleLogika[r, s].JeZafixovano)
                    {
                        poleLogika[r, s].JeZafixovano = true;
                        poleUI[r, s].ReadOnly = true;
                        poleUI[r, s].BackColor = poleLogika[r, s].ZiskejBarvu();
                        poleUI[r, s].ForeColor = Color.FromArgb(50, 50, 50);
                    }
                }
            }
        }

        private string ZjistiBonus(int r, int s)
        {
            // Středové pole
            if (r == 7 && s == 7) return "start";

            // Trojnásobné slovo
            if ((r == 0 || r == 7 || r == 14) && (s == 0 || s == 7 || s == 14)) return "3W";

            // Dvojnásobné slovo
            if (r == s || r + s == 14)
            {
                if ((r >= 1 && r <= 4) || (r >= 10 && r <= 13)) return "2W";
            }

            // Trojnásobné písmeno
            if ((r == 1 || r == 13) && (s == 5 || s == 9) || (r == 5 || r == 9) && (s == 1 || s == 13) || (r == 5 || r == 9) && (s == 5 || s == 9)) return "3L";

            // Dvojnásobné písmeno
            if ((r == 0 || r == 14) && (s == 3 || s == 11) || (r == 2 || r == 12) && (s == 6 || s == 8) || (r == 3 || r == 11) && (s == 0 || s == 14) || (r == 6 || r == 8) && (s == 2 || s == 12) || (r == 7 && (s == 3 || s == 11)) || (s == 7 && (r == 3 || r == 11))) return "2L";

            return "zadny";
        }
    }
}
