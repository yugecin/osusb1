using System;
using System.Drawing;
using System.Text;

namespace osusb1 {
partial class all {
	class Zcheckerboard : Z {

		vec3[] points;
		vec3[] _points;
		Orect[] rects;

		public const float SPACING = 10;
		public const int SIZE = 11;

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
					Rect rect = new Rect(null, Color.White, _points, p, p + 1, p + 2, p + 3);
					rects[idx] = new Orect(rect, 0);
					idx++;
				}
			}
			move(points, Zlc.mid);
			move(points, v3(v2(SIZE / 2f) * -SPACING, 0f));
		}

		public override void draw(SCENE scene) {
			ICommand.round_move_decimals.Push(5);
			copy(_points, points);

			Zlc.adjust(_points);

			foreach (Orect r in rects) {
				r.update(scene);
			}
			ICommand.round_move_decimals.Pop();
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
