using System;
using System.Drawing;

namespace osusb1 {
partial class all {
	class Ztor : Z {

		Pixelscreen screen = new Pixelscreen(640, 480, 1);

		vec3 mid = v3(0f, 50f, 90f);

		const int DIVH = 35;
		const int DIVV = 15;
		const int RH = 40;
		const int RV = 10;

		Rect[] rects;
		vec3[] points;
		vec3[] _points;

		public Ztor(int start, int stop) {
			this.start = start;
			this.stop = stop;

			this.rects = new Rect[DIVH * DIVV];
			this.points = new vec3[DIVH * DIVV];
			this._points = new vec3[DIVH * DIVV];
			const float INTH = 360f / DIVH;
			const float INTV = 360f / DIVV;
			for (int a = 0; a < DIVH; a++) {
				vec3 p1 = v3(mid);
				float anga = rad(INTH * a);
				for (int b = 0; b < DIVV; b++) {
					float angb = rad(INTV * b);
					float dist = RH - RV * cos(angb);
					vec3 p = mid + v3(dist * cos(anga), dist * sin(anga), RV * sin(angb));
					this.points[a * DIVV + b] = p;

					int a_ = (a + 1) % DIVH;
					int b_ = (b + 1) % DIVV;

					int _1 = a * DIVV + b;
					int _2 = a_ * DIVV + b;
					int _3 = a * DIVV + b_;
					int _4 = a_ * DIVV + b_;

					Rect r = new Rect(this, Color.Green, this._points, _1, _2, _3, _4);
					this.rects[a * DIVV + b] = r;
				}
			}
		}

		public override void draw(SCENE scene) {
			turn(this._points, this.points, mid, scene.reltime / 5f + mousex, scene.reltime / 10f + mousey);
			/*
			if (scene.g != null) {
				foreach (vec3 v in this._points) {
					vec4 r = p.Project(v);
					scene.g.FillRectangle(new SolidBrush(Color.Green), r.x - 1, r.y - 1, 2, 2);
				}
			}
			*/
			screen.clear();
			foreach (Rect r in rects) {
				if (!r.shouldcull()) {
					//vec3 col = v3(.5f, .68f, .98f);
					//col *= .1f + .9f * (r.surfacenorm().norm() ^ r.rayvec().norm());
					//r.color = r.tri1.color = r.tri2.color = col.col();
					r.draw(screen);
				}
			}
			//screen.draw(scene);
			if (scene.g != null) foreach (Rect r in rects) {
				if (!r.shouldcull()) {
					vec4 loc = p.Project(lerp(r.pts[r.a], r.pts[r.d], .5f));
					int lx = (int) loc.x;
					int ly = (int) loc.y;
					if (lx < 0 || 640 <= lx || ly < 0 || 480 <= ly) {
						continue;
					}
					object o = screen.owner[lx, ly];
					if (!(o is Tri)) {
						continue;
					}
					if (((Tri) o).owner != r) {
						continue;
					}
					vec3 col = v3(.5f, .68f, .98f);
					col *= .1f + .9f * (r.surfacenorm().norm() ^ r.rayvec().norm());
					scene.g.FillRectangle(new SolidBrush(col.col()), lx - 1, ly - 1, 3, 3);
				}
			}
		}

		public override void fin(Writer w) {
		}

	}
}
}
