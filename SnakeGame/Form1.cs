using MetroFramework.Forms;
using MetroFramework.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SnakeGame
{
    public partial class Form1 : Form
    {
        int satirSayisi = 20;
        int sutunSayisi = 20;
        int bogumBoyut;
        List<Point> yilan;
        int xYon = 1;
        int yYon = 0;
        int xYeniYon, yYeniYon;
        bool yonDegisti = false;
        Point yem;
        Random rnd = new Random();
        bool oyunBittiMi = false;
        bool kolayMi = false;
        int puan = 0;
        string oyuncu;
        List<string> skorlar = new List<string>();
        
        public Form1()
        {
            EskiSkorlariOku();
            InitializeComponent();
            TitremeyiAzalt();
            bogumBoyut = saha.Width / sutunSayisi;
            YilanOlustur();
            YemUret();
        }

        private void EskiSkorlariOku()
        {
            try
            {
                skorlar = File.ReadAllLines("skorlar.txt").ToList();
            }
            catch (Exception)
            {

               
            }
        }

        private void TitremeyiAzalt()
        {
            typeof(Panel).InvokeMember("DoubleBuffered",
                BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                null, saha, new object[] { true });
        }

        private void YemUret()
        {
            int x, y;
            do
            {

                x = rnd.Next(0, sutunSayisi);
                y = rnd.Next(0, satirSayisi);


            } while (YilaninUzerindeMi(x, y));
            yem = new Point(x, y);
        }

        private bool YilaninUzerindeMi(int x, int y)
        {
            foreach (Point nokta in yilan)
            {
                if (nokta.X == x && nokta.Y == y)
                {
                    return true;
                }
            }
            return false;

        }

        private void YilanOlustur()
        {
            Point orta = new Point((sutunSayisi / 2), (satirSayisi / 2));
            yilan = new List<Point>
            {
                orta,
                new Point(orta.X - 1 , orta.Y),
                new Point(orta.X - 2 , orta.Y),

            };
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.F2)
            {
                if (oyunBittiMi)
                {
                    OyunuYenidenBaslat();
                    return base.ProcessCmdKey(ref msg, keyData);

                }
                if (timer1.Enabled)
                {
                    timer1.Enabled = false;
                    lblDurdu.Show();
                }
                else
                {
                    timer1.Enabled = true;
                    lblAciklama.Hide();
                    lblDurdu.Hide();
                }
            }
            if (yonDegisti || !timer1.Enabled)
                return base.ProcessCmdKey(ref msg, keyData);


            switch (keyData)
            {
                case Keys.Up:
                    xYeniYon = 0;
                    yYeniYon = -1;
                    break;
                case Keys.Down:
                    xYeniYon = 0;
                    yYeniYon = 1;
                    break;
                case Keys.Right:
                    xYeniYon = 1;
                    yYeniYon = 0;
                    break;
                case Keys.Left:
                    xYeniYon = -1;
                    yYeniYon = 0;
                    break;
                default:
                    return base.ProcessCmdKey(ref msg, keyData);
            }
            if (xYeniYon * xYon != -1 && yYeniYon * yYon != -1)
            {
                xYon = xYeniYon;
                yYon = yYeniYon;
                yonDegisti = true;
            }


            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void OyunuYenidenBaslat()
        {
            puan = 0;
            lblPuan.Text = "000";
            oyunBittiMi = false;
            yonDegisti = false;
            xYon = 1;
            yYon = 0;
            lblOyunBitti.Hide();
            YilanOlustur();
            YemUret();
            saha.Refresh();
            timer1.Interval = 100;
            timer1.Enabled = true;
            
        }

        private void saha_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;



            YilanCiz(g);
            YemCiz(g);


        }

        private void YemCiz(Graphics g)
        {
            int x = yem.X * bogumBoyut;
            int y = yem.Y * bogumBoyut;
            g.FillRectangle(Brushes.Red, x, y, bogumBoyut, bogumBoyut);
            g.DrawRectangle(Pens.White, x, y, bogumBoyut - 1, bogumBoyut - 1);

        }

        private void YilanCiz(Graphics g)
        {
            BogumCiz(g, yilan[0].X, yilan[0].Y, true); // Baş

            for (int i = 1; i < yilan.Count; i++)
            {
                BogumCiz(g, yilan[i].X, yilan[i].Y);
            }
        }

        private void BogumCiz(Graphics g, int sutunNo, int satirNo, bool basMi = false)
        {
            int x = sutunNo * bogumBoyut;
            int y = satirNo * bogumBoyut;
            g.FillRectangle(basMi ? Brushes.Gray : Brushes.White, x, y, bogumBoyut, bogumBoyut);
            g.DrawRectangle(Pens.Gray, x, y, bogumBoyut - 1, bogumBoyut - 1);

        }

        private void tsmiKolay_Click(object sender, EventArgs e)
        {
            kolayMi = true;
            tsmiKolay.Checked = true;
            tsmiZor.Checked = false;

        }

        private void tsmiZor_Click(object sender, EventArgs e)
        {
            kolayMi = false;
            tsmiKolay.Checked = false;
            tsmiZor.Checked = true;
        }

        private void tsmiSkorlar_Click(object sender, EventArgs e)
        {
            SkorlarForm frmSkorlar = new SkorlarForm();
            frmSkorlar.ShowDialog();
        }

        private void btnOyunuBaslat_Click(object sender, EventArgs e)
        {
            string ad = txtOyuncu.Text.Replace(";", "").Trim();
            if (ad == "")
            {
                MessageBox.Show("Lütfen adınızı girin.");
                return;
            }
            oyuncu = ad;
            Text = "Yılan Oyunu - " + ad;
            Refresh();
            pnlGiris.Hide();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Point bas = yilan[0];
            
            Point yeniBas = kolayMi
                 ? new Point((bas.X + 1 * xYon + sutunSayisi) % sutunSayisi, (bas.Y + 1 * yYon + satirSayisi) % satirSayisi)
                 : new Point(bas.X + xYon, bas.Y + yYon);
            if (YilaninUzerindeMi(yeniBas.X, yeniBas.Y) || yeniBas.X < 0 || yeniBas.X >= sutunSayisi || yeniBas.Y < 0 || yeniBas.Y >= sutunSayisi)
            {
                SkorKaydet();
                timer1.Enabled = false;
                lblOyunBitti.Text = string.Format("OYUN BİTTİ\r\nSKORUNUZ: {0}\r\nTEKRAR OYNA (F2)", puan);
                lblOyunBitti.Show();
                oyunBittiMi = true;
                return;
            }
            yilan.Insert(0, yeniBas);
            if (yeniBas == yem)
            {
                puan += kolayMi ? 5 : 10;
                YemUret();
                if (timer1.Interval > 50)
                {
                    timer1.Interval -= 10;
                }
            }
            else
            {
                yilan.RemoveAt(yilan.Count - 1);

            }
            lblPuan.Text = puan.ToString("000");
            saha.Refresh();
            yonDegisti = false;

        }

        private void SkorKaydet()
        {
            string skorMetin = string.Format("{0:00000};{1};{2}", puan, DateTime.Now.ToString("s"), oyuncu);
            skorlar.Add(skorMetin);
            skorlar.Sort();
            skorlar.Reverse();
            File.WriteAllLines("skorlar.txt", skorlar);
        }
    }
}
