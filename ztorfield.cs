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
		const float SPACING2 = SPACING / 2f;

		const int FIELDSIZE = 11;

		vec3[] points;
		vec3[] _points;
		Oline[] lines;

		const int T1 = 102900; // = start
		const int T2 = 103200;

		const int MOVETIME = 500;

		struct MOV {
			public int time;
			public M m;
		}

		struct M {
			public vec3 dir;
			public Eq[] eqs;
			public M[] t;
			public M(vec3 dir, Eq xeq, Eq yeq, Eq zeq) {
				this.dir = v3(dir);
				eqs = new Eq[] { xeq, yeq, zeq };
				t = new M[4];
			}
		}

		static M[] ms;
		MOV[] movs;

		public Ztorfield(int start, int stop) {
			framedelta = T2 - T1;
			this.start = sync(start);
			this.stop = stop;

			ms = new M[] {
				// cba to generate this rn
				new M(v3(1f, 1f, 0f), sin, cos, eq_linear),		
				new M(v3(1f, 1f, 0f), cos, sin, eq_linear),		
				new M(v3(1f, -1f, 0f), sin, cos, eq_linear),		
				new M(v3(1f, -1f, 0f), cos, sin, eq_linear),		
				new M(v3(1f, 0f, 1f), sin, eq_linear, cos),		
				new M(v3(1f, 0f, 1f), cos, eq_linear, sin),		
				new M(v3(1f, 0f, -1f), sin, eq_linear, cos),		
				new M(v3(1f, 0f, -1f), cos, eq_linear, sin),		
				new M(v3(-1f, 1f, 0f), sin, cos, eq_linear),		
				new M(v3(-1f, 1f, 0f), cos, sin, eq_linear),		
				new M(v3(-1f, -1f, 0f), sin, cos, eq_linear),		
				new M(v3(-1f, -1f, 0f), cos, sin, eq_linear),		
				new M(v3(-1f, 0f, 1f), sin, eq_linear, cos),		
				new M(v3(-1f, 0f, 1f), cos, eq_linear, sin),		
				new M(v3(-1f, 0f, -1f), sin, eq_linear, cos),		
				new M(v3(-1f, 0f, -1f), cos, eq_linear, sin),		
				new M(v3(0f, 1f, 1f), eq_linear, cos, sin),		
				new M(v3(0f, 1f, 1f), eq_linear, sin, cos),		
				new M(v3(0f, 1f, -1f), eq_linear, cos, sin),		
				new M(v3(0f, 1f, -1f), eq_linear, sin, cos),		
				new M(v3(0f, -1f, 1f), eq_linear, cos, sin),		
				new M(v3(0f, -1f, 1f), eq_linear, sin, cos),		
				new M(v3(0f, -1f, -1f), eq_linear, cos, sin),		
				new M(v3(0f, -1f, -1f), eq_linear, sin, cos),		
			};
			for (int i = 0; i < ms.Length; i++) {
				int a = 0;
				for (int j = 1; j < 3; j++) {
					if (ms[i].eqs[j] == cos) {
						a = j;
						break;
					}
				}
				int count = 0;
				for (int j = 0; j < ms.Length; j++) {
					if (i == j) {
						continue;
					}
					if (ms[j].dir[a] == ms[i].dir[a] &&
						ms[j].eqs[a] == sin)
					{
						ms[i].t[count++] = ms[j];
					}
				}
			}

			Random rand = new Random("ztorfield".GetHashCode());
			movs = new MOV[stop - start / MOVETIME];
			MOV __m;
			__m.time = T2;
			__m.m = ms[16];
			movs[0] = __m;
			for (int i = 1; i < movs.Length; i++) {
				MOV mov;
				mov.m = __m.m.t[rand.Next(4)];
				mov.time = T2 + i * MOVETIME;
				movs[i] = mov;
				__m = mov;
			}

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
			copy(_points, points);
			//turn(this._points, this.points, campos, scene.reltime / 5f + mouse.x, scene.reltime / 10f + mouse.y);

			vec3 dp = v3(0f);
			for (int i = 0; i < movs.Length; i++) {
				if (scene.time < movs[i].time) {
					break;
				}
				if (scene.time > movs[i].time + MOVETIME) {
					dp += movs[i].m.dir * SPACING2;
					continue;
				}
				float x = progress(movs[i].time, movs[i].time + MOVETIME, scene.time);
				for (int j = 0; j < 3; j++) {
					Eq eq = movs[i].m.eqs[j];
					float mod = eq(x * PI2);
					if (eq == cos) {
						mod = 1f - mod;
					}
					dp[j] += mod * movs[i].m.dir[j] * SPACING2;
				}
				break;
			}
			vec2 vd = viewdir(campos, campos + dp);
			vec4 lquatx = quat(0f, 0f, rad(scene.reltime / 5f));
			vec4 lquaty = quat(0f, rad(scene.reltime / 10f), 0f);
			move(_points, dp);
			//turn(_points, campos + dp, quat(0f, rad(mouse.y), rad(mouse.x)));
			turn(_points, campos, quat(0f, rad(mouse.y), rad(mouse.x)));
			//turn(_points, campos, lquatx);
			//turn(_points, campos, lquaty);

			int idx = 0;
			framedelta = T2 - T1;
			if (scene.time >= T2) {
				framedelta = 125;
			}
			foreach (Oline o in lines) {
				if (scene.time < T2) {
					int c = DIVV * DIVH * 2;
					if (idx++ % (c * 2) >= c) {
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
