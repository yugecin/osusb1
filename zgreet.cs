using System;
using System.Drawing;
using System.Text;

namespace osusb1 {
partial class all {
	class Zgreet : Z {

		vec3 mid = v3(0f, 100f, 100f);

		Sprite[] sprites;

		const int SPACING = 3;
		const int MOVETIME = 10000;

		public Zgreet(int start, int stop) {
			this.start = start;
			this.stop = stop;
			framedelta = 100;

			string[,] text = {
				{"Kewlers", "Emily | Sunpy"},
				{"mfx", "MrRheinerZufall"},
				{"byterapers", "Luken"},
				{"Razor 1911", "Wieku"},
				{"Logicoma", "truck"},
				{"Fairlight", "iq"},
				{"ASD", "mukkuru"},
				{"Elude", "nameless194"},
				{"CNCD", "yentis"},
				{"Loonies", "11t"},
			};

			string[] two = {
			};

			int pointcount = 0;
			for (int i = 0; i < text.GetLength(0); i++) {
				for (int j = 0; j < text.GetLength(1); j++) {
					pointcount += font.calcPointCount(text[i,j]);
				}
			}
			sprites = new Sprite[pointcount];

			const int xoffset = 300;

			int idx = 0;
			for (int z = 0; z < text.GetLength(0); z++) {
				for (int q = 0; q < text.GetLength(1); q++) {
					string t = text[z,q];
					int width = font.textWidth(t);
					int mod = 1 - 2 * q;
					int xoff = 0;
					for (int i = 0; i < t.Length; i++) {
						int c = t[i] - 32;
						int cw = font.charwidth[c];
						for (int j = 0; j < font.charheight; j++) {
							for (int k = 0; k < cw; k++) {
								if (((font.chardata[c][j] >> k) & 1) == 1) {
									int x = xoff + k;
									x *= SPACING;
									x -= width / 2;
									x -= xoffset * mod;
									mkpx(idx++, j, z, x, mod);
								}
							}
						}
						xoff += cw + 1;
					}
				}
			}
		}

		private void mkpx(int idx, int j, int z, int x, int mod) {
			x += 640 / 2;
			int y = 480 / 2;
			const int YDIFF = 200;
			int yoff = YDIFF * mod;

			if (mod == -1) {
				j = font.charheight - j - 2;
			}
			int timeoffset = j + z * (font.charheight + 6);
			timeoffset *= MOVETIME / YDIFF * SPACING / 2;
			timeoffset += start;
			int time = timeoffset;
			int end = timeoffset + MOVETIME;

			var s = new Sprite(Sprite.SPRITE_SQUARE_3_3, Sprite.NO_ADJUST_LAST);
			var m = new MoveCommand(time, end, v2(x, y + yoff), v2(x, y - yoff));
			s.starttime = time;
			s.endtime = end;
			s.addMove(m);
			sprites[idx] = s;
		}

		public override void draw(SCENE scene) {
			if (rendering) {
				return;
			}

			foreach (Sprite s in sprites) {
				var mc = s.movecmds.First.Value;
				if (mc.start > scene.time || mc.end < scene.time) {
					continue;
				}
				float x = progress(mc.start, mc.end, scene.time);
				x = Equation.fromNumber(mc.easing).calc(x);
				vec2 p = lerp(mc.from, mc.to, x);
				scene.g.FillRectangle(new SolidBrush(Color.White), p.x, p.y, 3, 3);
			}
		}

		public override void fin(Writer w) {
			foreach (Sprite s in sprites) {
				s.fin(w);
			}
		}

	}
}
}
