using System;
using System.Drawing;
using System.Text;

namespace osusb1 {
partial class all {
	class Zgreet : Z {

		vec3 mid = v3(0f, 100f, 100f);

		Sprite[] sprites;

		const int SPACING = 2;
		const int LINESPACING = 5;
		const int MOVETIME = 10000;

		public
		const int PULSETIME = 95100;

		const int SHOW_START = 86500;
		const int SHOW_END = PULSETIME;
		const int SHOW_TIME = 700;
		int show_delay;

		public
		const int FADE_START = 99600;
		const int FADE_END = 101700;
		const int FADE_TIME = 1000;
		const int FADE_TIMES = 5;

		Random rand = new Random("zgreet".GetHashCode());

		public Zgreet(int start, int stop) {
			this.start = start;
			this.stop = stop;
			framedelta = 100;

			string[,] text = {
				{"Kewlers", "Emily | Sunpy"},
				{"mfx", "MrRheinerZufall"},
				{"byterapers", "Luken"},
				{"Razor 1911", "Wieku"},
				{"Logicoma", "Truck"},
				{"Fairlight", "iq"},
				{"ASD", "Mukkuru"},
				{"Elude", "nameless194"},
				{"CNCD", "Yentis"},
				{"Loonies", "11t"},
				{"Titan", "PLACEHOLDER"},
			};

			int tl0 = text.GetLength(0);
			show_delay = (SHOW_END - SHOW_START - SHOW_TIME) / tl0;

			int pointcount = 0;
			for (int i = 0; i < text.GetLength(0); i++) {
				for (int j = 0; j < text.GetLength(1); j++) {
					pointcount += font.calcPointCount(text[i,j]);
				}
			}
			sprites = new Sprite[pointcount];

			Random rand = new Random();
			int idx = 0;
			int z2 = tl0 / 2;
			for (int z = 0; z < tl0; z++) {
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
									x -= width / 2;
									mkpx(idx++, x, j, -z2 + z, z, mod);
								}
							}
						}
						xoff += cw + 1;
					}
				}
			}
		}

		private void mkpx(int idx, int x, int j, int z, int z0, int mod) {
			const int xoffset = 250;
			const int YDIFF = 50;

			vec2 mid = v2(640 / 2, 480 / 2);

			vec2 p = v2(x, j);
			p.y += z * (font.charheight + LINESPACING) * mod;

			p *= SPACING;

			p.x -= xoffset * mod;
			p += mid;

			vec2 fromp = v2(p);
			fromp.y += YDIFF * mod;

			int starttime = SHOW_START + show_delay * z0;
			int endtime = starttime + SHOW_TIME;

			int pulsestart = PULSETIME;
			pulsestart += (int) ((p - mid).length() * 1.1f);
			pulsestart -= (int) (v2(xoffset / 2f).length());

			var s = new Sprite(Sprite.SPRITE_SQUARE_2_2, Sprite.NO_ADJUST_LAST);
			var m = new MoveCommand(starttime, endtime, fromp, p);
			//var f = new FadeCommand(starttime, endtime, 0f, 1f);
			var f = new ColorCommand(starttime, endtime, v3(0f), v3(1f));
			var c = new ColorCommand(pulsestart, pulsestart + 800, Zheart.basecolor, v3(1f));
			m.easing = Equation.fromEquation(eq_out_cubic).number;
			//f.easing = Equation.fromEquation(eq_out_expo).number;
			f.easing = Equation.fromEquation(eq_out_expo).number;
			c.easing = Equation.fromEquation(eq_in_quad).number;
			s.addMove(m);
			//s.addFade(f);
			s.addColor(f);
			//s.addColor(new ColorCommand(starttime, starttime, v3(1f), v3(1f))); // because yeah
			s.addColor(c);
			s.starttime = starttime;
			s.endtime = stop;
			int fadestart = rand.Next(sync(FADE_START), FADE_END - FADE_TIME);
			int fadetime = FADE_TIME / FADE_TIMES / 2;
			s.addRaw(string.Format("_L,{0},{1}", fadestart, FADE_TIMES));
			//s.addRaw("_" + new FadeCommand(0, 0, 0f, 0f).ToString());
			//s.addRaw("_" + new FadeCommand(fadetime, fadetime * 2, 1f, 1f).ToString());
			s.addRaw("_" + new FadeCommand(0, fadetime, 1f, 1f).ToString());
			s.addRaw("_" + new FadeCommand(fadetime, fadetime * 2, 0f, 0f).ToString());
			sprites[idx] = s;
		}

		public override void draw(SCENE scene) {
			if (rendering) {
				return;
			}

			foreach (Sprite s in sprites) {
				var mc = s.movecmds.First.Value;
				if (mc.start > scene.time) {
					continue;
				}
				float x = progressx(mc.start, mc.end, scene.time);
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
