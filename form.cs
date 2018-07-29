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
		trackBar1.ValueChanged += udata_ValueChanged;
		trackBar2.ValueChanged += udata_ValueChanged;
		trackBar3.ValueChanged += udata_ValueChanged;
		trackBar4.ValueChanged += udata_ValueChanged;
		trackBar5.ValueChanged += udata_ValueChanged;
		trackBar6.ValueChanged += udata_ValueChanged;
		trackBar7.ValueChanged += udata_ValueChanged;
		trackBar8.ValueChanged += udata_ValueChanged;
		this.Text = all.osb;
	}

	void udata_ValueChanged(object sender, EventArgs e) {
		string num = (sender as Control).Tag.ToString();
		int val = (sender as TrackBar).Value * 5;
		(Controls.Find("udata" + num, false)[0] as Label).Text = val.ToString();
		all.udata[int.Parse(num)] = val;
		panel1.Invalidate();
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
	static int framedelta;

	static bool rendering;
	static bool isPhantomFrame;

	public static int mousex;
	public static int mousey;

	public static int[] udata = new int[8];

	static void init() {
		zs.Clear();
		//zs.Add(new Zdebugdot(00000, 5000));
		//zs.Add(new Zwaves(00000, 20000));
		zs.Add(new Zrub(00000, 40000));
		zs.Add(new Z0010spect(50000, 60000));
		zs.Add(new Ztestcube2(60000, 70000));
		//zs.Add(new Ztor(70000, 80000));
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
				if (isPhantomFrame && !z.processPhantomFrames) {
					continue;
				}
				int reltime = time - z.start;
				z.draw(new SCENE(z.start, z.stop, time, g));
			}
		}
	}

	internal
	static void export(int fromtime, int totime, int fps) {
		mousex = 0;
		mousey = 0;
		int mintime = int.MaxValue;
		int maxtime = int.MinValue;
		foreach (Z z in zs) {
			if (z.start < mintime) {
				mintime = z.start;
			}
			if (z.stop > maxtime) {
				maxtime = z.stop;
			}
		}
		framedelta = 1000 / fps;
		int currentdelta = 0;
		int phantomdelta = 50;
		int nextprogress = 5;
		//mintime = fromtime;
		//maxtime = totime;
		rendering = true;
		for (int i = mintime; i < maxtime; i += phantomdelta, currentdelta += phantomdelta) {
			int progress = (i - mintime) * 100 / (maxtime - mintime);
			if (progress >= nextprogress) {
				Console.Write("{0}% ", progress);
				nextprogress += 5;
			}
			isPhantomFrame = currentdelta < framedelta;
			if (!isPhantomFrame) {
				currentdelta = 0;
			}
			render(i, null);
		}
		rendering = false;
		isPhantomFrame = false;
		Console.WriteLine("\nWriting...");
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
		foreach (string sprite in Sprite.usagedata.Keys) {
			Console.WriteLine("sprite '{0}': {1}", sprite, Sprite.usagedata[sprite]);
		}
		Console.WriteLine("Done");
	}

	static void fin(Writer w) {
		foreach (Z z in zs) {
			w.byteswritten = 0;
			z.fin(w);
			Console.WriteLine("scene '{0}': {1}KB", z.GetType().Name, w.byteswritten / 1000f);
		}
		w.ln("4,3,1,,NaN,-∞");
		w.ln("");
	}

}
}
