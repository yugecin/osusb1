using System;
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

		public Ztunnel(int start, int stop) {
			this.start = start;
			this.stop = stop;

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
					dots[idx].addCommandOverride(new ColorCommand(start + 10000, start + 20000, v3(1f), v3(1f, 0f, 0f)));
				}
			}
		}

		public override void draw(SCENE scene) {
			ICommand.round_scale_decimals.Push(1);
			vec3 movement = v3(0f, scene.progress * -600f, 0f);
			for (int i = 0; i < points.Length; i++) {
				_points[i] = points[i] + movement;
			}

			float _rot = scene.reltime / 50f;
			turn(_points, _points, Zrub.mid, quat(rad(_rot), 0f, 0f));
			turn(_points, _points, Zrub.mid, mousex, mousey);

			for (int i = 0; i < points.Length; i++) {
				vec4 q = p.Project(_points[i]);
				const float FOS = 250f;
				const float FOE = 650f;
				float mod = (1f - (clamp(q.w, FOS, FOE) - FOS) / (FOE - FOS));
				float size = 8f * mod;
				vec4 col = v4(.4f, .1f, .9f, 1f);
				col.w = mod;
				col.w *= clamp(scene.reltime, 0f, 400f) / 400f;
				dots[i].update(scene.time, col, q, size);
				dots[i].draw(scene.g);
			}
			ICommand.round_scale_decimals.Pop();
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
