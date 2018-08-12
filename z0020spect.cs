using System;
using System.Drawing;

namespace osusb1 {
partial class all {
	class Z0020spect : Z {

		const int NBARS = FFT.FREQS;
		const int SQSIZE = 8;
		const int MAXHEIGHT = 60;

		protected const int ET = 200;
		protected const int FT = ET + 700;

		vec3[] points;
		vec3[] _points;
		Pcube[] pcubes;
		Rect[] rects;
		Cube[] cubes;
		protected Orect[] orects;

		public Z0020spect(int start, int stop) {
			this.start = start;
			this.stop = stop;
			framedelta = 100;

			points = new vec3[8 * NBARS];
			_points = new vec3[points.Length];
			pcubes = new Pcube[NBARS];
			rects = new Rect[6 * NBARS];
			orects = new Orect[6 * NBARS];
			cubes = new Cube[NBARS];
			Color[] colors = new Color[] {
				Color.Cyan, Color.Lime, Color.Red, Color.White, Color.DeepPink, Color.Blue
			};
			for (int i = 0; i < NBARS; i++) {
				int bi = 8 * i;
				vec3 bp = v3(Zsc.mid);
				bp.z -= MAXHEIGHT / 4f;
				bp.x += SQSIZE * (i - NBARS / 2f);
				pcubes[i] = new Pcube(points, bi);
				pcubes[i].set(bp, SQSIZE, SQSIZE, MAXHEIGHT);
				cubes[i] = new Cube(colors, _points, bi);
			}
			int[] order = { Cube.D, Cube.R, Cube.U, Cube.L, Cube.B, Cube.F };
			for (int j = 0; j < NBARS; j++) {
				for (int i = 0; i < 6; i++) {
					int idx = j * 6 + i;
					rects[idx] = cubes[j].rects[order[i]];
					var or = new Orect(rects[idx], 0);
					or.addCommandOverride(new FadeCommand(stop - 300, stop, 1f, 0f));
					orects[reorder(idx)] = or;
				}
			}
		}

		protected virtual int reorder(int idx) {
			return idx;
		}

		public override void draw(SCENE scene) {
			ICommand.round_move_decimals.Push(5);
			for (int i = 0; i < NBARS; i++) {
				pcubes[i].setheight(MAXHEIGHT * smoothen(fft.smoothframe.values[i], scene));
			}
			for (int i = 0; i < points.Length; i++) {
				_points[i] = v3(points[i]);
			}
			Zsc.adjust(_points);
			int z = 0;
			foreach (Orect r in orects) {
				Cube cube = cubes[reorder(z++) / 6];
				if (shoulddrawcube(cube)) {
					r.update(scene);
				}
			}
			ICommand.round_move_decimals.Pop();
		}

		private float smoothen(float value, SCENE scene) {
			return value * clamp(inprogress(scene), 0f, 1f) * clamp(outprogress(scene), 0f, 1f);
		}

		protected virtual float inprogress(SCENE scene) {
			return progress(scene.starttime + ET, scene.starttime + FT, scene.time);
		}

		protected virtual float outprogress(SCENE scene) {
			return 1f - progress(scene.endtime - FT, scene.endtime - ET, scene.time);
		}

		protected virtual bool shoulddrawcube(Cube c) {
			return c.rects[Cube.L].shouldcull();
		}

		public override void fin(Writer w) {
			foreach (Orect r in orects) {
				r.fin(w);
			}
		}

	}
}
}
