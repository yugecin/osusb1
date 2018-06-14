using System;
using System.Drawing;
using System.Text;

namespace osusb1 {
partial class all {
	class Odottedrect {

		private Rect r;
		private readonly int dotcount;

		public Odottedrect(Rect rect, int dotcount) {
			this.r = rect;
			this.dotcount = dotcount;
		}

		public void draw(SCENE scene, Pixelscreen screen) {
			if (r.shouldcull()) {
				return;
			}
			for (int x = 0; x < dotcount; x++) {
				for (int y = 0; y < dotcount; y++) {
					float INC = 1f / dotcount;
					float dx = x * (1f - INC) / (dotcount - 1) + INC / 2f;
					float dy = y * (1f - INC) / (dotcount - 1) + INC / 2f;
					vec3 ab = lerp(r.pts[r.a], r.pts[r.b], dx);
					vec3 cd = lerp(r.pts[r.c], r.pts[r.d], dx);
					vec3 pt = lerp(ab, cd, dy);
					vec4 loc = p.Project(pt);
					if (!isOnScreen(loc.xy)) {
						continue;
					}
					object o = screen.ownerAt(loc.xy);
					if (!(o is Tri)) {
						continue;
					}
					if (((Tri) o).owner != r) {
						continue;
					}
					if (scene.g == null) {
						continue;
					}
					//scene.g.DrawRectangle(new Pen(col), lx - 1, ly - 1, 3, 3);
					scene.g.FillRectangle(new SolidBrush(r.color), loc.x - 1, loc.y - 1, 3, 3);
				}
			}
		}

	}
}
}
