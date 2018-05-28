using System;
using System.Drawing;

namespace osusb1 {
partial class all {
	class Z0010spect : Z {

		const int NBARS = 8;
		const int SQSIZE = 10;
		const int MAXHEIGHT = 30;

		vec3[] points;
		vec3[] _points;
		Pcube[] pcubes;
		Rect[] rects;

		//Pixelscreen screen = new Pixelscreen(500, 400, 1);
		//Pixelscreen screen = new Pixelscreen(250, 175, 2);
		//Pixelscreen screen = new Pixelscreen(125, 100, 4);
		Pixelscreen screen = new Pixelscreen(100, 75, 6);

		public Z0010spect(int start, int stop) {
			this.start = start;
			this.stop = stop;
			points = new vec3[8 * NBARS];
			_points = new vec3[points.Length];
			pcubes = new Pcube[NBARS];
			rects = new Rect[6 * NBARS];
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
			int[] order = { Cube.F, Cube.B, Cube.R, Cube.L, Cube.U, Cube.D };
			for (int i = 0; i < 6; i++) {
				for (int j = 0; j < NBARS; j++) {
					rects[i * NBARS + j] = cubes[j].rects[order[i]];
				}
			}
		}

		public override void draw(SCENE scene) {
			screen.clear();
			for (int i = 0; i < NBARS; i++) {
				pcubes[i].setheight(MAXHEIGHT * fft.frame.values[i]);
			}
			turn(_points, points, v3(0f, 50f, 70f), scene.progress * 580f, 0f);
			foreach (Rect r in rects) {
				if (!r.shouldcull()) {
					r.draw(screen);
				}
			}
			screen.draw(scene);
		}

		public override void fin(Writer w) {
			screen.fin(w);
		}

	}
}
}
