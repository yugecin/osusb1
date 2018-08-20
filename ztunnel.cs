using System;
using System.Collections.Generic;
using System.Text;

namespace osusb1 {
partial class all {
	class Ztunnel : Z {

		public static int FRAMEDELTA;

		vec3[] points;
		vec3[] _points;
		Odot[] dots;
		int[] pointfadetime;

		const int LENGTH = 80;
		const int SPACING = 10;
		const int CIRCLEAMOUNT = Zsc.TUNNEL_RAD / 4;
		const int SEGWIDTH = CIRCLEAMOUNT / 6;
		const int SEGLENGTH = 3;
		const int SEGSPACINGMOD = 7;
		const float ANGINC = PI / CIRCLEAMOUNT;

		const float FADESTART = 300f;
		const float FADEEND = FADESTART + 400f;

		const float LIGHTSTARTPOS = 50f;
		const float LIGHTSPEEDMOD = 2.2f; // higher = faster
		const int LIGHTFALLOFFTIME = 350;

		const int FADETIME = 300; // this should be sync()'d
		const int FADEWINDOW = 2000 - FADETIME;

		List<int> lighttimes = new List<int>();
		vec3 color = v3(.4f, .1f, .9f);

		public Ztunnel(int start, int stop) {
			this.start = start;
			this.stop = stop;
			framedelta = 300;
			FRAMEDELTA = framedelta;

			lighttimes.Add(21000);
			lighttimes.Add(29750);
			lighttimes.Add(38350);
			lighttimes.Add(47050);

			const int ssettings =
				Sprite.INTERPOLATE_MOVE | Sprite.EASE_FADE | Sprite.EASE_SCALE;

			points = new vec3[CIRCLEAMOUNT * LENGTH];
			_points = new vec3[points.Length];
			dots = new Odot[points.Length];
			pointfadetime = new int[points.Length];
			Random r = new Random("sunpy:3".GetHashCode());
			int[] fadeoffset = new int[CIRCLEAMOUNT];
			float y = -FADEEND;
			for (int i = 0; i < LENGTH; i++) {
				y += SPACING;
				if (i % SEGLENGTH == 0) {
					y += (SEGSPACINGMOD - 1) * SPACING;
					for (int j = 0; j < CIRCLEAMOUNT; j++) {
						fadeoffset[j] = -1;
					}
				}
				float ang = 0f;
				int k = 0;
				int segment = 0;
				for (int j = 0; j < CIRCLEAMOUNT; j++) {
					if (++k < SEGWIDTH) {
						ang += ANGINC;
					} else {
						k = 0;
						ang += ANGINC * (SEGWIDTH + 1);
						segment++;
						if (fadeoffset[segment] == -1) {
							int v = r.Next(FADEWINDOW - framedelta);
							fadeoffset[segment] = sync(v);
						}
					}
					int idx = i * CIRCLEAMOUNT + j;
					vec3 p = v3(Zsc.mid);
					p.x += cos(ang) * Zsc.TUNNEL_RAD;
					p.y += y;
					p.z += sin(ang) * Zsc.TUNNEL_RAD;
					points[idx] = p;
					dots[idx] = new Odot(Sprite.SPRITE_DOT_6_12, ssettings);
					int ft = stop - FADEWINDOW + fadeoffset[segment];
					pointfadetime[idx] = ft;
					FadeCommand fc;
					fc = new FadeCommand(ft, ft + FADETIME, 1f, 0f);
					dots[idx].addCommandOverride(fc);
					foreach (int lighttime in lighttimes) {
						int lightstart = lightStartTime(p, lighttime, lighttime);
						int lightend = lightstart + LIGHTFALLOFFTIME;
						ColorCommand cc;
						cc = new ColorCommand(lightstart, lightend, v3(1f), color);
						cc.easing = Equation.fromEquation(eq_in_quad).number;
						dots[idx].addCommandOverride(cc);
					}
				}
			}
			framedelta = 900;
		}

		const int B1S = 32500;
		const int B1E = 34500;

		public override void draw(SCENE scene) {
			ICommand.round_scale_decimals.Push(1);
			float flyInMod = (1f - clamp(scene.reltime, 0f, 900f) / 900f) * 1000f;

			for (int i = 0; i < points.Length; i++) {
				_points[i] = v3(points[i]);
				_points[i].y = pointYPosAt(points[i], scene.time);
				_points[i].y += flyInMod;
			}

			framedelta = 900;
			if (scene.reltime >= 900) {
				float _rot = (scene.reltime - 900f) / 50f;
				_rot -= clamp(progress(sync(B1S), sync(B1E), scene.time), 0f, 1f) * 60f;
				turn(_points, Zsc.mid, quat(rad(_rot), 0f, 0f));
				framedelta = 300; // this won't end good
			}

			for (int i = 0; i < points.Length; i++) {
				vec3 p = _points[i];
				vec3 mid = v3(Zsc.mid);
				mid.y = p.y;
				int ft = pointfadetime[i];
				float x = clamp(progressx(ft, ft + FADETIME, scene.time), 0f, 1f);
				_points[i] = p + (p - mid).norm() * 30f * x;
			}

			Zsc.adjust(_points);

			for (int i = 0; i < points.Length; i++) {
				vec4 q = p.Project(_points[i]);
				vec4 col = v4(color, 1f);
				if (!rendering) {
					// it's done using command overrides (see ctor),
					// so don't do this when exporting.
					// this is purely for the preview (why am I even doing this)
					foreach (int lighttime in lighttimes) {
						int lightstart = lightStartTime(points[i], scene.time, lighttime);
						int lightend = lightstart + LIGHTFALLOFFTIME;
						if (lightstart < scene.time && scene.time < lightend) {
							float x = progress(lightstart, lightend, scene.time);
							col = v4(lerp(v3(1f), col.xyz, x), 1f);
						}
					}
					int ft = pointfadetime[i];
					col.w *= 1f - clamp(progressx(ft, ft + FADETIME, scene.time), 0f, 1f);
				}
				float fadestart = FADESTART + Zsc.moveback;
				float fadeend = FADEEND + Zsc.moveback;
				float distCul = clamp(progress(fadestart, fadeend, q.w - flyInMod), 0f, 1f);
				float size = 8f * (1f - distCul);
				if (scene.reltime < 900 && _points[i].y < flyInMod) {
					size = 0f;
				}
				dots[i].update(scene.time, col, q, size);
				dots[i].draw(scene.g);
			}
			ICommand.round_scale_decimals.Pop();
		}

		private float pointYPosAt(vec3 point, int time) {
			float x = (float) (time - start) / (stop - start);
			float movement = x * -(600f + FADEEND);
			movement += clamp(progress(sync(B1S), sync(B1E), time), 0f, 1f) * 40f;
			return point.y + movement;
		}

		private int lightStartTime(vec3 point, int time, int lighttime) {
			float pp = pointYPosAt(point, lighttime);
			float dif = LIGHTSTARTPOS - pp;
			return lighttime + (int) (dif / LIGHTSPEEDMOD);
		}

		public override void fin(Writer w) {
			ICommand.round_scale_decimals.Push(1);
			foreach (Odot o in dots) {
				o.fin(w);
			}
			ICommand.round_scale_decimals.Pop();
		}

	}
}
}
