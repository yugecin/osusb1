//#define SCREEN
using System;
using System.Drawing;

namespace osusb1 {
partial class all {
	class Ztorfield : Z {

		const int DIVH = 6;
		const int DIVV = 3;
		const int RH = 30;
		const int RV = 5;
		const int SPACING = RH * 6;

		const int FIELDSIZE = 11;

		vec3[] points;
		vec3[] _points;
		Oline[] lines;

		const int T1 = 102900; // = start
		const int T2 = 103200;

		public Ztorfield(int start, int stop) {
			framedelta = T2 - T1;
			this.start = sync(start);
			this.stop = stop;

			vec3[] templatepoints = new vec3[DIVH * DIVV];
			int[] templatelines = new int[templatepoints.Length * 4];
			const float INTH = 360f / DIVH;
			const float INTV = 360f / DIVV;
			for (int a = 0; a < DIVH; a++) {
				vec3 p1 = v3(campos);
				float anga = rad(INTH * a);
				for (int b = 0; b < DIVV; b++) {
					int idx = a * DIVV + b;
					float angb = rad(INTV * b);
					float dist = RH - RV * cos(angb);
					vec3 p = campos + v3(dist * cos(anga), dist * sin(anga), RV * sin(angb));
					templatepoints[idx] = p;

					int a_ = (a + 1) % DIVH;
					int b_ = (b + 1) % DIVV;

					int _1 = a * DIVV + b;
					int _2 = a_ * DIVV + b;
					int _3 = a * DIVV + b_;
					int _4 = a_ * DIVV + b_;
					
					templatelines[idx * 4 + 0] = _1;
					templatelines[idx * 4 + 1] = _2;
					templatelines[idx * 4 + 2] = _3;
					templatelines[idx * 4 + 3] = _4;
				}
			}

			points = new vec3[templatepoints.Length * FIELDSIZE * FIELDSIZE * FIELDSIZE];
			_points = new vec3[points.Length];
			lines = new Oline[points.Length * 2];

			const int F2 = FIELDSIZE / 2;
			for (int i = 0; i < FIELDSIZE; i++) {
				for (int j = 0; j < FIELDSIZE; j++) {
					for (int k = 0; k < FIELDSIZE; k++) {
						vec3[] pts = new vec3[templatepoints.Length];
						copy(pts, templatepoints);
						int idx = (i * FIELDSIZE * FIELDSIZE + j * FIELDSIZE + k);
						if (idx % 2 == 0) {
							turn(pts, campos, quat(0f, PI2, 0f));
						}
						idx *= pts.Length;
						vec3 offset = v3(i, j, k);
						offset -= F2;
						if (i == F2 && j == F2 && k == F2) {
							offset.x = 10000f;
						}
						move(pts, offset * SPACING);
						copy(points, idx, pts, 0, pts.Length);
						for (int z = 0; z < pts.Length; z++) {
							int _1 = templatelines[z * 4 + 0] + idx;
							int _2 = templatelines[z * 4 + 1] + idx;
							int _3 = templatelines[z * 4 + 2] + idx;
							int _4 = templatelines[z * 4 + 3] + idx;
							lines[(idx + z) * 2 + 0] = new Oline(_points, _1, _3);
							lines[(idx + z) * 2 + 1] = new Oline(_points, _4, _3);
						}
					}
				}
			}
		}

		public override void draw(SCENE scene) {
			//turn(this._points, this.points, campos, scene.reltime / 5f + mouse.x, scene.reltime / 10f + mouse.y);
			copy(_points, points);

			int idx = 0;
			framedelta = T2 - T1;
			if (scene.time >= T2) {
				framedelta = 125;
			}
			foreach (Oline o in lines) {
				if (scene.time < T2) {
					int c = DIVV * DIVH * 2;
					if (idx++ % (c * 2) < c) {
						continue;
					}
				}
				float dist = ((o.pts[o.a] + o.pts[o.b]) / 2 - campos).length();
				float a = 1f - progressx(300, 500, dist);
				o.update(scene.time, v4(v3(1f), a));
				o.draw(scene.g);
			}
		}

		public override void fin(Writer w) {
			foreach (Oline o in lines) {
				o.fin(w);
			}
		}

	}
}
}
