using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ScrabbleScore
{
    public partial class Form1 : Form
    {
        // Nastavení desky
        private const int VelikostDesky = 15;
        private const int VelikostPolicka = 38;

        // UI pole a data
        private TextBox[,] poleUI = new TextBox[VelikostDesky, VelikostDesky];
        private Policko[,] poleLogika = new Policko[VelikostDesky, VelikostDesky];

        private ToolTip toolTip = new ToolTip();
        private List<Hrac> hraci;
        private int aktualniHracIndex = 0;

        // Prvky panelu
        private ListBox lbHraci;
        private ListBox lbHistorie;
        private Label lblNaTahu;


        // Start okna
        public Form1(List<Hrac> zadaniHraci)
        {
            InitializeComponent();

            this.hraci = zadaniHraci;
            this.Text = "Scrabble Score";
            this.Size = new Size(1150, 750);
            this.BackColor = Color.FromArgb(30, 30, 30);
            this.StartPosition = FormStartPosition.CenterScreen;

            InicializujDesku();
            InicializujUI();
            AktualizujVse();
        }


        // Výroba mřížky
        private void InicializujDesku()
        {
            Panel panelDeska = new Panel
            {
                Size = new Size(VelikostDesky * VelikostPolicka, VelikostDesky * VelikostPolicka),
                Location = new Point(20, 20),
                BackColor = Color.FromArgb(40, 80, 40)
            };
            this.Controls.Add(panelDeska);

            for (int r = 0; r < VelikostDesky; r++)
            {
                for (int s = 0; s < VelikostDesky; s++)
                {
                    poleLogika[r, s] = new Policko { Bonus = ZjistiBonus(r, s) };

                    TextBox tb = new TextBox
                    {
                        Size = new Size(VelikostPolicka - 2, VelikostPolicka - 2),
                        Location = new Point(s * VelikostPolicka, r * VelikostPolicka),
                        TextAlign = HorizontalAlignment.Center,
                        MaxLength = 1,
                        BorderStyle = BorderStyle.None,
                        BackColor = poleLogika[r, s].ZiskejBarvu(),
                        Font = new Font("Segoe UI", 14, FontStyle.Bold),
                        CharacterCasing = CharacterCasing.Upper,
                        Tag = new Point(r, s)
                    };

                    tb.TextChanged += ObsluhaZmenyTextu;
                    tb.MouseDown += ObsluhaZolika;
                    tb.MouseEnter += ObsluhaToolTipu;

                    poleUI[r, s] = tb;
                    panelDeska.Controls.Add(tb);
                }
            }
        }


        // Ovládací prvky
        private void InicializujUI()
        {
            int panelX = 620;

            // Info nápis
            lblNaTahu = new Label { Text = "NA TAHU", Location = new Point(panelX, 20), Size = new Size(300, 30), Font = new Font("Segoe UI", 14, FontStyle.Bold), ForeColor = Color.White };
            this.Controls.Add(lblNaTahu);

            // Seznam hráčů
            lbHraci = new ListBox { Location = new Point(panelX, 60), Size = new Size(200, 100), BackColor = Color.FromArgb(45, 45, 45), ForeColor = Color.White, BorderStyle = BorderStyle.None, Font = new Font("Segoe UI", 11) };
            this.Controls.Add(lbHraci);

            // Potvrdit tah
            Button btnOk = new Button { Text = "POTVRDIT TAH", Location = new Point(panelX, 170), Size = new Size(200, 45), BackColor = Color.SeaGreen, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            btnOk.Click += PotvrditTah;
            this.Controls.Add(btnOk);

            // Vymazat pole
            Button btnClear = new Button { Text = "VYMAZAT TAH", Location = new Point(panelX, 225), Size = new Size(200, 35), BackColor = Color.IndianRed, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnClear.Click += (s, e) => SmazatAktualniTah();
            this.Controls.Add(btnClear);

            // Tlačítko restart
            Button btnRestart = new Button { Text = "KOMPLETNÍ RESTART", Location = new Point(panelX, 480), Size = new Size(200, 50), BackColor = Color.FromArgb(80, 80, 80), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            btnRestart.Click += (s, e) => RestartovatHru();
            this.Controls.Add(btnRestart);

            // Popisky barev
            Label lblLegendaNadpis = new Label { Text = "LEGENDA:", Location = new Point(panelX, 280), ForeColor = Color.White, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            this.Controls.Add(lblLegendaNadpis);
            VytvorPolozkuLegendy(panelX, 310, Color.FromArgb(255, 80, 80), "3W - Trojnásobné slovo");
            VytvorPolozkuLegendy(panelX, 335, Color.FromArgb(255, 180, 180), "2W - Dvojnásobné slovo");
            VytvorPolozkuLegendy(panelX, 360, Color.FromArgb(60, 120, 255), "3L - Trojnásobné písmeno");
            VytvorPolozkuLegendy(panelX, 385, Color.FromArgb(170, 210, 255), "2L - Dvojnásobné písmeno");
            VytvorPolozkuLegendy(panelX, 410, Color.Red, "ČERVENÝ TEXT - Žolík (0 b.)");
            VytvorPolozkuLegendy(panelX, 435, Color.White, "Pravé klik na pole = Žolík");

            // Historie tahů
            lbHistorie = new ListBox { Location = new Point(panelX + 220, 60), Size = new Size(260, 500), BackColor = Color.FromArgb(45, 45, 45), ForeColor = Color.LightGray, BorderStyle = BorderStyle.None, Font = new Font("Segoe UI", 9) };
            this.Controls.Add(lbHistorie);
        }


        // Pravý klik - žolík
        private void ObsluhaZolika(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                TextBox tb = (TextBox)sender;
                Point p = (Point)tb.Tag;

                if (poleLogika[p.X, p.Y].JeZafixovano == false && string.IsNullOrEmpty(tb.Text) == false)
                {
                    // Přepnutí žolíka
                    poleLogika[p.X, p.Y].JeZolik = !poleLogika[p.X, p.Y].JeZolik;

                    if (poleLogika[p.X, p.Y].JeZolik == true)
                    {
                        tb.ForeColor = Color.Red;
                    }
                    else
                    {
                        tb.ForeColor = Color.Black;
                    }
                }
            }
        }

        // Bublina s body
        private void ObsluhaToolTipu(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (string.IsNullOrEmpty(tb.Text) == false)
            {
                Point p = (Point)tb.Tag;
                int body = poleLogika[p.X, p.Y].JeZolik ? 0 : ScrabbleLogika.ZiskejBodovouHodnotu(tb.Text);

                string info = "Písmeno " + tb.Text + ": " + body + " bodů";
                if (poleLogika[p.X, p.Y].JeZolik == true)
                {
                    info += " (Žolík - 0 b.)";
                }
                toolTip.SetToolTip(tb, info);
            }
        }


        // Kontrola a potvrzení
        private void PotvrditTah(object sender, EventArgs e)
        {
            List<Point> nove = new List<Point>();
            bool deskaBylaPrazdna = true;

            // Hledání nových písmen
            for (int r = 0; r < 15; r++)
            {
                for (int s = 0; s < 15; s++)
                {
                    if (poleLogika[r, s].JeZafixovano == true) { deskaBylaPrazdna = false; }
                    if (poleLogika[r, s].JeZafixovano == false && string.IsNullOrEmpty(poleLogika[r, s].Pismeno) == false)
                    {
                        nove.Add(new Point(r, s));
                    }
                }
            }

            if (nove.Count == 0) { return; }

            if (nove.Count > 7)
            {
                MessageBox.Show("V jednom tahu můžete položit max 7 písmen!");
                return;
            }

            // Kontrola směru
            bool radekStejny = nove.All(p => p.X == nove[0].X);
            bool sloupecStejny = nove.All(p => p.Y == nove[0].Y);

            if (radekStejny == false && sloupecStejny == false)
            {
                MessageBox.Show("Písmena musí být v jedné přímce!");
                return;
            }

            // Kontrola navazování
            bool navazuje = deskaBylaPrazdna ? nove.Any(p => p.X == 7 && p.Y == 7) : nove.Any(p => SousediSeStarym(p.X, p.Y));

            if (navazuje == false)
            {
                string msg = deskaBylaPrazdna ? "První slovo musí jít přes střed!" : "Musíte navázat na existující písmeno!";
                MessageBox.Show(msg);
                return;
            }

            // Výpočet bodů
            int bodyTah = VypocitejSkore(nove, out string seznamSlov);

            if (bodyTah == 0)
            {
                MessageBox.Show("Slovo se nemůže skládat pouze z jednoho písmene!");
                return;
            }

            Hrac h = hraci[aktualniHracIndex];
            h.Skore += bodyTah;

            lbHistorie.Items.Insert(0, h.Jmeno + ": " + seznamSlov + " (+" + bodyTah + "b)");

            // Fixace a rotace
            ZafixujPismena();
            aktualniHracIndex = (aktualniHracIndex + 1) % hraci.Count;
            AktualizujVse();
        }


        // Sčítání bodů za tah
        private int VypocitejSkore(List<Point> nove, out string slovaText)
        {
            int celkem = 0;
            List<string> nalezenaSlova = new List<string>();
            bool jeHorizontalni = nove.All(p => p.X == nove[0].X);

            // Hlavní slovo
            celkem += SpoctiJednoSlovo(nove[0].X, nove[0].Y, jeHorizontalni, out string hlavni);
            if (string.IsNullOrEmpty(hlavni) == false) { nalezenaSlova.Add(hlavni); }

            // Křížová slova
            foreach (Point p in nove)
            {
                int bodyKriz = SpoctiJednoSlovo(p.X, p.Y, !jeHorizontalni, out string kriz);
                if (bodyKriz > 0)
                {
                    celkem += bodyKriz;
                    nalezenaSlova.Add(kriz);
                }
            }

            // Bonus 50 bodů
            if (nove.Count == 7)
            {
                celkem += 50;
                nalezenaSlova.Add("BINGO BONUS (+50b)");
            }

            slovaText = string.Join(", ", nalezenaSlova);
            return celkem;
        }

        // Hledání slova v řadě
        private int SpoctiJednoSlovo(int r, int s, bool horiz, out string text)
        {
            int startR = r; int startS = s;

            // Najít začátek
            if (horiz == true)
            {
                while (startS > 0 && string.IsNullOrEmpty(poleLogika[startR, startS - 1].Pismeno) == false) { startS--; }
            }
            else
            {
                while (startR > 0 && string.IsNullOrEmpty(poleLogika[startR - 1, startS].Pismeno) == false) { startR--; }
            }

            int sumaBody = 0; int nasobitelSlova = 1; int delka = 0; text = "";
            int currR = startR; int currS = startS;

            // Průchod slova
            while (currR < 15 && currS < 15 && string.IsNullOrEmpty(poleLogika[currR, currS].Pismeno) == false)
            {
                Policko p = poleLogika[currR, currS];
                int pBody = p.JeZolik ? 0 : ScrabbleLogika.ZiskejBodovouHodnotu(p.Pismeno);

                if (p.JeZafixovano == false)
                {
                    if (p.Bonus == "2L") { pBody *= 2; }
                    if (p.Bonus == "3L") { pBody *= 3; }
                    if (p.Bonus == "2W") { nasobitelSlova *= 2; }
                    if (p.Bonus == "3W") { nasobitelSlova *= 3; }
                }

                sumaBody += pBody;
                text += p.Pismeno;
                delka++;

                if (horiz == true) { currS++; } else { currR++; }
            }

            if (delka < 2) { text = ""; return 0; }
            return sumaBody * nasobitelSlova;
        }


        // Kompletní čistka
        private void RestartovatHru()
        {
            DialogResult odpoved = MessageBox.Show("Smazat úplně všechno a začít znovu?", "Kompletní restart", MessageBoxButtons.YesNo);

            if (odpoved == DialogResult.Yes)
            {
                // Reset mřížky
                for (int r = 0; r < 15; r++)
                {
                    for (int s = 0; s < 15; s++)
                    {
                        poleLogika[r, s].Pismeno = "";
                        poleLogika[r, s].JeZafixovano = false;
                        poleLogika[r, s].JeZolik = false;

                        poleUI[r, s].Text = "";
                        poleUI[r, s].ReadOnly = false;
                        poleUI[r, s].ForeColor = Color.Black;
                        poleUI[r, s].BackColor = poleLogika[r, s].ZiskejBarvu();
                    }
                }

                // Reset hráčů a historie
                foreach (Hrac h in hraci)
                {
                    h.Skore = 0;
                    h.HistorieTahu.Clear();
                }

                lbHistorie.Items.Clear();
                aktualniHracIndex = 0;
                AktualizujVse();
            }
        }


        // Psaní do pole
        private void ObsluhaZmenyTextu(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;
            Point p = (Point)tb.Tag;
            poleLogika[p.X, p.Y].Pismeno = tb.Text;

            
            if (string.IsNullOrEmpty(tb.Text) == false && poleLogika[p.X, p.Y].JeZafixovano == false)
            {
                tb.BackColor = Color.FromArgb(255, 248, 200);
            }
            else
            {
                tb.BackColor = poleLogika[p.X, p.Y].ZiskejBarvu();
            }
        }


        // Zafixování
        private void ZafixujPismena()
        {
            for (int r = 0; r < 15; r++)
            {
                for (int s = 0; s < 15; s++)
                {
                    if (string.IsNullOrEmpty(poleLogika[r, s].Pismeno) == false)
                    {
                        poleLogika[r, s].JeZafixovano = true;
                        poleUI[r, s].ReadOnly = true;
                        poleUI[r, s].BackColor = poleLogika[r, s].ZiskejBarvu();

                        // Barva textu
                        if (poleLogika[r, s].JeZolik == true)
                        {
                            poleUI[r, s].ForeColor = Color.DarkRed;
                        }
                        else
                        {
                            poleUI[r, s].ForeColor = Color.DimGray;
                        }
                    }
                }
            }
        }


        // Sousedící písmena
        private bool SousediSeStarym(int r, int s)
        {
            int[] dr = { -1, 1, 0, 0 }; int[] ds = { 0, 0, -1, 1 };
            for (int i = 0; i < 4; i++)
            {
                int nr = r + dr[i]; int ns = s + ds[i];
                if (nr >= 0 && nr < 15 && ns >= 0 && ns < 15 && poleLogika[nr, ns].JeZafixovano == true)
                {
                    return true;
                }
            }
            return false;
        }


        // Refresh tabulek
        private void AktualizujVse()
        {
            lbHraci.Items.Clear();
            foreach (Hrac h in hraci)
            {
                lbHraci.Items.Add(h.ToString());
            }

            lblNaTahu.Text = "NA TAHU: " + hraci[aktualniHracIndex].Jmeno;
        }


        // Legenda
        private void VytvorPolozkuLegendy(int x, int y, Color barva, string text)
        {
            Panel p = new Panel { Location = new Point(x, y), Size = new Size(15, 15), BackColor = barva };
            Label l = new Label { Text = text, Location = new Point(x + 20, y - 2), Size = new Size(180, 20), ForeColor = Color.Silver, Font = new Font("Segoe UI", 8) };
            this.Controls.Add(p); this.Controls.Add(l);
        }


        // Smazání rozepsaného
        private void SmazatAktualniTah()
        {
            for (int r = 0; r < 15; r++)
            {
                for (int s = 0; s < 15; s++)
                {
                    if (poleLogika[r, s].JeZafixovano == false)
                    {
                        poleUI[r, s].Text = "";
                        poleLogika[r, s].Pismeno = "";
                        poleLogika[r, s].JeZolik = false;
                        poleUI[r, s].ForeColor = Color.Black;
                    }
                }
            }
        }


        // Typ bonusu
        private string ZjistiBonus(int r, int s)
        {
            if (r == 7 && s == 7) return "start";
            if ((r == 0 || r == 7 || r == 14) && (s == 0 || s == 7 || s == 14)) return "3W";
            if (r == s || r + s == 14) { if ((r >= 1 && r <= 4) || (r >= 10 && r <= 13)) return "2W"; }
            if ((r == 1 || r == 13) && (s == 5 || s == 9) || (r == 5 || r == 9) && (s == 1 || s == 13) || (r == 5 || r == 9) && (s == 5 || s == 9)) return "3L";
            if ((r == 0 || r == 14) && (s == 3 || s == 11) || (r == 2 || r == 12) && (s == 6 || s == 8) || (r == 3 || r == 11) && (s == 0 || s == 14) || (r == 6 || r == 8) && (s == 2 || s == 12) || (r == 7 && (s == 3 || s == 11)) || (s == 7 && (r == 3 || r == 11))) return "2L";
            return "zadny";
        }
    }
}