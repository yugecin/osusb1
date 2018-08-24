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
		this.Text = all.osb;
		all.Widescreen = chkwidescreen.Checked;
		nuptime.Value = 138200;
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

	private void chkwidescreen_CheckedChanged(object sender, EventArgs e) {
		all.Widescreen = chkwidescreen.Checked;
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
		all.Widescreen = chkwidescreen.Checked;
		all.processPhantom = chkPhantom.Checked;
		all.export(chkComments.Checked);
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
			all.mouse.x = e.Location.X - pmouse.X + plastval.X;
			all.mouse.y = -(e.Location.Y - pmouse.Y) + plastval.Y;
			panel1.Refresh();
		}
	}

	private void panel1_MouseUp(object sender, MouseEventArgs e) {
		pmousedown = false;
		if (e.Button == System.Windows.Forms.MouseButtons.Right) {
			all.mouse.x = all.mouse.y = 0;
		}
		plastval = all.mouse.point();
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
		font = new Font();
		init();
		Application.EnableVisualStyles();
		Application.SetCompatibleTextRenderingDefault(false);
		Application.Run(new form());
	}

	public static string path = @"S:\games\osu!\Songs\Jeremy Blake - Flex";
	public static string osb = path + @"\Jeremy Blake - Flex (yugecin).osb";
	public static string osbt = path + @"\Jeremy Blake - Flex (yugecin).osbt";

	static List<Z> zs;
	static Projection p;
	static FFT fft;
	static Font font;

	static bool rendering;
	static bool isPhantomFrame;
	public static bool processPhantom;

	public static vec2 mouse = v2(0f);

	public static int[] udata = new int[8];

	static void init() {
		//zs.Clear();
		//zs.Add(new Zsc(16200, 69150));
		//zs.Add(new Ztunnel(16200, 52000));
		//zs.Add(new Z0020spect(17100, 31900));
		//zs.Add(new Z002Bspect(17300, 22000));
		//zs.Add(new Z002Cspect(22000, 31900));
		//zs.Add(new Zwaves(51750, 69150));
		//zs.Add(new Zrub(31900, 69150));
		//zs.Add(new Zheart(69150, 102900));
		//zs.Add(new Zgreet(86500, 101700));
		//zs.Add(new Ztorfield(102900, 121000));
		zs.Add(new Zlc(121000, 138200));
		zs.Add(new Zcheckerboard(121000, 138200));
		zs.Add(new Zltext(121000, 129800, "Robin"));
		zs.Add(new Zltext(129800, 138200, "Emily"));
		zs.Add(new Zstarfield(138200, 155700));
		zs.Add(new Ztor(138200, 155700));
		//zs.Add(new Zstart(00000, 36000));
		//zs.Add(new Zdebugdot(00000, 5000));
		//zs.Add(new Zdebugdot2(00000, 70000));
		//zs.Add(new Ztestcube3(00000, 20000));
		//zs.Add(new Ztestcube4(00000, 20000));
		//zs.Add(new Ztestfont(00000, 20000));
		//zs.Add(new Ztestfont2(00000, 10000));
		//zs.Add(new Z0010spect(50000, 60000));
		//zs.Add(new Ztestcube2(00000, 10000));
		foreach (Z z in zs) {
			if (z.framedelta == 0) {
				throw new Exception("framedelta for " + z.GetType().Name);
			}
			if (z.phantomframedelta == 0) {
				z.phantomframedelta = z.framedelta;
			}
		}
	}

	internal
	static void render(int time, Graphics g) {
		if (g != null) {
			g.TranslateTransform(107f, 0f);
			g.FillRectangle(new SolidBrush(Color.Black), LOWERBOUND, 0, UPPERBOUND - LOWERBOUND * 2, 480);
		}

		p.Update(time);
		fft.Update(time);

		foreach (Z z in zs) {
			if (z.start <= time && time < z.stop) {
				isPhantomFrame = false;
				if (rendering && time % z.framedelta != 0) {
					if (!processPhantom || time % z.phantomframedelta != 0) {
						continue;
					}
					isPhantomFrame = true;
				}
				int reltime = time - z.start;
				z.draw(new SCENE(z.start, z.stop, time, g));
			}
		}

		if (g != null && !Widescreen) {
			Widescreen = true;
			var b = new SolidBrush(SystemColors.Control);
			g.FillRectangle(b, LOWERBOUND, 0, -LOWERBOUND, 480);
			g.FillRectangle(b, UPPERBOUND + LOWERBOUND, 0, -LOWERBOUND, 480);
			Widescreen = false;
		}
	}

	internal
	static void export(bool comments) {
		mouse.x = 0;
		mouse.y = 0;
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
		int nextprogress = 5;
		rendering = true;
		for (int i = mintime; i < maxtime; i += 5) {
			int progress = (i - mintime) * 100 / (maxtime - mintime);
			if (progress >= nextprogress) {
				Console.Write("{0}% ", progress);
				nextprogress += 5;
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
			Writer writer = new Writer(w, comments);
			fin(writer);
		}
		foreach (string sprite in Sprite.usagedata.Keys) {
			Console.WriteLine("sprite '{0}': {1}", sprite, Sprite.usagedata[sprite]);
		}
		Console.WriteLine(
			"easing results: {0} success {1} failure {2} commands saved",
			Sprite.easeResultSuccess,
			Sprite.easeResultFailed,
			Sprite.easeCommandsSaved
		);
		Console.WriteLine("Done");
	}

	static void fin(Writer w) {
		int totalbytes = 0;
		foreach (Z z in zs) {
			Sprite.framedelta = z.framedelta;
			w.byteswritten = 0;
			w.comment(z.GetType().Name);
			z.fin(w);
			Console.WriteLine(
				"scene '{0}' @ {1}fps ({2}): {3}KB",
				z.GetType().Name,
				1000 / z.framedelta,
				1000 / z.phantomframedelta,
				w.byteswritten / 1000f
			);
			totalbytes += w.byteswritten;
		}
		Console.WriteLine("{0}KB / {1}KiB", totalbytes / 1000f, totalbytes / 1024f);
		w.ln("4,3,1,,NaN,-∞");
		w.ln("");
	}

}
}
