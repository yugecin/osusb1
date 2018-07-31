using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace osusb1 {
partial class all {
	class Orect {
		public const int SETTING_SHADED = 0x1;

		Rect rect;
		Otri[] tris;
		int settings;

		public Orect(Rect rect, int settings) {
			this.rect = rect;
			this.settings = settings;
			tris = new Otri[4];
			for (int i = 0; i < 4; i++) {
				tris[i] = new Otri();
			}
		}

		public void update(SCENE scene) {
			Tri[] _t = { rect.tri1, rect.tri2 };
			for (int i = 0; i < 2; i++) {
				Tri t = _t[i];
				Otri tri1 = tris[i * 2 + 0];
				Otri tri2 = tris[i * 2 + 1];

				if (rect.shouldcull()) { // somehow this doesn't work when using t
					tri1.update(scene.time, null, 0f, null, v2(0f));
					tri2.update(scene.time, null, 0f, null, v2(0f));
					continue;
				}

				vec4 shade = all.col(t.color);
				if ((settings & SETTING_SHADED) > 0) {
					shade *= .3f + .7f * (rect.surfacenorm().norm() ^ rect.rayvec().norm());
					shade.w = 1f;
				}

				float w = p.Project((t.points[t.a] + t.points[t.b] + t.points[t.c]) / 3f).w;

				vec2[] pts = {
					p.Project(t.points[t.a]).xy,
					p.Project(t.points[t.b]).xy,
					p.Project(t.points[t.c]).xy,
				};

				if (distance(pts[0], pts[1]) < distance(pts[0], pts[2])) {
					swap<vec2>(pts, 1, 2);
				}
				if (distance(pts[0], pts[1]) < distance(pts[1], pts[2])) {
					swap<vec2>(pts, 0, 2);
				}

				float rot = angle(pts[0], pts[1]);
				float dangle = rot - angle(pts[0], pts[2]);
				float x = cos(dangle) * distance(pts[0], pts[2]) / distance(pts[0], pts[1]);
				vec2 phantom = lerp(pts[0], pts[1], x);

				dotri(scene, tri1, pts, phantom, 0, w, shade);
				dotri(scene, tri2, pts, phantom, 1, w, shade);
			}
		}

		private void dotri(SCENE scene, Otri tri, vec2[] pts, vec2 phantom, int i, float w, vec4 col) {
			float rot = angle(phantom, pts[2]) + PI;
			vec2 pos = pts[2];
			vec2 size = v2(distance(phantom, pts[2]), distance(phantom, pts[i]));
			float d = angle(pts[i], phantom) - angle(pts[2], phantom);
			if (d > PI || (d < 0 && d > -PI)) {
				pos = pts[i];
				rot -= PI2;
				size = v2(size.y, size.x);
			}
			tri.update(scene.time, col, rot, v4(pos.x, pos.y, 1f, w), size);
			tri.draw(scene.g);
		}

		public void fin(Writer w) {
			foreach (Otri t in tris) {
				t.fin(w);
			}
		}
	}
}
}
