using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace osusb1 {
partial class all {

	[STAThread]
	static void Main() {
		CultureInfo customCulture = (CultureInfo) Thread.CurrentThread.CurrentCulture.Clone();
		customCulture.NumberFormat.NumberDecimalSeparator = ".";
		Thread.CurrentThread.CurrentCulture = customCulture;
		p = new Projection();
		zs = new List<Z>();
		init();
		Application.EnableVisualStyles();
		Application.SetCompatibleTextRenderingDefault(false);
		Application.Run(new form());
	}

	static string path = @"S:\games\osu!\Songs\Renard - System Failure";
	static string osb = path + @"\Renard - System Failure (yugecin).osb";
	static string osbt = path + @"\Renard - System Failure (yugecin).osbt";

	static List<Z> zs;
	static Projection p;

	static void init() {
		zs.Clear();
		zs.Add(new Z0010spect(00000, 20000));
		zs.Add(new Ztestcube2(20000, 40000));
	}

	internal
	static void render(int time, Graphics g) {
		if (g != null) {
			g.FillRectangle(new SolidBrush(Color.Black), 0, 0, 640, 480);
		}
		p.Update(time);

		foreach (Z z in zs) {
			if (z.start <= time && time < z.stop) {
				int reltime = time - z.start;
				z.draw(new SCENE(z.start, z.stop, time, g));
			}
		}
	}

	internal
	static void export(int fps, int fromtime, int totime) {
		int interval = 1000 / fps;
		for (int i = fromtime; i < totime; i += interval) {
			render(i, null);
		}
		using (StreamWriter w = new StreamWriter(osb)) {
			using (StreamReader r = new StreamReader(osbt)) {
				string s;
				while ((s = r.ReadLine()) != null) {
					w.Write(s + "\n");
				}
			}
			Writer writer = new Writer(w);
			fin(writer);
		}
	}

	static void fin(Writer w) {
		foreach (Z z in zs) {
			z.fin(w);
		}
	}

}

partial class form : Form {

	public form() {
		InitializeComponent();
	}

	void panel1_Paint(object sender, PaintEventArgs e) {
		/*
		Bitmap bm = new Bitmap(panel1.Width, panel1.Height);
		all.render((int) nuptime.Value, Graphics.FromImage(bm));
		e.Graphics.DrawImage(bm, 0, 0);
		*/
		all.render((int) nuptime.Value, e.Graphics);
	}

	void nuptime_ValueChanged(object sender, EventArgs e) {
		panel1.Invalidate();
	}

	void button1_Click(object sender, EventArgs e) {
		timer1.Enabled = !timer1.Enabled;
	}

	void timer1_Tick(object sender, EventArgs e) {
		nuptime.Value = (int) nuptime.Value + timer1.Interval;
	}

	void UI_ExportRequest(object sender, EventArgs e) {
		all.export((int) numericUpDown1.Value, (int) numericUpDown2.Value, (int) numericUpDown3.Value);
	}
}
}
