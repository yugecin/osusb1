//#define SCREEN
using System;
using System.Drawing;

namespace osusb1 {
partial class all {
	class Ztor : Z {

#if SCREEN
		Pixelscreen screen = new Pixelscreen(640 / 6, 480 / 6, 6);
#else
		Pixelscreen screen = new Pixelscreen(640, 480, 1);
#endif

		vec3 mid = v3(0f, 50f, 90f);

		const int DIVH = 35;
		const int DIVV = 15;
		const int RH = 30;
		const int RV = 10;

		Rect[] rects;
		vec3[] points;
		vec3[] _points;
		Odot[] dots;

		vec3 basecolor = v3(.5f, .68f, .98f);

		public Ztor(int start, int stop) {
			this.start = start;
			this.stop = stop;
			framedelta = 100;

			this.rects = new Rect[DIVH * DIVV];
			this.points = new vec3[DIVH * DIVV];
			dots = new Odot[rects.Length];
			this._points = new vec3[DIVH * DIVV];
			const float INTH = 360f / DIVH;
			const float INTV = 360f / DIVV;
			for (int a = 0; a < DIVH; a++) {
				vec3 p1 = v3(mid);
				float anga = rad(INTH * a);
				for (int b = 0; b < DIVV; b++) {
					dots[a * DIVV + b] = new Odot(Sprite.SPRITE_DOT_6_12, 0);
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
			turn(this._points, this.points, mid, scene.reltime / 5f + mouse.x, scene.reltime / 10f + mouse.y);

			copy(_points, points);
			vec4 q;
			q = quat(0f, 0f, -scene.reltime / 1000f);
			turn(_points, mid, q);
			q = quat(0f, -scene.reltime / 1400f, 0f);
			turn(_points, mid, q);
			q = quat(scene.reltime / 2000f, 0f, 0f);
			turn(_points, mid, q);
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
#if SCREEN
					vec4 col = v4(basecolor, 1f);
					col *= .2f + .8f * (r.surfacenorm().norm() ^ r.rayvec().norm());
					r.setColor(col.col());
#endif
					r.draw(screen);
				}
			}
#if SCREEN
			screen.draw(scene);
#else
			for (int i = 0; i < rects.Length; i++) {
				Odot od = dots[i];
				Rect r = rects[i];
				if (r.shouldcull()) {
					goto cull;
				}
				vec4 a = p.Project(r.pts[r.a]);
				vec4 d = p.Project(r.pts[r.d]);
				vec3 loc = lerp(a.xyz, d.xyz, .5f);
				if (!isOnScreen(loc.xy)) {
					goto cull;
				}
				object o = screen.ownerAt(loc.xy);
				if (!(o is Tri)) {
					goto cull;
				}
				if (((Tri) o).owner != r) {
					goto cull;
				}
				vec4 b = p.Project(r.pts[r.b]);
				float dist = min(distance(a.xy, d.xy), distance(a.xy, b.xy));
				float size = dist / 3f;
				vec3 col = v3(basecolor);
				col *= .1f + .9f * (r.surfacenorm().norm() ^ r.rayvec().norm());
				od.update(scene.time, v4(col, 1f), v4(loc, 1f), size);
				od.draw(scene.g);
				continue;
cull:
				od.update(scene.time, null, null, 0f);
			}
#endif
		}

		public override void fin(Writer w) {
#if SCREEN
			screen.fin(w);
#else
			foreach (Odot o in dots) {
				o.fin(w);
			}
#endif
		}

	}
}
}
