using System;
using System.Drawing;

namespace osusb1 {
partial class all {
	class Zheart : Z {

		vec3 mid = v3(0f, 50f, 90f);

		Rect[] rects;
		vec3[] points;
		vec3[] _points;
		Odot[] dots;
		Orect[] orects;

		vec3 basecolor = v3(255, 175, 244) / 255f;

		public Zheart(int start, int stop) {
			this.start = start;
			this.stop = stop;
			framedelta = 100;

			loadobj("obj1", out points, out rects);
			dots = new Odot[points.Length];
			orects = new Orect[rects.Length];
			_points = new vec3[points.Length];
			for (int i = 0; i < points.Length; i++) {
				points[i] = points[i] * 20f + mid;
			}
			for (int i = 0; i < points.Length; i++) {
				dots[i] = new Odot(Sprite.SPRITE_DOT_6_12, 0);
			}
			for (int i = 0; i < rects.Length; i++) {
				orects[i] = new Orect(rects[i], 0);
				rects[i].pts = _points;
				rects[i].tri1.points = _points;
				rects[i].tri2.points = _points;
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

			if (!rendering) {
				turn(_points, mid, quat(0f, 0f, rad(mouse.x)));
				turn(_points, mid, quat(0f, rad(-mouse.y), 0f));
			}

			foreach (Orect o in orects) {
				if (!o.rect.shouldcull()) {
					vec3 col = v3(basecolor);
					float v = (o.rect.surfacenorm().norm() ^ o.rect.rayvec().norm());
					v *= 1.4f;
					col *= .3f + .7f * v;
					if (col.x > 1f) col.x = 1f;
					if (col.y > 1f) col.y = 1f;
					if (col.z > 1f) col.z = 1f;
					o.rect.setColor(v4(col, 1f).col());
				}
				o.update(scene);
			}
		}

		public override void fin(Writer w) {
			foreach (Orect o in orects) {
				o.fin(w);
			}
		}

	}
}
}
