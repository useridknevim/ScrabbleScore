using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ScrabbleScore
{
    public partial class Form1 : Form
    {
        // Konstanty pro velikost desky
        private const int VelikostDesky = 15;
        private const int VelikostPolicka = 38;

        // Pole pro uchování textových polí (UI) a logických dat (Data)
        private TextBox[,] poleUI = new TextBox[VelikostDesky, VelikostDesky];
        private Policko[,] poleLogika = new Policko[VelikostDesky, VelikostDesky];

        // Pomůcka pro zobrazení bodů při najetí myší
        private ToolTip toolTip = new ToolTip();

        // Seznam hráčů a index toho, kdo je právě na řadě
        private List<Hrac> hraci;
        private int aktualniHracIndex = 0;

        // Komponenty uživatelského rozhraní
        private ListBox lbHraci;    // Tabulka celkového skóre
        private ListBox lbHistorie; // Seznam odehraných tahů
        private Label lblNaTahu;    // Text s jménem aktuálního hráče

        // Konstruktor - spustí se při vytvoření okna
        public Form1(List<Hrac> zadaniHraci)
        {
            InitializeComponent();
            this.hraci = zadaniHraci;

            // Nastavení základních vlastností okna
            this.Text = "Scrabble Score Master";
            this.Size = new Size(1150, 720);
            this.BackColor = Color.FromArgb(30, 30, 30); // Tmavé pozadí
            this.StartPosition = FormStartPosition.CenterScreen;

            // Zavolání metod pro vytvoření desky a ovládacích prvků
            InicializujDesku();
            InicializujUI();
            AktualizujVse();
        }

        // Metoda vytvoří mřížku 15x15 TextBoxů
        private void InicializujDesku()
        {
            Panel panelDeska = new Panel
            {
                Size = new Size(VelikostDesky * VelikostPolicka, VelikostDesky * VelikostPolicka),
                Location = new Point(20, 20),
                BackColor = Color.FromArgb(40, 80, 40) // Zelená barva plátna
            };
            this.Controls.Add(panelDeska);

            for (int r = 0; r < VelikostDesky; r++)
            {
                for (int s = 0; s < VelikostDesky; s++)
                {
                    // Vytvoření logického políčka a přiřazení bonusu podle souřadnic
                    poleLogika[r, s] = new Policko { Bonus = ZjistiBonus(r, s) };

                    // Vytvoření vizuálního políčka (TextBox)
                    TextBox tb = new TextBox
                    {
                        Size = new Size(VelikostPolicka - 2, VelikostPolicka - 2),
                        Location = new Point(s * VelikostPolicka, r * VelikostPolicka),
                        TextAlign = HorizontalAlignment.Center,
                        MaxLength = 1,
                        BorderStyle = BorderStyle.None,
                        BackColor = poleLogika[r, s].ZiskejBarvu(),
                        Font = new Font("Segoe UI", 14, FontStyle.Bold),
                        CharacterCasing = CharacterCasing.Upper, // Vždy velká tiskací písmena
                        Tag = new Point(r, s) // Schováme si souřadnice do Tagu pro pozdější použití
                    };

                    // Událost: Když uživatel napíše písmeno
                    tb.TextChanged += (snder, e) => {
                        Point p = (Point)tb.Tag;
                        poleLogika[p.X, p.Y].Pismeno = tb.Text.ToUpper();
                    };

                    // Událost: Když najede myší, zobrazí se ToolTip s bodovou hodnotou
                    tb.MouseEnter += (snder, e) => {
                        if (string.IsNullOrEmpty(tb.Text) == false)
                        {
                            int body = ScrabbleLogika.ZiskejBodovouHodnotu(tb.Text);
                            toolTip.SetToolTip(tb, "Písmeno " + tb.Text + ": " + body + " bodů");
                        }
                    };

                    poleUI[r, s] = tb;
                    panelDeska.Controls.Add(tb);
                }
            }
        }

        // Metoda vytvoří pravý panel s informacemi a tlačítky
        private void InicializujUI()
        {
            int panelX = 620;

            // Zobrazení aktuálního hráče
            lblNaTahu = new Label { Text = "NA TAHU", Location = new Point(panelX, 20), Size = new Size(200, 30), Font = new Font("Segoe UI", 14, FontStyle.Bold), ForeColor = Color.White };
            this.Controls.Add(lblNaTahu);

            // Seznam všech hráčů a bodů
            lbHraci = new ListBox { Location = new Point(panelX, 60), Size = new Size(200, 100), BackColor = Color.FromArgb(45, 45, 45), ForeColor = Color.White, BorderStyle = BorderStyle.None, Font = new Font("Segoe UI", 11) };
            this.Controls.Add(lbHraci);

            // Tlačítko pro potvrzení tahu
            Button btnOk = new Button { Text = "POTVRDIT TAH", Location = new Point(panelX, 170), Size = new Size(200, 45), BackColor = Color.SeaGreen, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            btnOk.Click += PotvrditTah;
            this.Controls.Add(btnOk);

            // Tlačítko pro smazání rozepsaného tahu
            Button btnClear = new Button { Text = "VYMAZAT TAH", Location = new Point(panelX, 225), Size = new Size(200, 35), BackColor = Color.IndianRed, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnClear.Click += (s, e) => SmazatAktualniTah();
            this.Controls.Add(btnClear);

            // Legenda barev
            Label lblLegendaNadpis = new Label { Text = "LEGENDA POLÍ:", Location = new Point(panelX, 280), ForeColor = Color.White, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            this.Controls.Add(lblLegendaNadpis);

            VytvorPolozkuLegendy(panelX, 310, Color.FromArgb(255, 80, 80), "3W - Trojnásobné slovo");
            VytvorPolozkuLegendy(panelX, 335, Color.FromArgb(255, 180, 180), "2W - Dvojnásobné slovo");
            VytvorPolozkuLegendy(panelX, 360, Color.FromArgb(60, 120, 255), "3L - Trojnásobné písmeno");
            VytvorPolozkuLegendy(panelX, 385, Color.FromArgb(170, 210, 255), "2L - Dvojnásobné písmeno");
            VytvorPolozkuLegendy(panelX, 410, Color.FromArgb(255, 230, 100), "STŘED - Startovní pole");

            // Historie tahů
            Label lblHistNadpis = new Label { Text = "HISTORIE TAHŮ", Location = new Point(panelX + 220, 20), Size = new Size(200, 30), Font = new Font("Segoe UI", 12, FontStyle.Bold), ForeColor = Color.White };
            this.Controls.Add(lblHistNadpis);
            lbHistorie = new ListBox { Location = new Point(panelX + 220, 60), Size = new Size(260, 400), BackColor = Color.FromArgb(45, 45, 45), ForeColor = Color.LightGray, BorderStyle = BorderStyle.None, Font = new Font("Segoe UI", 9) };
            this.Controls.Add(lbHistorie);
        }

        // Pomocná metoda pro vykreslení řádku v legendě
        private void VytvorPolozkuLegendy(int x, int y, Color barva, string text)
        {
            Panel p = new Panel { Location = new Point(x, y), Size = new Size(15, 15), BackColor = barva };
            Label l = new Label { Text = text, Location = new Point(x + 20, y - 2), Size = new Size(180, 20), ForeColor = Color.Silver, Font = new Font("Segoe UI", 8) };
            this.Controls.Add(p);
            this.Controls.Add(l);
        }

        // --- HLAVNÍ LOGIKA PRO POTVRZENÍ TAHU ---
        private void PotvrditTah(object sender, EventArgs e)
        {
            List<Point> nove = new List<Point>();
            bool deskaBylaPrazdna = true;

            // 1. KROK: Projdeme desku a najdeme písmena, která hráč právě položil
            for (int r = 0; r < 15; r++)
            {
                for (int s = 0; s < 15; s++)
                {
                    if (poleLogika[r, s].JeZafixovano == true)
                    {
                        deskaBylaPrazdna = false;
                    }
                    if (poleLogika[r, s].JeZafixovano == false && string.IsNullOrEmpty(poleLogika[r, s].Pismeno) == false)
                    {
                        nove.Add(new Point(r, s));
                    }
                }
            }

            // Pokud hráč nic nepoložil, nic neděláme
            if (nove.Count == 0)
            {
                return;
            }

            // 2. KROK: Kontrola limitu 7 písmen (Bingo limit)
            if (nove.Count > 7)
            {
                MessageBox.Show("Chyba: V jednom tahu můžete položit maximálně 7 písmen!");
                return;
            }

            // 3. KROK: Kontrola linearity (vše v jedné řadě nebo sloupci)
            bool radekStejny = nove.All(p => p.X == nove[0].X);
            bool sloupecStejny = nove.All(p => p.Y == nove[0].Y);
            if (radekStejny == false && sloupecStejny == false)
            {
                MessageBox.Show("Písmena musí být v jedné přímce (ne diagonálně)!");
                return;
            }

            // 4. KROK: Kontrola navazování
            bool navazuje = false;
            if (deskaBylaPrazdna == true)
            {
                // První tah hry musí jít přes střed (7,7)
                navazuje = nove.Any(p => p.X == 7 && p.Y == 7);
            }
            else
            {
                // Další tahy musí sousedit s už položeným písmenem
                navazuje = nove.Any(p => SousediSeStarym(p.X, p.Y));
            }

            if (navazuje == false)
            {
                string msg = deskaBylaPrazdna ? "První slovo musí procházet středem!" : "Slovo musí navazovat na existující písmena!";
                MessageBox.Show(msg);
                return;
            }

            // 5. KROK: Výpočet bodů (včetně Bingo bonusu a křížení slov)
            int bodyTah = VypocitejSkore(nove, out string seznamSlov);

            // Připsání bodů aktivnímu hráči
            Hrac h = hraci[aktualniHracIndex];
            h.Skore += bodyTah;

            // Zápis do historie
            lbHistorie.Items.Insert(0, h.Jmeno + ": " + seznamSlov + " (+" + bodyTah + "b)");

            // Uzamknutí písmen na desce
            ZafixujPismena();

            // Přepnutí hráče a aktualizace zobrazení
            aktualniHracIndex = (aktualniHracIndex + 1) % hraci.Count;
            AktualizujVse();
        }

        // Pomocná metoda pro kontrolu sousedství
        private bool SousediSeStarym(int r, int s)
        {
            int[] dr = { -1, 1, 0, 0 }; // Nahoru, Dolů
            int[] ds = { 0, 0, -1, 1 }; // Vlevo, Vpravo
            for (int i = 0; i < 4; i++)
            {
                int nr = r + dr[i];
                int ns = s + ds[i];
                if (nr >= 0 && nr < 15 && ns >= 0 && ns < 15)
                {
                    if (poleLogika[nr, ns].JeZafixovano == true)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        // Metoda vypočítá celkové skóre za tah
        private int VypocitejSkore(List<Point> nove, out string slovaText)
        {
            int celkem = 0;
            List<string> nalezenaSlova = new List<string>();

            // Zjistíme směr hlavního slova
            bool jeHorizontalni = nove.All(p => p.X == nove[0].X);

            // 1. Spočítáme hlavní slovo
            celkem += SpoctiJednoSlovo(nove[0].X, nove[0].Y, jeHorizontalni, out string hlavni);
            if (string.IsNullOrEmpty(hlavni) == false)
            {
                nalezenaSlova.Add(hlavni);
            }

            // 2. Spočítáme křížová slova (kolmo na hlavní slovo)
            foreach (var p in nove)
            {
                int bodyKriz = SpoctiJednoSlovo(p.X, p.Y, !jeHorizontalni, out string kriz);
                if (bodyKriz > 0)
                {
                    celkem += bodyKriz;
                    nalezenaSlova.Add(kriz);
                }
            }

            // 3. BINGO BONUS (Pokud hráč vyložil přesně 7 písmen)
            if (nove.Count == 7)
            {
                celkem += 50;
                nalezenaSlova.Add("BINGO BONUS (+50b)");
            }

            slovaText = string.Join(", ", nalezenaSlova);
            return celkem;
        }

        // Metoda najde a spočítá body pro jedno slovo v daném směru
        private int SpoctiJednoSlovo(int r, int s, bool horiz, out string text)
        {
            int startR = r;
            int startS = s;

            // Najdeme začátek slova
            if (horiz == true)
            {
                while (startS > 0 && string.IsNullOrEmpty(poleLogika[startR, startS - 1].Pismeno) == false)
                {
                    startS--;
                }
            }
            else
            {
                while (startR > 0 && string.IsNullOrEmpty(poleLogika[startR - 1, startS].Pismeno) == false)
                {
                    startR--;
                }
            }

            int sumaBody = 0;
            int nasobitelSlova = 1;
            int delka = 0;
            text = "";

            int currR = startR;
            int currS = startS;

            // Procházíme písmena a sčítáme body + aplikujeme bonusy polí
            while (currR < 15 && currS < 15 && string.IsNullOrEmpty(poleLogika[currR, currS].Pismeno) == false)
            {
                Policko p = poleLogika[currR, currS];
                int pBody = ScrabbleLogika.ZiskejBodovouHodnotu(p.Pismeno);

                // Bonusy polí se počítají jen pro nová písmena
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

            // Pokud má "slovo" jen jedno písmeno, není to slovo (body = 0)
            if (delka < 2)
            {
                text = "";
                return 0;
            }

            return sumaBody * nasobitelSlova;
        }

        // Vymaže aktuálně rozepsaný tah z desky
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
                    }
                }
            }
        }

        // Zafixuje potvrzená písmena (zešednou a nejdou měnit)
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
                        poleUI[r, s].ForeColor = Color.DimGray;
                    }
                }
            }
        }

        // Aktualizace tabulky skóre a nápisu "Na tahu"
        private void AktualizujVse()
        {
            lbHraci.Items.Clear();
            foreach (var h in hraci)
            {
                lbHraci.Items.Add(h.ToString());
            }
            lblNaTahu.Text = "NA TAHU: " + hraci[aktualniHracIndex].Jmeno;
        }

        // Rozložení bonusových polí na herní desce
        private string ZjistiBonus(int r, int s)
        {
            if (r == 7 && s == 7) return "start";
            if ((r == 0 || r == 7 || r == 14) && (s == 0 || s == 7 || s == 14)) return "3W";
            if (r == s || r + s == 14)
            {
                if ((r >= 1 && r <= 4) || (r >= 10 && r <= 13)) return "2W";
            }
            if ((r == 1 || r == 13) && (s == 5 || s == 9) || (r == 5 || r == 9) && (s == 1 || s == 13) || (r == 5 || r == 9) && (s == 5 || s == 9)) return "3L";
            if ((r == 0 || r == 14) && (s == 3 || s == 11) || (r == 2 || r == 12) && (s == 6 || s == 8) || (r == 3 || r == 11) && (s == 0 || s == 14) || (r == 6 || r == 8) && (s == 2 || s == 12) || (r == 7 && (s == 3 || s == 11)) || (s == 7 && (r == 3 || r == 11))) return "2L";
            return "zadny";
        }
    }
}