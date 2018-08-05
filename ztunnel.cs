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
		const float ANGINC = TWOPI / CIRCLEAMOUNT;

		public Ztunnel(int start, int stop) {
			this.start = start;
			this.stop = stop;

			points = new vec3[CIRCLEAMOUNT * LENGTH];
			_points = new vec3[points.Length];
			dots = new Odot[points.Length];
			for (int i = 0; i < LENGTH; i++) {
				float ang = 0f;
				for (int j = 0; j < CIRCLEAMOUNT; j++) {
					int idx = i * CIRCLEAMOUNT + j;
					vec3 p = v3(Zrub.mid);
					p.x += cos(ang) * RAD;
					p.y += (i - LENGTH / 2) * SPACING;
					p.z += sin(ang) * RAD;
					points[idx] = p;
					int settings = Sprite.INTERPOLATE_MOVE;
					settings |= Sprite.EASE_FADE;
					settings |= Sprite.EASE_SCALE;
					dots[idx] = new Odot(Sprite.SPRITE_DOT_6_12, settings);
					ang += ANGINC;
				}
			}
		}

		public override void draw(SCENE scene) {
			vec3 movement = v3(0f, scene.progress * -600f, 0f);
			for (int i = 0; i < points.Length; i++) {
				_points[i] = points[i] + movement;
			}
			turn(_points, _points, Zrub.mid, 800f * scene.progress + mousex, 1200f * scene.progress + mousey);

			for (int i = 0; i < points.Length; i++) {
				vec4 q = p.Project(_points[i]);
				const float FOS = 250f;
				const float FOE = 650f;
				float mod = (1f - (clamp(q.w, FOS, FOE) - FOS) / (FOE - FOS));
				float size = 8f * mod;
				vec4 col = v4(1f);
				col.w = mod;
				dots[i].update(scene.time, col, q, size);
				dots[i].draw(scene.g);
			}
		}

		public override void fin(Writer w) {
			foreach (Odot o in dots) {
				o.fin(w);
			}
		}

	}
}
}
