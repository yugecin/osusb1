using System;
using System.Drawing;

namespace osusb1 {
partial class all {
	class Zrub : Z {

		Pixelscreen screen = new Pixelscreen(640, 480, 1);

		vec3[] points;
		vec3[] _points;
		Cube[] cubes;

		vec3 mid;

		public Zrub(int start, int stop) {
			this.start = start;
			this.stop = stop;

			this.mid = v3(0f, 50f, 100f);
			this.points = new vec3[27 * 8];
			this._points = new vec3[27 * 8];
			this.cubes = new Cube[27];

			for (int a = 0; a < 3; a++) {
				for (int b = 0; b < 3; b++) {
					for (int c = 0; c < 3; c++) {
						gen(a, b, c);
					}
				}
			}
		}

		private void gen(int a, int b, int c) {
			Color defcol = Color.Gray;
			Color[] cols = new Color[6];
			for (int i = 0; i < 6; i++) {
				cols[i] = Color.Gray;
			}
			cols[Cube.L] = new Color[] { Color.Red, defcol, defcol }[a];
			cols[Cube.R] = new Color[] { defcol, defcol, Color.Orange }[a];
			cols[Cube.B] = new Color[] { defcol, defcol, Color.Yellow }[b];
			cols[Cube.F] = new Color[] { Color.White, defcol, defcol }[b];
			cols[Cube.U] = new Color[] { defcol, defcol, Color.Green }[c];
			cols[Cube.D] = new Color[] { Color.Blue, defcol, defcol }[c];
			int idx = a * 9 + b * 3 + c;
			int pidx = idx * 8;
			this.cubes[idx] = new Cube(cols, this._points, pidx);
			vec3 basepoint = v3(a - 1, b - 1, c - 1) * 12f + mid;
			new Pcube(this.points, pidx).set(basepoint, 10f, 10f, 10f);
		}

		public override void draw(SCENE scene) {
			screen.clear();
			turn(_points, points, this.mid, scene.progress * 200f, scene.progress * 900f);
			foreach (Cube c in this.cubes) {
				c.draw(screen);
			}
			screen.draw(scene);
		}

		public override void fin(Writer w) {
		}

	}
}
}
