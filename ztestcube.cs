using System;
using System.Collections.Generic;
using System.Drawing;

namespace osusb1 {
partial class all {
	class Ztestcube : Z {

		vec3 mid = v3(0f, 0f, 100f);
		vec3[] points = new vec3[] {
			v3(-10f, -10f, 90f),
			v3(-10f, -10f, 110f),
			v3(-10f, 10f, 90f),
			v3(-10f, 10f, 110f),
			v3(10f, -10f, 90f),
			v3(10f, -10f, 110f),
			v3(10f, 10f, 90f),
			v3(10f, 10f, 110f),
		};
		Odot[] dots = new Odot[8];
		Sline[] lines = new Sline[12];

		public Ztestcube(int start, int stop) {
			this.start = start;
			this.stop = stop;
			int i = 0;
			for (i = 0; i < 8; i++) {
				dots[i] = new Odot();
			}
			i = 0;
			lines[i++] = new Sline(dots[0], dots[1]);
			lines[i++] = new Sline(dots[2], dots[3]);
			lines[i++] = new Sline(dots[4], dots[5]);
			lines[i++] = new Sline(dots[6], dots[7]);

			lines[i++] = new Sline(dots[0], dots[2]);
			lines[i++] = new Sline(dots[1], dots[3]);
			lines[i++] = new Sline(dots[4], dots[6]);
			lines[i++] = new Sline(dots[5], dots[7]);

			lines[i++] = new Sline(dots[0], dots[4]);
			lines[i++] = new Sline(dots[1], dots[5]);
			lines[i++] = new Sline(dots[2], dots[6]);
			lines[i++] = new Sline(dots[3], dots[7]);
		}

		public override void draw(SCENE scene) {
			int i = 0;
			foreach (vec3 point in turn(points, mid, 200f * scene.progress, 300f * scene.progress)) {
				dots[i].update(scene.projection.Project(point));
				dots[i].draw(scene.g);
				++i;
			}
			foreach (Sline s in lines) {
				s.draw(scene.g);
			}

			if (scene.g == null) {
				foreach (Sline l in lines) {
					l.update(scene.time);
				}
			}
		}

		public override void fin(Writer w) {
			foreach (Sline l in lines) {
				l.fin(w);
			}
		}

	}
}
}
