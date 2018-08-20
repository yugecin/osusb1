using System;
using System.Drawing;
using System.Text;

namespace osusb1 {
partial class all {
	class Zcheckerboard : Z {

		static vec3 mid = v3(campos);

		vec3[] points;
		vec3[] _points;
		Orect[] rects;

		const float SPACING = 10;
		const int SIZE = 11;

		public Zcheckerboard(int start, int stop) {
			this.start = start;
			this.stop = stop;
			framedelta = 100;

			rects = new Orect[SIZE * SIZE / 2 + 1];
			points = new vec3[rects.Length * 4];
			_points = new vec3[points.Length];
			int idx = 0;
			for (int i = 0; i < SIZE; i++) {
				for (int j = 0; j < SIZE; j++) {
					if ((i + j) % 2 == 1) {
						continue;
					}
					int p = idx * 4;
					points[p + 0] = v3((i + 0) * SPACING, (j + 0) * SPACING, 0f);
					points[p + 1] = v3((i + 0) * SPACING, (j + 1) * SPACING, 0f);
					points[p + 2] = v3((i + 1) * SPACING, (j + 0) * SPACING, 0f);
					points[p + 3] = v3((i + 1) * SPACING, (j + 1) * SPACING, 0f);
					Rect rect = new Rect(null, Color.Red, _points, p, p + 1, p + 2, p + 3);
					rects[idx] = new Orect(rect, 0);
					idx++;
				}
			}
			move(points, mid);
			move(points, v3(v2(SIZE / 2f) * -SPACING, 0f));
		}

		public static vec3 dp = v3(0f);
		public static vec4 lquatx = v4(0f);
		public static vec4 lquaty = v4(0f);

		public override void draw(SCENE scene) {
			ICommand.round_move_decimals.Push(5);
			copy(_points, points);

			const int DOWNTIME = 5000;
			dp = v3(0f);
			float rotprogress = clamp(scene.reltime - DOWNTIME, 0, stop);
			rotprogress /= 20000;
			dp.x = cos(rotprogress * TWOPI + PI2);
			dp.y = sin(rotprogress * TWOPI + PI2);
			float d = SPACING * SIZE / 2;
			float x = (1f - progressx(0, DOWNTIME, scene.reltime)) * PI2;
			dp.xy *= d * cos(x);
			dp.z -= sin(x) * d + 20f;

			vec2 vd = viewdir(campos, mid + dp);
			lquatx = quat(0f, 0f, vd.x);
			lquaty = quat(0f, vd.y, 0f);

			adjust(_points);

			foreach (Orect r in rects) {
				r.update(scene);
			}
			ICommand.round_move_decimals.Pop();
		}

		public static void adjust(vec3[] points) {
			turn(points, mid, quat(0f, rad(mouse.y), rad(mouse.x)));
			move(points, dp);
			turn(points, campos, lquatx);
			turn(points, campos, lquaty);
		}

		public override void fin(Writer w) {
			ICommand.round_move_decimals.Push(5);
			foreach (Orect r in rects) {
				r.fin(w);
			}
			ICommand.round_move_decimals.Pop();
		}

	}
}
}
