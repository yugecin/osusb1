using System;
using System.Drawing;

namespace osusb1 {
partial class all {
	class Z0020spect : Z {

		const int NBARS = FFT.FREQS;
		const int SQSIZE = 8;
		const int MAXHEIGHT = 60;

		vec3[] points;
		vec3[] _points;
		Pcube[] pcubes;
		Rect[] rects;
		Orect[] orects;

		public Z0020spect(int start, int stop) {
			this.start = start;
			this.stop = stop;
			framedelta = 100;

			points = new vec3[8 * NBARS];
			_points = new vec3[points.Length];
			pcubes = new Pcube[NBARS];
			rects = new Rect[6 * NBARS];
			orects = new Orect[6 * NBARS];
			Cube[] cubes = new Cube[NBARS];
			Color[] colors = new Color[] {
				Color.Cyan, Color.Lime, Color.Red, Color.White, Color.DeepPink, Color.Blue
			};
			for (int i = 0; i < NBARS; i++) {
				int bi = 8 * i;
				vec3 bp = v3(0f + SQSIZE * (i - NBARS / 2), 50f, 70f);
				pcubes[i] = new Pcube(points, bi);
				pcubes[i].set(bp, SQSIZE, SQSIZE, MAXHEIGHT);
				cubes[i] = new Cube(colors, _points, bi);
			}
			int[] order = { Cube.D, Cube.R, Cube.U, Cube.L, Cube.B, Cube.F };
			for (int i = 0; i < 6; i++) {
				for (int j = 0; j < NBARS; j++) {
					int idx = i * NBARS + j;
					rects[idx] = cubes[j].rects[order[i]];
					orects[idx] = new Orect(rects[idx], 0);
				}
			}
		}

		public override void draw(SCENE scene) {
			for (int i = 0; i < NBARS; i++) {
				pcubes[i].setheight(MAXHEIGHT * fft.smoothframe.values[i]);
			}
			turn(_points, points, v3(0f, 50f, 70f), -20f, 0f);
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
