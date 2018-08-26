using System;
using System.Drawing;

namespace osusb1 {
partial class all {
	class Zheart : Z {

		Pixelscreen screen = new Pixelscreen(640 / 6, 480 / 6, 6);

		vec3 mid = v3(0f, 30f, 90f);

		const int FLYINTIME = 800;

		struct INDATA {
			public vec3[] pts;
			public vec3[] _pts;
			public Rect rect;
			public Orect orect;
			public vec3 rots;
			public int flyinstart;
		}

		vec3[] points;
		vec3[] _points;
		Odot[] dots;
		Orect[] orects;
		Oline[] olines;
		INDATA[] indata;
		int indatac;

		public static
		vec3 basecolor = v3(255, 175, 244) / 255f;

		const float BPM = 111f;
		const int BEATLEN = (int) (60000f / BPM);

		int transitionone;
		int transitiontwo;
		int transitiontri;

		int firstpulsetime;
		int turnstop;

		public Zheart(int start, int stop) {
			this.start = start;
			framedelta = BEATLEN / 4; // should be (540 / 4 =) 135
			this.start -= framedelta;
			this.stop = sync(stop);
			firstpulsetime = sync(72900);
			turnstop = sync(86500);
			transitionone = sync(77900);
			transitiontwo = sync(80000);
			transitiontri = sync(82100);

			Rect[] rects;
			loadobj("obj1", out points, out rects);
			dots = new Odot[points.Length];
			orects = new Orect[rects.Length];
			olines = new Oline[rects.Length * 3];
			indata = new INDATA[rects.Length];
			_points = new vec3[points.Length];
			for (int i = 0; i < points.Length; i++) {
				_points[i] = points[i] = points[i] * 20f + mid;
			}
			for (int i = 0; i < points.Length; i++) {
				dots[i] = new Odot(Sprite.SPRITE_DOT_6_12, 0);
			}
			Color bc = v4(basecolor, 1f).col();
			Random rand = new Random("zhearts".GetHashCode());
			for (int i = 0; i < rects.Length; i++) {
				Rect r = rects[i];
				r.setColor(bc);
				orects[i] = new Orect(r, Orect.SETTING_SHADED);
				r.pts = _points;
				r.tri1.points = _points;
				r.tri2.points = _points;
				olines[i * 3 + 0] = new Oline(r.pts, r.a, r.b);
				olines[i * 3 + 1] = new Oline(r.pts, r.a, r.d);
				olines[i * 3 + 2] = new Oline(r.pts, r.c, r.d);
				//if (!r.tri1.shouldcull() || !r.tri2.shouldcull()) {
					vec3[] inpts = {
						v3(points[r.a]),
						v3(points[r.b]),
						v3(points[r.c]),
						v3(points[r.d])
					};
					INDATA id;
					id.pts = inpts;
					id._pts = new vec3[4];
					id.rect = new Rect(null, bc, id._pts, 0, 1, 2, 3);
					id.orect = new Orect(id.rect, Orect.SETTING_SHADED | Orect.SETTING_NO_BCULL); 
					//var fc = new FadeCommand(start, start + 300, 0f, 1f);
					//id.orect.addCommandOverride(fc);
					float m = (project(r.mid()).xy - v2(50f)).length();
					id.flyinstart = start + (int) m;
					id.rots = v3(rand.Next(10), rand.Next(10), rand.Next(10));
					indata[indatac++] = id;
				//}
			}
		}

		public override void draw(SCENE scene) {
			ICommand.round_move_decimals.Push(5);
			turn(this._points, this.points, mid, scene.reltime / 5f + mouse.x, scene.reltime / 10f + mouse.y);

			copy(_points, points);
			for (int i = 0; i < points.Length; i++) {
				float expand = 0f;
				if (scene.time < 86000) {
					float time = scene.time - 89 - 150;
					time -= BEATLEN;
					float interval = BEATLEN * 2;
					expand = progressx(interval - framedelta * 2, interval, time % interval);
				}
				int pulsetime = sync(Zgreet.PULSETIME) - framedelta;
				if (scene.time > pulsetime) {
					float v = 1.8f;
					expand += v - progressx(pulsetime, pulsetime + framedelta * 2, scene.time) * v;
				}
				_points[i] = ((_points[i] - mid) * (1f + expand * .2f)) + mid;
			}

			float ambient = .3f;

			if (scene.time > firstpulsetime) {
				float v = progressx(firstpulsetime, firstpulsetime + 1500, scene.time);
				v = eq_in_sine(v);
				ambient += (1f - ambient) * (1f - v);
				int reltime = scene.time - firstpulsetime;
				float turnprogress = progressx(firstpulsetime, turnstop, scene.time);
				int fstart = sync(Zgreet.FADE_START) - framedelta * 2;
				if (scene.time > fstart) {
					reltime = scene.time - fstart;
					turnprogress = eq_in_quad(reltime / 5000f);
				}
				vec4 q;
				q = quat(0f, 0f, -eq_in_out_sine(turnprogress) * TWOPI * 3);
				turn(_points, mid, q);
				q = quat(0f, -eq_in_out_quad(turnprogress) * TWOPI * 2, 0f);
				turn(_points, mid, q);
				q = quat(eq_in_out_quad(turnprogress) * TWOPI, 0f, 0f);
				turn(_points, mid, q);

				if (!rendering) {
					turn(_points, mid, quat(0f, 0f, rad(mouse.x)));
					turn(_points, mid, quat(0f, rad(-mouse.y), 0f));
				}

				if (scene.time < transitionone || scene.time > transitiontri) {
					foreach (Orect o in orects) {
						o.update(scene, ambient, .9f - ambient, 1.4f);
					}
				} else if (scene.time > transitiontwo) {
					foreach (Oline o in olines) {
						o.update(scene.time, v4(1f));
						o.draw(scene.g);
					}
					foreach (Orect o in orects) {
						o.update(scene, -1f, -1f, -1f);
					}
				} else /*if (scene.time > transitionone) */{
					screen.clear();
					foreach (Orect o in orects) {
						Rect r = o.rect;
						if (!r.shouldcull()) {
							float rv = (r.surfacenorm().norm() ^ r.rayvec().norm());
							vec3 col = (v3(ambient) + (1f - ambient) * rv) * basecolor;
							r.setColor(col.col());
							r.draw(screen);
						}
						o.update(scene, -1f, -1f, -1f);
					}
					screen.draw(scene);
					foreach (Orect o in orects) {
						o.rect.setColor(basecolor.col());
					}
				}
			} else {
				float dt = progressx(start, firstpulsetime, scene.time);
				for (int i = 0; i < indatac; i++) {
					INDATA j = indata[i];
					copy(j._pts, j.pts);
					vec3 dir = (j.rect.mid() - mid) * 2f;
					dir *= 1f - eq_in_sine(dt);
					dir.z *= .4f;
					move(indata[i]._pts, dir);
					vec3 md = j.rect.mid();
					vec3 r = j.rots * (1f - dt);
					turn(j._pts, md, quat(0f, 0f, r.z));
					turn(j._pts, md, quat(0f, r.y, 0f));
					turn(j._pts, md, quat(r.x, 0f, 0f));
					indata[i].orect.update(scene, ambient, .9f - ambient, 1.4f);
				}
			}
end:
			ICommand.round_move_decimals.Pop();
		}

		public override void fin(Writer w) {
			ICommand.round_move_decimals.Push(5);
			for (int i = 0; i < indatac; i++) {
				indata[i].orect.fin(w);
			}
			foreach (Orect o in orects) {
				o.fin(w);
			}
			foreach (Oline o in olines) {
				o.fin(w);
			}
			screen.fin(w);
			ICommand.round_move_decimals.Pop();
		}

	}
}
}
