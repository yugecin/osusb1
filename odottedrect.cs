using System;
using System.Drawing;
using System.Text;

namespace osusb1 {
partial class all {
	class Odottedrect {

		private Rect r;
		private readonly int dotcount;
		private Odot[] dots;

		public Odottedrect(Rect rect, int dotcount) {
			this.r = rect;
			this.dotcount = dotcount;
			this.dots = new Odot[dotcount * dotcount];
			for (int i = 0; i < this.dots.Length; i++) {
				this.dots[i] = new Odot();
			}
		}

		public void draw(SCENE scene, Pixelscreen screen) {
			if (r.shouldcull()) {
				for (int i = 0; i < this.dots.Length; i++) {
					this.dots[i].update(scene.time, null, null);
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
					object o = screen.ownerAt(loc.xy);
					if (!(o is Tri)) {
						goto norender;
					}
					if (((Tri) o).owner != r) {
						goto norender;
					}
					dot.update(scene.time, col(r.color), loc);
					dot.draw(scene.g);
					continue;
norender:
					dot.update(scene.time, null, null);
					continue;
				}
			}
		}

	}
}
}
