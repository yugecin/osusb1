using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace osusb1 {
	public partial class Form1 : Form {

		List<Z> zs;
		Projection p;

		public Form1() {
			InitializeComponent();
			CultureInfo customCulture = (CultureInfo) Thread.CurrentThread.CurrentCulture.Clone();
			customCulture.NumberFormat.NumberDecimalSeparator = ".";
			Thread.CurrentThread.CurrentCulture = customCulture;
			p = new Projection();
			zs = new List<Z>();
			init();
		}

		void init() {
			zs.Clear();
			zs.Add(new Ztestcube2(00000, 20000));
		}

		public void render(int time, Graphics g) {
			if (g != null) {
				g.FillRectangle(new SolidBrush(Color.Black), 0, 0, 640, 480);
			}
			p.Update(time);

			foreach (Z z in this.zs) {
				if (z.start <= time && time < z.stop) {
					int reltime = time - z.start;
					z.draw(new SCENE(z.start, z.stop, time, p, g));
				}
			}
		}

		public void fin(Writer w) {
			foreach (Z z in this.zs) {
				z.fin(w);
			}
		}

		void panel1_Paint(object sender, PaintEventArgs e) {
			Bitmap bm = new Bitmap(panel1.Width, panel1.Height);
			render((int) nuptime.Value, Graphics.FromImage(bm));
			e.Graphics.DrawImage(bm, 0, 0);
		}

		void nuptime_ValueChanged(object sender, EventArgs e) {
			panel1.Invalidate();
		}

		private void button1_Click(object sender, EventArgs e) {
			timer1.Enabled = !timer1.Enabled;
		}

		private void timer1_Tick(object sender, EventArgs e) {
			nuptime.Value = (int) nuptime.Value + timer1.Interval;
		}

		private void btnexport_Click(object sender, EventArgs e) {
			new Form2(this).ShowDialog();
		}

	}
}
