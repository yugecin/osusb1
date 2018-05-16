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
		Cube[] cubes;

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
			cubes = new Cube[NBARS];
			for (int i = 0; i < NBARS; i++) {
				int bi = 8 * i;
				vec3 bp = v3(0f + SQSIZE * (i - NBARS / 2), 50f, 70f);
				pcubes[i] = new Pcube(points, bi, bi + 1, bi + 2, bi + 3, bi + 4, bi + 5, bi + 6, bi + 7);
				pcubes[i].set(bp, SQSIZE, SQSIZE, MAXHEIGHT);
				cubes[i] = new Cube(
					Color.Cyan, Color.Lime, Color.Red, Color.White, Color.DeepPink, Color.Blue,
					_points,
					bi, bi + 1, bi + 2, bi + 3, bi + 4, bi + 5, bi + 6, bi + 7
				);
			}
		}

		public override void draw(SCENE scene) {
			screen.clear();
			for (int i = 0; i < NBARS; i++) {
				pcubes[i].setheight(MAXHEIGHT * fft.frame.values[i]);
			}
			turn(_points, points, v3(0f, 50f, 70f), scene.progress * 580f, 0f);
			foreach (Cube c in cubes) {
				c.draw(screen);
			}
			screen.draw(scene);
		}

		public override void fin(Writer w) {

		}

	}
}
}
