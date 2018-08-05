﻿using System;
using System.Collections.Generic;
using System.Text;

namespace osusb1 {
partial class all {
	class Ztunnel : Z {

		vec3[] points;
		vec3[] _points;
		Odot[] dots;

		const int LENGTH = 80;
		const int RAD = 132;
		const int SPACING = 10;
		const int CIRCLEAMOUNT = RAD / 4;
		const int SEGWIDTH = CIRCLEAMOUNT / 6;
		const int SEGLENGTH = 3;
		const int SEGSPACINGMOD = 5;
		const float ANGINC = PI / CIRCLEAMOUNT;

		const float FADESTART = 300f;
		const float FADEDIF = 400f;
		const float FADEEND = FADESTART + FADEDIF;

		const float LIGHTSTARTPOS = 50f;
		const float LIGHTSPEEDMOD = 2.2f; // higher = faster
		const int LIGHTFALLOFFTIME = 350;

		List<int> lighttimes = new List<int>();
		vec3 color = v3(.4f, .1f, .9f);

		public Ztunnel(int start, int stop) {
			this.start = start;
			this.stop = stop;

			lighttimes.Add(21000);
			lighttimes.Add(29750);
			lighttimes.Add(38350);
			lighttimes.Add(47050);

			const int ssettings =
				Sprite.INTERPOLATE_MOVE | Sprite.EASE_FADE | Sprite.EASE_SCALE;

			points = new vec3[CIRCLEAMOUNT * LENGTH];
			_points = new vec3[points.Length];
			dots = new Odot[points.Length];
			float y = 0;
			for (int i = 0; i < LENGTH; i++) {
				y += SPACING;
				if (i % SEGLENGTH == 0) {
					y += (SEGSPACINGMOD - 1) * SPACING;
				}
				float ang = 0f;
				int k = 0;
				for (int j = 0; j < CIRCLEAMOUNT; j++) {
					if (++k < SEGWIDTH) {
						ang += ANGINC;
					} else {
						k = 0;
						ang += ANGINC * (SEGWIDTH + 1);
					}
					int idx = i * CIRCLEAMOUNT + j;
					vec3 p = v3(Zrub.mid);
					p.y += 200f;
					p.x += cos(ang) * RAD;
					p.y += y;
					p.z += sin(ang) * RAD;
					points[idx] = p;
					dots[idx] = new Odot(Sprite.SPRITE_DOT_6_12, ssettings);
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
		}

		public override void draw(SCENE scene) {
			ICommand.round_scale_decimals.Push(1);
			for (int i = 0; i < points.Length; i++) {
				_points[i] = v3(points[i]);
				_points[i].y = pointYPosAt(points[i], scene.time);
			}

			float _rot = scene.reltime / 50f;
			turn(_points, _points, Zrub.mid, quat(rad(_rot), 0f, 0f));
			turn(_points, _points, Zrub.mid, mousex, mousey);

			for (int i = 0; i < points.Length; i++) {
				vec4 q = p.Project(_points[i]);
				float mod = (1f - (clamp(q.w, FADESTART, FADEEND) - FADESTART) / FADEDIF);
				float size = 8f * mod;
				vec3 color = this.color;
				if (!rendering) {
					// it's done using command overrides (see ctor),
					// so don't do this when exporting.
					// this is purely for the preview (why am I even doing this)
					foreach (int lighttime in lighttimes) {
						int lightstart = lightStartTime(points[i], scene.time, lighttime);
						int lightend = lightstart + LIGHTFALLOFFTIME;
						if (lightstart < scene.time && scene.time < lightend) {
							float x = progress(lightstart, lightend, scene.time);
							color = lerp(v3(1f), color, x);
						}
					}
				}
				vec4 col = v4(color, 1f);
				col.w = mod;
				col.w *= clamp(scene.reltime, 0f, 400f) / 400f;
				dots[i].update(scene.time, col, q, size);
				dots[i].draw(scene.g);
			}
			ICommand.round_scale_decimals.Pop();
		}

		private float pointYPosAt(vec3 point, int time) {
			float progress = (float) (time - start) / (stop - start);
			float movement = progress * -600f;
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
