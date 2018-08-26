using System;
using System.Drawing;

namespace osusb1 {
partial class all {
	class Ztorfield : Z {

		const int DIVH = 6;
		const int DIVV = 4;
		const int RH = 30;
		const int RV = 5;
		const int SPACING = RH * 9;
		const float SPACING2 = SPACING / 2f;

		const int FIELDSIZE = 7;

		vec3[] points;
		vec3[] _points;
		Oline[] lines;

		const int T1 = 102900; // = start
		const int T2 = 103200;
		const int T3 = 103800;

		const int MOVETIME = 4200;

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
			__m.time = T3;
			__m.m = ms[23];
			movs[0] = __m;
			for (int i = 1; i < movs.Length; i++) {
				MOV mov;
				mov.m = __m.m.t[rand.Next(4)];
				mov.time = T3 + i * MOVETIME;
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

			points = new vec3[templatepoints.Length * FIELDSIZE * FIELDSIZE * FIELDSIZE * 3];
			_points = new vec3[points.Length];
			lines = new Oline[points.Length * 2];

			for (int i = 0; i < FIELDSIZE; i++) {
				for (int j = 0; j < FIELDSIZE; j++) {
					for (int k = 0; k < FIELDSIZE; k++) {
						vec3 off = v3(0f);
						vec4 rot = quat(0f, PI2, 0f);
						mk(templatepoints, templatelines, i, j, k, 0, off, rot);
						off = v3(-SPACING2, -SPACING2, 0f);
						rot = quat(PI2, 0f, 0f);
						mk(templatepoints, templatelines, i, j, k, 1, off, rot);
						off = v3(0f, -SPACING2, -SPACING2);
						rot = quat(0f, 0f, 0f);
						mk(templatepoints, templatelines, i, j, k, 2, off, rot);
					}
				}
			}
		}

		private void mk(vec3[] templatepoints, int[] templatelines, int i, int j, int k, int m, vec3 off, vec4 rot) {
			vec3[] pts = new vec3[templatepoints.Length];
			copy(pts, templatepoints);
			int idx = (i * FIELDSIZE * FIELDSIZE + j * FIELDSIZE + k) * 3 + m;
			idx *= pts.Length;
			turn(pts, campos, rot);
			vec3 offset = v3(i, j, k);
			const int F2 = FIELDSIZE / 2;
			offset -= F2;
			move(pts, offset * SPACING);
			move(pts, off);
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

		public override void draw(SCENE scene) {
			copy(_points, points);

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
			move(_points, dp);
			turn(_points, campos, quat(0f, rad(mouse.y), rad(mouse.x)));

			if (scene.time >= T3) {
				phantomframedelta = framedelta = 50;
				float reltime = scene.time - T3;
				reltime *= .5f;
				vec4 lquatx = quat(0f, 0f, reltime * PI2 / MOVETIME);
				vec4 lquaty = quat(0f, -reltime * PI / MOVETIME, 0f);
				float x = progress(0, 400, reltime / 30f);
				vec4 lquatz = quat(rad(sin(x) * TWOPI), 0f, 0f);
				turn(_points, campos, lquatx);
				turn(_points, campos, lquaty);
				turn(_points, campos, lquatz);
			} else if (scene.time >= T2) {
				phantomframedelta = framedelta = T3 - T2;
			} else {
				phantomframedelta = framedelta = T2 - T1;
			}
			int idx = 0;
			foreach (Oline o in lines) {
				if (scene.time < T2) {
					int c = DIVV * DIVH * 2;
					if (idx++ % (c * 2) >= c) {
						continue;
					}
				}
				float dist = ((o.pts[o.a] + o.pts[o.b]) / 2 - campos).length();
				float a = 1f - progressx(400, 450, dist);
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
