using System;
using System.Drawing;

namespace osusb1 {
partial class all {
	class Zstart : Z {

		vec3 mid = v3(0f, -30f, 100f);

		const int SIZE = 25;
		const float SPACING = 2.5f;
		const float ELEVATION = 12.5f;

		vec3[] points;
		Odot[] dots;

		public Zstart(int start, int stop) {
			this.start = start;
			this.stop = stop;
			framedelta = 100;

			points = new vec3[SIZE * SIZE];
			dots = new Odot[SIZE * SIZE];

			for (int i = 0; i < SIZE; i++) {
				for (int j = 0; j < SIZE; j++) {
					int idx = i * SIZE + j;
					int x = i - SIZE / 2;
					int y = j - SIZE / 2;
					vec3 point = mid + v3(x, y, 0f) * SPACING;
					points[idx] = point;
					int s = Sprite.INTERPOLATE_MOVE;
					dots[idx] = new Odot(Sprite.SPRITE_DOT_6_12, s);
				}
			}
		}

		public override void draw(SCENE scene) {
			for (int i = 0; i < points.Length; i++) {
				vec3 point = v3(points[i]);
				point.z += zval(scene.time, point) * ELEVATION;
				point = turn(point, mid, quatd(0f + mouse.x, 20f + mouse.y, 0f));
				vec4 q = project(point);
				dots[i].update(scene.time, v4(1f), q, 6f);
				dots[i].draw(scene.g);
			}
		}

		private float zval(int time, vec3 p) {
			int timedif = (int) (20f * (p - mid).length());
			FFT.FRAME sound = fft.SmootherValue(time - timedif);
			float m = 0f;
			for (int i = 0; i < FFT.FREQS; i++) {
				m = max(m, sound.values[i]);
			}
			return m;
		}

		public override void fin(Writer w) {
			foreach (Odot d in dots) {
				d.fin(w);
			}
		}

	}
}
}
