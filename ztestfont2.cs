using System;
using System.Drawing;
using System.Text;

namespace osusb1 {
partial class all {
	class Ztestfont2 : Z {

		vec3 mid = v3(0f, -30f, 100f);

		vec3[] points;
		vec3[] _points;
		Rect[] rects;
		Orect[] orects;
		int pointcount;

		const int
			PA = 1,
			PB = 0,
			PC = 4,
			PD = 5,
			PE = 3,
			PF = 2,
			PG = 6,
			PH = 7;

		public Ztestfont2(int start, int stop) {
			this.start = start;
			this.stop = stop;

			string text = "Hello!";

			int width = font.textWidth(text);
			int fullsize = (width - (text.Length - 1)) * font.charheight;
			pointcount = 0;
			points = new vec3[fullsize * 8];
			rects = new Rect[fullsize * 6];
			int rectcount = 0;

			const int SIZE = 2;

			Color[] cols = { Color.Cyan, Color.Lime, Color.Red, Color.Blue, Color.Yellow, Color.Orange };
			vec3 topleft = mid - v3(width / 2f * SIZE, 0f, -font.charheight / 2f * SIZE);
			for (int i = 0; i < text.Length; i++) {
				int c = text[i] - 32;
				vec3 pos = v3(topleft);
				for (int j = 0; j < font.charheight; j++) {
					int cw = font.charwidth[c];
					for (int k = 0; k < font.charwidth[c]; k++) {
						byte cd = font.chardata[c][j];
						if (((cd >> k) & 1) == 1) {
							int pidx = pointcount * 8;
							vec3 basepoint = pos + v3(k * SIZE, 0f, 0f);
							new Pcube(points, pidx).set(basepoint, SIZE, SIZE, SIZE);
							Cube cube = new Cube(cols, _points, pidx);

							rects[rectcount++] = cube.rects[Cube.F];
							rects[rectcount++] = cube.rects[Cube.B];
							if (k == 0 || ((cd >> (k - 1)) & 1) != 1) {
								rects[rectcount++] = cube.rects[Cube.L];
							}
							if (k == cw - 1 || ((cd >> (k + 1)) & 1) != 1) {
								rects[rectcount++] = cube.rects[Cube.R];
							}
							if (j == 0 || ((font.chardata[c][j - 1] >> k) & 1) != 1) {
								rects[rectcount++] = cube.rects[Cube.U];
							}
							if (j == font.charheight - 1 || ((font.chardata[c][j + 1] >> k) & 1) != 1) {
								rects[rectcount++] = cube.rects[Cube.D];
							}
							pointcount++;
						}
					}
					pos.z -= SIZE;
				}
				topleft.x += (font.charwidth[c] + 1) * SIZE;
			}

			_points = new vec3[pointcount * 8];
			orects = new Orect[rectcount];
			for (int i = 0; i < rectcount; i++) {
				rects[i].pts = _points;
				rects[i].tri1.points = _points;
				rects[i].tri2.points = _points;
				orects[i] = new Orect(rects[i], 0);
			}
		}

		public override void draw(SCENE scene) {
			turn(_points, points, mid, 800f * scene.progress + mousex, 1200f * scene.progress + mousey);

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
