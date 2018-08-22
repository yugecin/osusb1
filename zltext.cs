using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace osusb1 {
partial class all {
	class Zltext : Z {

		vec3[] points;
		vec3[] _points;
		MRECT[] rects;
		MRECT[] rects2;
		Orect[] orects;
		Orect[] orects2;

		struct MRECT {
			public Rect rect;
			public Tri left;
			public MRECT(Rect rect, Tri left) {
				this.rect = rect;
				this.left = left;
			}
		}

		public Zltext(int start, int stop, string text) {
			this.start = start;
			this.stop = stop;
			framedelta = 100;

			int width = font.textWidth(text);

			const int SIZE = 2;

			List<vec3> pointlist = new List<vec3>();
			List<MRECT> rectlist = new List<MRECT>();
			List<MRECT> rrectlist = new List<MRECT>();
			Color[] cols = { 
				v3(.9f).col(),
				v3(.6f + .3f).col(),
				v3(.6f + .3f).col(),
				v3(.3f + .6f).col(),
				v3(.3f + .6f).col(),
				v3(.9f).col(),
			};
			points = new vec3[8];
			vec3 topleft = Zlc.mid - v3(width / 2f * SIZE, 0f, 0f);
			int pointidx = 0;
			for (int i = 0; i < text.Length; i++) {
				int c = text[i] - 32;
				vec3 pos = v3(topleft);
				int cw = font.charwidth[c];
				for (int k = 0; k < font.charwidth[c]; k++) {
					pos.z = topleft.z - SIZE;
					List<MRECT> newrects = new List<MRECT>();
					bool[,] skip = new bool[6, font.charheight];
					for (int j = font.charheight - 1; j >= 0; j--) {
						byte cd = font.chardata[c][j];
						pos.z += SIZE;
						if (((cd >> k) & 1) == 0) {
							continue;
						}
						vec3 basepoint = pos + v3(k * SIZE, 0f, 0f);
						new Pcube(points, 0*pointidx).set(basepoint, SIZE, SIZE * 3, SIZE);
						for (int z = 0; z < 8; z++) {
							pointlist.Add(points[z]);
						}
						Cube cube = new Cube(cols, _points, pointidx);
						Tri tri = cube.rects[Cube.L].tri1;

						if (!skip[Cube.F, j]) {
							add(cube, Cube.F, tri, newrects, skip, j, c, k);
						}
						if (!skip[Cube.B, j]) {
							add(cube, Cube.B, tri, newrects, skip, j, c, k);
						}
						if (!skip[Cube.L, j] && (k == 0 || ((cd >> (k - 1)) & 1) != 1)) {
							add(cube, Cube.L, tri, newrects, skip, j, c, k);
						}
						if (!skip[Cube.R, j] && (k == cw - 1 || ((cd >> (k + 1)) & 1) != 1)) {
							add(cube, Cube.R, tri, newrects, skip, j, c, k);
						}
						if (j == 0 || ((font.chardata[c][j - 1] >> k) & 1) != 1) {
							newrects.Add(new MRECT(cube.rects[Cube.U], tri));
						}
						if (j == font.charheight - 1 || ((font.chardata[c][j + 1] >> k) & 1) != 1) {
							newrects.Add(new MRECT(cube.rects[Cube.D], tri));
						}
						pointidx += 8;
					}
					for (int z = 0; z < newrects.Count; z++) {
						rectlist.Add(newrects[z]);
						rrectlist.Add(newrects[newrects.Count - z - 1]);
					}
					newrects.Clear();
				}
				topleft.x += (cw + 1) * SIZE;
			}

			points = new vec3[pointlist.Count];
			_points = new vec3[points.Length];
			int pointcount = 0;
			foreach (vec3 p in pointlist) {
				points[pointcount++] = p;
			}
			rects = new MRECT[rectlist.Count];
			rects2 = new MRECT[rectlist.Count];
			orects = new Orect[rects.Length];
			orects2 = new Orect[rects.Length];
			int rectcount = 0;
			foreach (MRECT r in rectlist) {
				int i = rectcount++;
				rects[i] = r;
				r.left.points = _points;
				Rect rect = r.rect;
				rect.pts = _points;
				rect.tri1.points = _points;
				rect.tri2.points = _points;
				orects[i] = new Orect(rect, Orect.SETTING_SHADED);
			}
			rectcount = 0;
			foreach (MRECT r in rrectlist) {
				int i = rectcount++;
				rects2[i] = r;
				orects2[i] = new Orect(r.rect, Orect.SETTING_SHADED);
			}
		}

		private void add(Cube cube, int face, Tri tri, List<MRECT> newrects, bool[,] skip, int j, int c, int k) {
			Rect r = cube.rects[face];
			for (int jj = j - 1; jj >= 0; jj--) {
				if (((font.chardata[c][jj] >> k) & 1) == 0) {
					break;
				}
				r.a += 8;
				r.b += 8;
				r.tri1.a += 8;
				r.tri1.b += 8;
				r.tri2.b += 8;
				skip[face, jj] = true;
			}
			newrects.Add(new MRECT(r, tri));
		}

		public override void draw(SCENE scene) {
			ICommand.round_move_decimals.Push(5);
			copy(_points, points);
			Zlc.adjust(_points);
			for (int i = rects2.Length - 1; i >= 0; i--) {
				if (!rects2[i].left.shouldcull()) {
					orects2[i].update(scene);
				} else {
					orects2[i].update(scene, -1f, -1f, -1f);
				}
			}
			for (int i = 0; i < rects.Length; i++) {
				if (rects[i].left.shouldcull()) {
					orects[i].update(scene);
					orects[i].update(scene, -1f, -1f, -1f);
				}
			}
			ICommand.round_move_decimals.Pop();
		}

		public override void fin(Writer w) {
			ICommand.round_move_decimals.Push(5);
			for (int i = rects2.Length - 1; i >= 0; i--) {
				orects2[i].fin(w);
			}
			for (int i = 0; i < rects.Length; i++) {
				orects[i].fin(w);
			}
			ICommand.round_move_decimals.Pop();
		}

	}
}
}
