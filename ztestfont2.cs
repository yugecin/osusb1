using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace osusb1 {
partial class all {
	class Ztestfont2 : Z {

		vec3 mid = v3(0f, -20f, 100f);

		vec3[] points;
		vec3[] _points;
		Rect[] rects;
		Orect[] orects;

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
			framedelta = 100;

			string text1 = "L|ne 1";
			string text2 = "l!ne tw@";
			string text = text1 + text2;

			int width = font.textWidth(text);
			int width1 = font.textWidth(text1);
			int width2 = font.textWidth(text2);
			int pointcount = font.calcPointCount(text);
			points = new vec3[pointcount * 8];
			_points = new vec3[points.Length];

			const int SIZE = 2;

			List<Rect> toprects = new List<Rect>();
			List<Rect> siderects = new List<Rect>();
			List<Rect> otherrects = new List<Rect>();
			Color[] cols = { Color.Cyan, Color.Lime, Color.Red, Color.Blue, Color.Yellow, Color.Orange };
			vec3 topleft = mid - v3(width1 / 2f * SIZE, 0f, -(font.charheight + 1) * SIZE);
			int pointidx = 0;
			for (int i = 0; i < text.Length; i++) {
				if (i == text1.Length) {
					topleft = mid - v3(width2 / 2f * SIZE, 0f, 1 * SIZE);
				}
				int c = text[i] - 32;
				vec3 pos = v3(topleft);
				for (int j = 0; j < font.charheight; j++) {
					int cw = font.charwidth[c];
					for (int k = 0; k < font.charwidth[c]; k++) {
						byte cd = font.chardata[c][j];
						if (((cd >> k) & 1) == 1) {
							vec3 basepoint = pos + v3(k * SIZE, 0f, 0f);
							new Pcube(points, pointidx).set(basepoint, SIZE, SIZE, SIZE);
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
			float x = 0;
			float y = mouse.y + udata[0];
			float z = mouse.x - scene.progress * 360f * 5f / 4f;

			if (scene.progress < 0.2f) {
				float rp = scene.progress / 0.2f;
				y = -75 * sin(rp * PI);
			} else if (scene.progress < 0.6f) {
				float rp = (scene.progress - 0.2f) / 0.4f;
				y = 75 * sin(rp * PI);
			} else if (scene.progress < 0.8f) {
				float rp = (scene.progress - 0.6f) / 0.2f;
				y = -75 * sin(rp * PI);
			} else {
				float rp = (scene.progress - 0.8f) / 0.2f;
				x = -75 * sin(rp * PI);
				y = -75 * sin(rp * PI);
				z = -75 * sin(rp * PI);
			}
			y += mouse.y;
			turn(_points, points, mid, quat(rad(x), rad(y), rad(z)));

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
