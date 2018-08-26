using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace osusb1 {
partial class all {
	class Zstartcube : Z {

		vec3 mid = v3(0f, -30f, 100f);

		vec3[] points;
		vec3[] _points;
		Cube cube;
		LINE[] lines;

		struct LINE {
			public Oline line;
			public Rect r1, r2;
			public vec3[] pts;
			public int a, b;
			public LINE(Rect r1, Rect r2, int a, int b) {
				pts = new vec3[2];
				line = new Oline(pts, 0, 1);
				this.r1 = r1;
				this.r2 = r2;
				this.a = a;
				this.b = b;
			}
		}

		const float SIZE = 30f;

		public Zstartcube(int start, int stop) {
			this.start = start;
			this.stop = stop;
			framedelta = 50;

			points = new vec3[8];
			_points = new vec3[8];
			new Pcube(points, 0).set(mid - v3(0f, 0f, SIZE / 2f), SIZE, SIZE, SIZE);
			Color col = Color.Black;
			Color[] cols = { col, col, col, col, col, col };
			Cube cube = new Cube(cols, _points, 0);
			Rect[] r = cube.rects;
			lines = new LINE[] {
				new LINE(r[Cube.F], r[Cube.U], 0, 1),
				new LINE(r[Cube.F], r[Cube.L], 0, 3),
				new LINE(r[Cube.F], r[Cube.R], 1, 2),
				new LINE(r[Cube.F], r[Cube.D], 2, 3),
				new LINE(r[Cube.L], r[Cube.U], 0, 5),
				new LINE(r[Cube.L], r[Cube.B], 5, 4),
				new LINE(r[Cube.L], r[Cube.D], 4, 3),
				new LINE(r[Cube.B], r[Cube.D], 4, 7),
				new LINE(r[Cube.R], r[Cube.U], 1, 6),
				new LINE(r[Cube.R], r[Cube.B], 6, 7),
				new LINE(r[Cube.R], r[Cube.D], 2, 7),
				new LINE(r[Cube.B], r[Cube.U], 5, 6),
			};
		}

		public override void draw(SCENE scene) {
			turn(_points, points, mid, 800f * scene.progress, 1200f * scene.progress);
			float linex = progressx(0, 2000, scene.reltime);
			bool cull = linex == 1f;
			foreach (LINE l in lines) {
				if (cull && l.r1.shouldcull() && l.r2.shouldcull()) {
					l.line.update(scene.time, null);
					continue;
				}
				l.pts[0] = _points[l.a];
				l.pts[1] = _points[l.b];
				if (linex != 1f) {
					l.pts[1] = lerp(l.pts[0], l.pts[1], linex);
				}
				l.line.update(scene.time, v4(1f));
				l.line.draw(scene.g);
			}
		}

		public override void fin(Writer w) {
			foreach (LINE l in lines) {
				l.line.fin(w);
			}
		}

	}
}
}
