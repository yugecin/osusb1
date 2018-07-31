using System;
using System.Drawing;
using System.Text;

namespace osusb1 {
partial class all {
	class Ztestcube3 : Z {

		vec3 mid = v3(0f, 0f, 100f);

		vec3[] points;
		vec3[] _points;
		Cube cube;
		Otri[] tris;

		const int
			PA = 1,
			PB = 0,
			PC = 4,
			PD = 5,
			PE = 3,
			PF = 2,
			PG = 6,
			PH = 7;

		public Ztestcube3(int start, int stop) {
			this.start = start;
			this.stop = stop;

			points = new vec3[] {
				v3(-10f, -10f, 90f),
				v3(-10f, -10f, 110f),
				v3(-10f, 10f, 90f),
				v3(-10f, 10f, 110f),
				v3(10f, -10f, 90f),
				v3(10f, -10f, 110f),
				v3(10f, 10f, 90f),
				v3(10f, 10f, 110f),
			};

			_points = new vec3[points.Length];

			cube = new Cube(
				Color.Cyan,
				Color.Lime,
				Color.Red,
				Color.Blue,
				Color.Yellow,
				Color.Orange,
				_points,
				PA,
				PD,
				PC,
				PB,
				PF,
				PE,
				PH,
				PG
			);

			tris = new Otri[24];
			for (int i = 0; i < tris.Length; i++) {
				tris[i] = new Otri();
			}
		}

		public override void draw(SCENE scene) {
			turn(_points, points, mid, 800f * scene.progress, 1200f * scene.progress);

			for (int i = 0; i < 6; i++) {
				Rect r = cube.rects[i];
				Tri[] _t = { r.tri1, r.tri2 };
				for (int j = 0; j < 2; j++) {
					Tri t = _t[j];
					Otri tri1 = tris[i * 4 + j * 2 + 0];
					Otri tri2 = tris[i * 4 + j * 2 + 1];

					if (cube.rects[i].shouldcull()) { // somehow this doesn't work when using t
						tri1.update(scene.time, null, 0f, null, v2(0f));
						tri2.update(scene.time, null, 0f, null, v2(0f));
						continue;
					}

					vec4 shade = col(r.color);
					shade *= .3f + .7f * (r.surfacenorm().norm() ^ r.rayvec().norm());
					shade.w = 1f;

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

					if (scene.g != null) {
						PointF[] ptsf = new PointF[] { pts[0].pointf(), pts[1].pointf(), pts[2].pointf() };
						scene.g.DrawPolygon(new Pen(Color.Magenta), ptsf);
						scene.g.FillRectangle(new SolidBrush(Color.Magenta), phantom.x - 2f, phantom.y - 2f, 5f, 5f);
						scene.g.FillRectangle(new SolidBrush(Color.White), pts[0].x - 2f, pts[0].y - 2f, 5f, 5f);
					}
				}
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

		public override void fin(Writer w) {
			foreach (Otri t in tris) {
				t.fin(w);
			}
		}

	}
}
}
