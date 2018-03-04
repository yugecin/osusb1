using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace osusb1 {
	public partial class Form2 : Form {

		private static string path = @"S:\games\osu!\Songs\Professor Kliq - Surfs Up (Eat Shit)";
		private static string osb = path + @"\Professor Kliq - Surfs Up (Eat Shit) (yugecin).osb";
		private static string osbt = path + @"\Professor Kliq - Surfs Up (Eat Shit) (yugecin).osbt";
		private Form1 f1;

		public Form2(Form1 f1) {
			this.f1 = f1;
			InitializeComponent();
		}

		private void btnexport_Click(object sender, EventArgs e) {
			int interval = (int) (1000 / numericUpDown3.Value);
			for (int i = (int) numericUpDown1.Value; i < (int) numericUpDown2.Value; i += interval) {
				f1.render(i, null);
			}
			using (StreamWriter w = new StreamWriter(osb)) {
				using (StreamReader r = new StreamReader(osbt)) {
					string s;
					while ((s = r.ReadLine()) != null) {
						w.Write(s + "\n");
					}
				}
				Writer writer = new Writer(w);
				f1.fin(writer);
			}
			this.Close();
		}

	}
}
