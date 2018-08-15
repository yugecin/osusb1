using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace osusb1 {
partial class all {
	static void loadobj(string name, out vec3[] pts, out Rect[] rects) {
		List<vec3> v = new List<vec3>();
		List<Rect> r = new List<Rect>();
		using (StreamReader i = new StreamReader(name + ".obj")) {
			string line;
			while ((line = i.ReadLine()) != null) {
				if (line.StartsWith("v ")) {
					string[] prts = line.Split(' ');
					v.Add(v3(prts[1], prts[2], prts[3]));
					continue;
				}
				if (!line.StartsWith("f ")) {
					continue;
				}
				string[] z = line.Split('/', ' ');
				int a = int.Parse(z[1]) - 1;
				int b = int.Parse(z[4]) - 1;
				int d = int.Parse(z[7]) - 1;
				int c = int.Parse(z[10]) - 1;
				r.Add(new Rect(null, Color.Red, null, a, b, c, d));
			}
		}
		pts = new vec3[v.Count];
		rects = new Rect[r.Count];
		for (int i = 0; i < pts.Length; i++) {
			pts[i] = v[i];
		}
		for (int i = 0; i < rects.Length; i++) {
			rects[i] = r[i];
			rects[i].pts = pts;
			rects[i].tri1.points = pts;
			rects[i].tri2.points = pts;
		}
	}
}
}
