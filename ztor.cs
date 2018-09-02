//#define SCREEN
using System;
using System.Drawing;

namespace osusb1 {
partial class all {
	class Ztor : Z {

#if SCREEN
		Pixelscreen screen = new Pixelscreen(640 / 6, 480 / 6, 6);
#else
		Pixelscreen screen1 = new Pixelscreen(640, 480, 1);
		Pixelscreen screen2 = new Pixelscreen(640, 480, 1);
#endif

		vec3 mid = v3(0f, 50f, 90f);

		const int DIVH = 30;
		const int DIVV = 12;
		const int RH = 30;
		const int RV = 10;

		Rect[] rects;
		vec3[] points;
		vec3[] _points;
		Odot[] dots;
		Rect textplane;
		vec2[] txtpoints;
		Odot[] txtdots;

		vec3 basecolor = v3(.5f, .68f, .98f);

		public Ztor(int start, int stop) {
			this.start = start;
			this.stop = stop;
			framedelta = 125;
			phantomframedelta = 25;

			string text = "Lacking creativity to make a better storyboard"
				+ "... hope you enjoyed this!";
			txtpoints = new vec2[font.calcPointCount(text)];
			txtdots = new Odot[txtpoints.Length];
			const float FONTSPACING = 2f;
			int pointidx = 0;
			vec2 fonttopleft = v2(UPPERBOUND, 260 - font.charheight / 2f * FONTSPACING);
			for (int i = 0; i < text.Length; i++) {
				int c = text[i] - 32;
				vec2 pos = v2(fonttopleft);
				for (int j = 0; j < font.charheight; j++) {
					int cw = font.charwidth[c];
					for (int k = 0; k < font.charwidth[c]; k++) {
						if (((font.chardata[c][j] >> k) & 1) == 1) {
							int idx = pointidx++;
							txtpoints[idx] = pos + v2(k * FONTSPACING, 0f);
							txtdots[idx] = new Odot(
								Sprite.SPRITE_SQUARE_2_2,
								Sprite.INTERPOLATE_MOVE | Sprite.COMPRESS_MOVE
							);
						}
					}
					pos.y += FONTSPACING;
				}
				fonttopleft.x += (font.charwidth[c] + 1) * FONTSPACING;
			}

			this.rects = new Rect[DIVH * DIVV];
			this.points = new vec3[DIVH * DIVV];
			dots = new Odot[rects.Length];
			this._points = new vec3[DIVH * DIVV];
			const float INTH = 360f / DIVH;
			const float INTV = 360f / DIVV;
			for (int a = 0; a < DIVH; a++) {
				vec3 p1 = v3(mid);
				float anga = rad(INTH * a);
				for (int b = 0; b < DIVV; b++) {
					var d = new Odot(Sprite.SPRITE_DOT_6_12, Sprite.EASE_ALL | Sprite.SESDSM);
					var c = new FadeCommand(stop - 500, stop, 1f, 0f);
					d.addCommandOverride(c);
					dots[a * DIVV + b] = d;
					float angb = rad(INTV * b);
					float dist = RH - RV * cos(angb);
					vec3 p = mid + v3(dist * cos(anga), dist * sin(anga), RV * sin(angb));
					this.points[a * DIVV + b] = p;

					int a_ = (a + 1) % DIVH;
					int b_ = (b + 1) % DIVV;

					int _1 = a * DIVV + b;
					int _2 = a_ * DIVV + b;
					int _3 = a * DIVV + b_;
					int _4 = a_ * DIVV + b_;

					Rect r = new Rect(this, Color.Green, this._points, _1, _2, _3, _4);
					this.rects[a * DIVV + b] = r;
				}
			}

			vec3[] rpts = {
				// I have no clue really
				mid + v3(-100, -70, 80),
				mid + v3(100, -70, 80),
				mid + v3(-100, -70, -80),
				mid + v3(100, -70, -80),
			};
			textplane = new Rect(null, Color.Red, rpts, 0, 1, 2, 3);
		}

		public override void draw(SCENE scene) {
			vec3[] opt = textplane.pts;
			textplane.setpts(copy(opt));

			copy(_points, points);
			vec4 q;
			q = quat(0f, 0f, -scene.reltime / 1000f);
			turn(_points, mid, q);
			q = quat(0f, -scene.reltime / 1400f, 0f);
			turn(_points, mid, q);
			q = quat(scene.reltime / 2000f, 0f, 0f);
			turn(_points, mid, q);
			/*
			if (scene.g != null) {
				foreach (vec3 v in this._points) {
					vec4 r = p.Project(v);
					scene.g.FillRectangle(new SolidBrush(Color.Green), r.x - 1, r.y - 1, 2, 2);
				}
			}
			*/
			if (!isPhantomFrame) {
				screen1.clear();
			}
			screen2.clear();
			foreach (Rect r in rects) {
				if (!r.shouldcull()) {
#if SCREEN
					vec4 col = v4(basecolor, 1f);
					col *= .2f + .8f * (r.surfacenorm().norm() ^ r.rayvec().norm());
					r.setColor(col.col());
#endif
					if (!isPhantomFrame) {
						r.draw(screen1);
					}
					r.draw(screen2);
				}
			}
			textplane.draw(screen2);
			if (!isPhantomFrame) {
#if SCREEN
				screen.draw(scene1);
#else
				for (int i = 0; i < rects.Length; i++) {
					Odot od = dots[i];
					Rect r = rects[i];
					if (r.shouldcull()) {
						goto cull;
					}
					vec4 a = project(r.pts[r.a]);
					vec4 d = project(r.pts[r.d]);
					vec3 loc = lerp(a.xyz, d.xyz, .5f);
					if (!isOnScreen(loc.xy)) {
						goto cull;
					}
					object o = screen1.ownerAt(loc.xy);
					if (!(o is Tri)) {
						goto cull;
					}
					if (((Tri) o).owner != r) {
						goto cull;
					}
					vec4 b = project(r.pts[r.b]);
					float dist = min(distance(a.xy, d.xy), distance(a.xy, b.xy));
					float size = dist / 3f;
					vec3 col = v3(basecolor);
					col *= .1f + .9f * (r.surfacenorm().norm() ^ r.rayvec().norm());
					od.update(scene.time, v4(col, 1f), v4(loc, 1f), size);
					od.draw(scene.g);
					continue;
cull:
					od.update(scene.time, null, null, 0f);
				}
#endif
			}
			for (int i = 0; i < txtdots.Length; i++) {
				vec2 px = txtpoints[i] + v2(-2000 * scene.progress, 0f);
				if (isOnScreen(px)) {
					var owner = screen2.ownerAt(px);
					if (owner is Tri && (owner as Tri).color == textplane.color) {
						txtdots[i].update(scene.time, v4(1f), v4(px, 1f, 1f));
						txtdots[i].draw(scene.g);
						continue;
					}
				}
				txtdots[i].update(scene.time, null, null);
			}
			textplane.setpts(opt);
		}

		public override void fin(Writer w) {
#if SCREEN
			screen.fin(w);
#else
			foreach (Odot o in dots) {
				o.fin(w);
			}
			ICommand.allow_mx_my.Push(true);
			foreach (Odot o in txtdots) {
				o.fin(w);
			}
			ICommand.allow_mx_my.Pop();
#endif
		}

	}
}
}
