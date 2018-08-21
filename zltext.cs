using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace osusb1 {
partial class all {
	class Zltext : Z {

		vec3[] points;
		vec3[] _points;
		Rect[] rects;
		Orect[] orects;

		public Zltext(int start, int stop) {
			this.start = start;
			this.stop = stop;
			framedelta = 100;

			string text = "Robin";

			int width = font.textWidth(text);
			int pointcount = font.calcPointCount(text);
			points = new vec3[pointcount * 8];
			_points = new vec3[points.Length];

			const int SIZE = 2;

			List<Rect> toprects = new List<Rect>();
			List<Rect> siderects = new List<Rect>();
			List<Rect> otherrects = new List<Rect>();
			Color[] cols = { Color.Cyan, Color.Lime, Color.Red, Color.Blue, Color.Yellow, Color.Orange };
			vec3 topleft = Zlc.mid - v3(width / 2f * SIZE, 0f, -(font.charheight) * SIZE);
			int pointidx = 0;
			for (int i = 0; i < text.Length; i++) {
				int c = text[i] - 32;
				vec3 pos = v3(topleft);
				for (int j = 0; j < font.charheight; j++) {
					int cw = font.charwidth[c];
					for (int k = 0; k < font.charwidth[c]; k++) {
						byte cd = font.chardata[c][j];
						if (((cd >> k) & 1) == 1) {
							vec3 basepoint = pos + v3(k * SIZE, 0f, 0f);
							new Pcube(points, pointidx).set(basepoint, SIZE, SIZE * 3, SIZE);
							Cube cube = new Cube(cols, _points, pointidx);

							otherrects.Add(cube.rects[Cube.F]);
							otherrects.Add(cube.rects[Cube.B]);
							if (k == 0 || ((cd >> (k - 1)) & 1) != 1) {
								siderects.Add(cube.rects[Cube.L]);
							}
							if (k == cw - 1 || ((cd >> (k + 1)) & 1) != 1) {
								siderects.Add(cube.rects[Cube.R]);
							}
							if (j == 0 || ((font.chardata[c][j - 1] >> k) & 1) != 1) {
								toprects.Add(cube.rects[Cube.U]);
							}
							if (j == font.charheight - 1 || ((font.chardata[c][j + 1] >> k) & 1) != 1) {
								siderects.Add(cube.rects[Cube.D]);
							}
							pointidx += 8;
						}
					}
					pos.z -= SIZE;
				}
				topleft.x += (font.charwidth[c] + 1) * SIZE;
			}

			int rectcount = 0;
			rects = new Rect[toprects.Count + siderects.Count + otherrects.Count];
			foreach (Rect r in toprects) {
				rects[rectcount++] = r;
			}
			foreach (Rect r in siderects) {
				rects[rectcount++] = r;
			}
			foreach (Rect r in otherrects) {
				rects[rectcount++] = r;
			}

			orects = new Orect[rects.Length];
			for (int i = 0; i < orects.Length; i++) {
				rects[i].pts = _points;
				rects[i].tri1.points = _points;
				rects[i].tri2.points = _points;
				orects[i] = new Orect(rects[i], 0);
			}
		}

		public override void draw(SCENE scene) {
			copy(_points, points);
			Zlc.adjust(_points);
			foreach (Orect r in orects) {
				r.update(scene);
			}
		}

		public override void fin(Writer w) {
			foreach (Orect r in orects) {
				r.fin(w);
			}
		}

	}
}
}
