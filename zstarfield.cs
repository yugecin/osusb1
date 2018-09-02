using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace osusb1 {
partial class all {
	class Zstarfield : Z {

		vec3 mid = v3(0f, 0f, 100f);

		vec3[] points;
		vec3[] _points;

		Odot[] odots;

		const int AMOUNT = 500;
		const float SIZEXZ = 650f;
		const float SIZEY = 2500f;

		public Zstarfield(int start, int stop) {
			this.start = start;
			this.stop = stop;
			framedelta = 250;

			points = new vec3[AMOUNT];
			_points = new vec3[AMOUNT];
			odots = new Odot[AMOUNT];

			Random r = new Random("zstarfield".GetHashCode());
			vec3 offset = mid - v3(SIZEXZ / 2, 0f, SIZEXZ / 2);
			for (int i = 0; i < AMOUNT; i++) {
				points[i] = v3(
					(float) r.NextDouble() * SIZEXZ,
					(float) r.NextDouble() * SIZEY,
					(float) r.NextDouble() * SIZEXZ
				) + offset; 
				odots[i] = new Odot(Sprite.SPRITE_DOT_6_12, Sprite.EASE_ALL | Sprite.SESDSM);
			}
		}

		public override void draw(SCENE scene) {
			copymove(_points, points, v3(0f, -(SIZEY - 500f) * scene.progress, 0f));
			for (int i = 0; i < AMOUNT; i++) {
				float d = distance(campos, _points[i]);
				float size = (1f - progressx(200, 700, d)) * 8f;
				odots[i].update(scene.time, v4(1f), project(_points[i]), size);
				odots[i].draw(scene.g);
			}
		}

		public override void fin(Writer w) {
			foreach (Odot o in odots) {
				o.fin(w);
			}
		}

	}
}
}
