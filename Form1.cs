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

		List<Z> zs = new List<Z>();
		Projection p = new Projection();

		public Form1() {
			InitializeComponent();
			CultureInfo customCulture = (CultureInfo) Thread.CurrentThread.CurrentCulture.Clone();
			customCulture.NumberFormat.NumberDecimalSeparator = ".";
			Thread.CurrentThread.CurrentCulture = customCulture;
			init();
		}

		void init() {
			zs.Add(new Ztestcube(10000, 20000));
		}

		void Render(int time, Graphics g) {
			g.FillRectangle(new SolidBrush(Color.Black), 0, 0, 640, 480);
			p.Update(time);

			foreach (Z z in this.zs) {
				if (z.start <= time && time < z.stop) {
					int reltime = time - z.start;
					z.draw(time, reltime, reltime / (float) (z.stop - z.start), p, g);
				}
			}
		}

		void panel1_Paint(object sender, PaintEventArgs e) {
			Render((int) nuptime.Value, e.Graphics);
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

	}
}
