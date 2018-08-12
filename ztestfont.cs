using System;
using System.Drawing;
using System.Text;

namespace osusb1 {
partial class all {
	class Ztestfont : Z {

		vec3 mid = v3(0f, 100f, 100f);

		vec3[] points;
		vec3[] _points;
		Odot[] dots;

		const int
			PA = 1,
			PB = 0,
			PC = 4,
			PD = 5,
			PE = 3,
			PF = 2,
			PG = 6,
			PH = 7;

		public Ztestfont(int start, int stop) {
			this.start = start;
			this.stop = stop;
			framedelta = 100;

			string text = "abc defABC DEG < ! > 2 @ # &";

			int width = font.textWidth(text);
			points = new vec3[font.calcPointCount(text)];

			const int SPACING = 2;

			vec3 topleft = mid - v3(width / 2f * SPACING, 0f, -font.charheight / 2f * SPACING);
			int pointidx = 0;
			for (int i = 0; i < text.Length; i++) {
				int c = text[i] - 32;
				vec3 pos = v3(topleft);
				for (int j = 0; j < font.charheight; j++) {
					int cw = font.charwidth[c];
					for (int k = 0; k < font.charwidth[c]; k++) {
						if (((font.chardata[c][j] >> k) & 1) == 1) {
							points[pointidx++] = pos + v3(k * SPACING, 0f, 0f);
						}
					}
					pos.z -= SPACING;
				}
				topleft.x += (font.charwidth[c] + 1) * SPACING;
			}


			_points = new vec3[points.Length];
			dots = new Odot[_points.Length];
			for (int i = 0; i < points.Length; i++) {
				dots[i] = new Odot(Sprite.SPRITE_DOT_6_12, Sprite.INTERPOLATE_MOVE);
			}
		}

		public override void draw(SCENE scene) {
			turn(_points, points, mid, 800f * scene.progress + mouse.x, 1200f * scene.progress + mouse.y);

			for (int i = 0; i < points.Length; i++) {
				dots[i].update(scene.time, v4(1f), p.Project(_points[i]), 6f);
				dots[i].draw(scene.g);
			}
		}

		public override void fin(Writer w) {
			foreach (Odot d in dots) {
				d.fin(w);
			}
		}

	}
}
}
