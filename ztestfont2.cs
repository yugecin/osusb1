using System;
using System.Drawing;
using System.Text;

namespace osusb1 {
partial class all {
	class Ztestfont2 : Z {

		vec3 mid = v3(0f, 0f, 100f);

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

			string text1 = "L|ne 1";
			string text2 = "l!ne tw@";
			string text = text1 + text2;

			int width = font.textWidth(text);
			int width1 = font.textWidth(text1);
			int width2 = font.textWidth(text2);
			int fullsize = (width - (text.Length - 1)) * font.charheight;
			pointcount = 0;
			points = new vec3[fullsize * 8];
			rects = new Rect[fullsize * 6];
			int rectcount = 0;

			const int SIZE = 2;

			Color[] cols = { Color.Cyan, Color.Lime, Color.Red, Color.Blue, Color.Yellow, Color.Orange };
			vec3 topleft = mid - v3(width1 / 2f * SIZE, 0f, -(font.charheight + 1) * SIZE);
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
							new Pcube(points, pointcount).set(basepoint, SIZE, SIZE, SIZE);
							Cube cube = new Cube(cols, _points, pointcount);

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
							pointcount += 8;
						}
					}
					pos.z -= SIZE;
				}
				topleft.x += (font.charwidth[c] + 1) * SIZE;
			}

			_points = new vec3[pointcount];
			orects = new Orect[rectcount];
			for (int i = 0; i < rectcount; i++) {
				rects[i].pts = _points;
				rects[i].tri1.points = _points;
				rects[i].tri2.points = _points;
				orects[i] = new Orect(rects[i], 0);
			}
		}

		public override void draw(SCENE scene) {
			float x = 0;
			float y = mousey + udata[0];
			float z = mousex - scene.progress * 360f * 5f / 4f;

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
			turn(pointcount, _points, points, mid, quat(rad(x), rad(y), rad(z)));

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
