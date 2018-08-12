using System;
using System.Drawing;
using System.Text;

namespace osusb1 {
partial class all {
	class Odottedrect {

		public static bool screentest = true;

		public readonly Rect r;
		private readonly int dotcount;
		private float size;
		private Odot[] dots;

		public Odottedrect(Rect rect, int dotcount, float size, int spritesettings) {
			this.r = rect;
			this.dotcount = dotcount;
			this.size = size;
			this.dots = new Odot[dotcount * dotcount];
			for (int i = 0; i < this.dots.Length; i++) {
				this.dots[i] = new Odot(Sprite.SPRITE_SQUARE_6_6, spritesettings);
			}
		}

		public void addCommandOverride(ICommand cmd) {
			foreach (Odot o in dots) {
				o.addCommandOverride(cmd);
			}
		}

		public void draw(SCENE scene, Pixelscreen screen) {
			if (r.shouldcull()) {
				for (int i = 0; i < this.dots.Length; i++) {
					this.dots[i].update(scene.time, null, null, 0f);
				}
				return;
			}
			for (int x = 0; x < dotcount; x++) {
				for (int y = 0; y < dotcount; y++) {
					Odot dot = this.dots[x * dotcount + y];
					float INC = 1f / dotcount;
					float dx = x * (1f - INC) / (dotcount - 1) + INC / 2f;
					float dy = y * (1f - INC) / (dotcount - 1) + INC / 2f;
					vec3 ab = lerp(r.pts[r.a], r.pts[r.b], dx);
					vec3 cd = lerp(r.pts[r.c], r.pts[r.d], dx);
					vec3 pt = lerp(ab, cd, dy);
					vec4 loc = p.Project(pt);
					if (!isOnScreen(loc.xy)) {
						goto norender;
					}
					if (screentest) {
						object o = screen.ownerAt(loc.xy);
						if (!(o is Tri)) {
							goto norender;
						}
						if (((Tri) o).owner != r) {
							goto norender;
						}
					}
					dot.update(scene.time, col(r.color), loc);
					dot.draw(scene.g);
					continue;
norender:
					dot.update(scene.time, null, null, size);
					continue;
				}
			}
		}

		public void fin(Writer w) {
			foreach (Odot o in this.dots) {
				o.fin(w);
			}
		}
	}
}
}
