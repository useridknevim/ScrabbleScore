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
    public partial class MenuForm : Form
    {
        private NumericUpDown numHraci;
        private FlowLayoutPanel panelJmena;
        private List<TextBox> textboxyJmen = new List<TextBox>();

        public MenuForm()
        {
            InitializeComponent();
            this.Text = "Scrabble Score Master";
            this.Size = new Size(400, 600);
            this.BackColor = Color.FromArgb(30, 30, 30);
            this.ForeColor = Color.White;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            Label lblNadpis = new Label
            {
                Text = "SCRABBLE",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                Location = new Point(50, 20),
                Size = new Size(300, 50),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.FromArgb(0, 200, 150)
            };

            Label lblPocet = new Label { Text = "Počet hráčů:", Location = new Point(50, 90), Size = new Size(120, 20), Font = new Font("Segoe UI", 10) };

            numHraci = new NumericUpDown
            {
                Location = new Point(200, 88),
                Minimum = 2,
                Maximum = 4,
                Value = 2,
                BackColor = Color.FromArgb(45, 45, 45),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 10)
            };
            numHraci.ValueChanged += ObnovitPoleProJmena;

            panelJmena = new FlowLayoutPanel
            {
                Location = new Point(50, 130),
                Size = new Size(300, 300),
                FlowDirection = FlowDirection.TopDown,
                AutoScroll = true
            };

            Button btnStart = new Button
            {
                Text = "ODSTARTOVAT HRU",
                Location = new Point(75, 460),
                Size = new Size(250, 60),
                BackColor = Color.FromArgb(0, 150, 136),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12, FontStyle.Bold)
            };
            btnStart.FlatAppearance.BorderSize = 0;
            btnStart.Click += BtnStart_Click;

            this.Controls.Add(lblNadpis);
            this.Controls.Add(lblPocet);
            this.Controls.Add(numHraci);
            this.Controls.Add(panelJmena);
            this.Controls.Add(btnStart);

            ObnovitPoleProJmena(null, null);
        }

        private void ObnovitPoleProJmena(object sender, EventArgs e)
        {
            panelJmena.Controls.Clear();
            textboxyJmen.Clear();

            for (int i = 0; i < (int)numHraci.Value; i++)
            {
                string placeholder = "Hráč " + (i + 1);
                Label lbl = new Label { Text = "Jméno " + (i + 1) + ". hráče:", Width = 280, Margin = new Padding(0, 10, 0, 0), Font = new Font("Segoe UI", 9) };

                TextBox tb = new TextBox
                {
                    Width = 280,
                    Text = placeholder,
                    Tag = placeholder,
                    BackColor = Color.FromArgb(45, 45, 45),
                    ForeColor = Color.Gray,
                    BorderStyle = BorderStyle.FixedSingle,
                    Font = new Font("Segoe UI", 11)
                };

                tb.Enter += (s, ev) => {
                    TextBox t = (TextBox)s;
                    if (t.Text == (string)t.Tag)
                    {
                        t.Text = "";
                        t.ForeColor = Color.White;
                    }
                };

                tb.Leave += (s, ev) => {
                    TextBox t = (TextBox)s;
                    if (string.IsNullOrWhiteSpace(t.Text))
                    {
                        t.Text = (string)t.Tag;
                        t.ForeColor = Color.Gray;
                    }
                };

                panelJmena.Controls.Add(lbl);
                panelJmena.Controls.Add(tb);
                textboxyJmen.Add(tb);
            }
        }

        private void BtnStart_Click(object sender, EventArgs e)
        {
            List<Hrac> seznamHracu = new List<Hrac>();
            foreach (var tb in textboxyJmen)
            {
                if (string.IsNullOrWhiteSpace(tb.Text) || tb.Text == (string)tb.Tag)
                    seznamHracu.Add(new Hrac((string)tb.Tag));
                else
                    seznamHracu.Add(new Hrac(tb.Text));
            }

            Form1 hlavniOkno = new Form1(seznamHracu);
            hlavniOkno.Show();
            this.Hide();
            hlavniOkno.FormClosed += (s, args) => this.Close();
        }
    }
}
