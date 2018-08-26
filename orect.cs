using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace osusb1 {
partial class all {
	class Orect {
		public const int SETTING_SHADED = 0x1;
		public const int SETTING_NO_BCULL = 0x2;

		public readonly Rect rect;
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

		public void addCommandOverride(ICommand cmd) {
			for (int i = 0; i < tris.Length; i++) {
				tris[i].addCommandOverride(cmd);
			}
		}

		public void update(SCENE scene) {
			update(scene, .3f, .7f, 1f);
		}

		public void update(SCENE scene, float light_ambient, float light_diffuse, float light_mod) {
			Tri[] _t = { rect.tri1, rect.tri2 };
			for (int i = 0; i < 2; i++) {
				Tri t = _t[i];
				Otri tri1 = tris[i * 2 + 0];
				Otri tri2 = tris[i * 2 + 1];

				if (light_mod < 0) {
					goto cull;
				}

				if (t.shouldcull() && (settings & SETTING_NO_BCULL) == 0) {
					goto cull;
				}

				vec4 shade = all.col(t.color);
				if ((settings & SETTING_SHADED) > 0) {
					float rv = (rect.surfacenorm().norm() ^ rect.rayvec().norm());
					if (t.shouldcull()) {
						rv *= -1;
					}
					shade *= light_ambient + light_diffuse * rv;
					shade *= light_mod;
					if (shade.x > 1f) shade.x = 1f;
					if (shade.y > 1f) shade.y = 1f;
					if (shade.z > 1f) shade.z = 1f;
					shade.w = 1f;
				}

				float w = project((t.points[t.a] + t.points[t.b] + t.points[t.c]) / 3f).w;

				vec4[] pts4 = {
					project(t.points[t.a]),
					project(t.points[t.b]),
					project(t.points[t.c]),
				};

				if (pts4[0].z < 1f || pts4[1].z < 1f || pts4[2].z < 1f) {
					goto cull;
				}

				vec2[] pts = { pts4[0].xy, pts4[1].xy, pts4[2].xy };

				if (!isonscreen(pts)) {
					goto cull;
				}

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
				continue;
cull:
				tri1.update(scene.time, null, 0f, null, v2(0f));
				tri2.update(scene.time, null, 0f, null, v2(0f));
			}
		}

		private bool isonscreen(vec2[] pts) {
			if (isOnScreen(pts[0]) || isOnScreen(pts[1]) || isOnScreen(pts[2])) {
				return true;
			}
			return ios(pts[0], pts[1]) || ios(pts[1], pts[2]) || ios(pts[0], pts[2]);
		}

		private bool ios(vec2 a, vec2 b) {
			// don't look
			vec2[] pts = {a, b};
			if (b.x < a.x) {
				swap<vec2>(pts, 0, 1);
			}
			if ((a.x < LOWERBOUND && b.x > LOWERBOUND) ||
				(a.x < UPPERBOUND && b.x > UPPERBOUND))
			{
				float x = progress(a.x, b.x, LOWERBOUND);
				if (x < 0 || x > 1) {
					x = progress(a.x, b.x, UPPERBOUND);
				}
				float y = lerp(a.y, b.y, x);
				if (y > 0 && y < 480) {
					return true;
				}
			}
			if (b.y < a.y) {
				swap<vec2>(pts, 0, 1);
			}
			if ((a.y < 0 && b.y > 0) ||
				(a.y < 480 && b.y > 480))
			{
				float y = progress(a.y, b.y, 0f);
				if (y < 0 || y > 1) {
					y = progress(a.y, b.y, 480);
				}
				float x = lerp(a.x, b.x, y);
				if (x > LOWERBOUND && x < UPPERBOUND) {
					return true;
				}
			}
			return false;
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
