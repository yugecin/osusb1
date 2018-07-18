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
partial class form : Form {

	public form() {
		InitializeComponent();
		this.Text = all.osb;
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
		((Control) sender).Enabled = false;
		all.export((int) numericUpDown1.Value, (int) numericUpDown2.Value, (int) numericUpDown3.Value);
		((Control) sender).Enabled = true;
	}

	bool pmousedown = false;
	Point plastval;
	Point pmouse;
	private void panel1_MouseDown(object sender, MouseEventArgs e) {
		pmousedown = true;
		pmouse = new Point(e.Location.X, e.Location.Y);
	}

	private void panel1_MouseMove(object sender, MouseEventArgs e) {
		if (pmousedown) {
			all.mousex = e.Location.X - pmouse.X + plastval.X;
			all.mousey = -(e.Location.Y - pmouse.Y) + plastval.Y;
			panel1.Refresh();
		}
	}

	private void panel1_MouseUp(object sender, MouseEventArgs e) {
		pmousedown = false;
		if (e.Button == System.Windows.Forms.MouseButtons.Right) {
			all.mousex = all.mousey = 0;
		}
		plastval = new Point(all.mousex, all.mousey);
		panel1.Refresh();
	}
}

partial class all {

	[STAThread]
	static void Main() {
		CultureInfo customCulture = (CultureInfo) Thread.CurrentThread.CurrentCulture.Clone();
		customCulture.NumberFormat.NumberDecimalSeparator = ".";
		Thread.CurrentThread.CurrentCulture = customCulture;
		p = new Projection();
		fft = new FFT();
		zs = new List<Z>();
		init();
		Application.EnableVisualStyles();
		Application.SetCompatibleTextRenderingDefault(false);
		Application.Run(new form());
	}

	public static string path = @"S:\games\osu!\Songs\sky_delta - Exordium";
	public static string osb = path + @"\sky_delta - Exordium (yugecin).osb";
	public static string osbt = path + @"\sky_delta - Exordium (yugecin).osbt";

	static List<Z> zs;
	static Projection p;
	static FFT fft;

	public static int mousex;
	public static int mousey;

	static void init() {
		zs.Clear();
		zs.Add(new Zrub(00000, 50000));
		zs.Add(new Z0010spect(50000, 60000));
		zs.Add(new Ztestcube2(60000, 70000));
		zs.Add(new Ztor(70000, 80000));
	}

	internal
	static void render(int time, Graphics g) {
		if (g != null) {
			g.FillRectangle(new SolidBrush(Color.Black), 0, 0, 640, 480);
		}

		p.Update(time);
		fft.Update(time);

		foreach (Z z in zs) {
			if (z.start <= time && time < z.stop) {
				int reltime = time - z.start;
				z.draw(new SCENE(z.start, z.stop, time, g));
			}
		}
	}

	internal
	static void export(int fromtime, int totime, int fps) {
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
		w.ln("4,3,1,,NaN,-∞");
		w.ln("");
	}

}
}
