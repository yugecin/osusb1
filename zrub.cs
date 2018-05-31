using System;
using System.Drawing;

namespace osusb1 {
partial class all {
	class Zrub : Z {

		static Color defcol = Color.Gray;

		Pixelscreen screen = new Pixelscreen(640, 480, 1);

		vec3[] points;
		vec3[] _points;
		Cube[] cubes;
		Rot[] rots;

		const int FM = 6;
		const int TMH = 7;
		const int TMV = 8;

		const float SIZE = 10f;
		const float SPACING = 10f;

		class Rot {
			public Cube[] cubes;
			public vec3 angles;
			public Rot(vec3 angles) {
				this.angles = angles;
				this.cubes = new Cube[9];
			}
		}

		vec3 mid;

		public Zrub(int start, int stop) {
			this.start = start;
			this.stop = stop;

			this.mid = v3(0f, 50f, 100f);
			this.points = new vec3[27 * 8];
			this._points = new vec3[27 * 8];
			this.cubes = new Cube[27];
			this.rots = new Rot[6 + 3];
			this.rots[Cube.L] = new Rot(v3(0f, +1f, 0f));
			this.rots[Cube.R] = new Rot(v3(0f, -1f, 0f));
			this.rots[Cube.F] = new Rot(v3(+1f, 0f, 0f));
			this.rots[Cube.B] = new Rot(v3(-1f, 0f, 0f));
			this.rots[Cube.D] = new Rot(v3(0f, 0f, +1f));
			this.rots[Cube.U] = new Rot(v3(0f, 0f, -1f));
			this.rots[FM] = new Rot(v3(0f, 0f, 0f));
			this.rots[TMH] = new Rot(v3(0f, 0f, 0f));
			this.rots[TMV] = new Rot(v3(0f, 0f, 0f));

			for (int a = 0; a < 3; a++) {
				for (int b = 0; b < 3; b++) {
					for (int c = 0; c < 3; c++) {
						gen(a, b, c);
					}
				}
			}
		}

		private void gen(int a, int b, int c) {
			Color[] cols = new Color[6];
			for (int i = 0; i < 6; i++) {
				cols[i] = Color.Gray;
			}
			cols[Cube.L] = new Color[] { Color.Red, defcol, defcol }[a];
			cols[Cube.R] = new Color[] { defcol, defcol, Color.Orange }[a];
			cols[Cube.F] = new Color[] { Color.White, defcol, defcol }[b];
			cols[Cube.B] = new Color[] { defcol, defcol, Color.Yellow }[b];
			cols[Cube.D] = new Color[] { Color.Blue, defcol, defcol }[c];
			cols[Cube.U] = new Color[] { defcol, defcol, Color.Green }[c];
			int idx = a * 9 + b * 3 + c;
			int pidx = idx * 8;
			this.cubes[idx] = new Cube(cols, this._points, pidx);
			vec3 basepoint = v3(a - 1, b - 1, c - 1) * SPACING + mid;
			new Pcube(this.points, pidx).set(basepoint, SIZE, SIZE, SIZE);

			this.rots[new int[] {Cube.L, TMV, Cube.R}[a]].cubes[b * 3 + c] = this.cubes[idx];
			this.rots[new int[] {Cube.F, TMH, Cube.B}[b]].cubes[a * 3 + c] = this.cubes[idx];
			this.rots[new int[] {Cube.D, FM, Cube.U}[c]].cubes[a * 3 + b] = this.cubes[idx];
		}

		public override void draw(SCENE scene) {
			screen.clear();
			turn(_points, points, this.mid, 0f, 0f);
			Rot rot = this.rots[Cube.D];
			foreach (Cube c in rot.cubes) {
				turn(c, this.mid, quat(rot.angles * scene.progress * 30f));
			}
			turn(_points, _points, this.mid, scene.progress * 200f + all.mousex, scene.progress * 900f + all.mousey);
			foreach (Cube c in this.cubes) {
				c.draw(screen);
			}
			//screen.draw(scene);
			foreach (Cube c in this.cubes) {
				foreach (Rect r in c.rects) {
					doside(scene, r);
				}
			}
		}

		void doside(SCENE scene, Rect r) {
			if (r.shouldcull()) {
				return;
			}

			if (scene.g == null) {
				return;
			}

			for (int x = 0; x < 5; x++) {
				for (int y = 0; y < 5; y++) {
					Color col = (x % 4 == 0 || y % 4 == 0) ? defcol : r.color;
					col = r.color;
					float dx = x * .8f / 4f + .1f;
					float dy = y * .8f / 4f + .1f;
					vec3 ab = lerp(r.pts[r.a], r.pts[r.b], dx);
					vec3 cd = lerp(r.pts[r.c], r.pts[r.d], dx);
					vec3 pt = lerp(ab, cd, dy);
					vec4 loc = p.Project(pt);
					int lx = (int) loc.x;
					int ly = (int) loc.y;
					if (lx < 0 || 640 <= lx || ly < 0 || 480 <= ly) {
						continue;
					}
					object o = screen.owner[lx, ly];
					if (!(o is Tri)) {
						continue;
					}
					if (((Tri) o).owner != r) {
						continue;
					}
					//scene.g.DrawRectangle(new Pen(col), lx - 1, ly - 1, 3, 3);
					scene.g.FillRectangle(new SolidBrush(col), lx - 1, ly - 1, 3, 3);
				}
			}
		}

		public override void fin(Writer w) {
		}

	}
}
}
