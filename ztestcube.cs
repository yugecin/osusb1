using System;
using System.Collections.Generic;
using System.Drawing;

namespace osusb1 {
	class Ztestcube : Z {

		P3D mid = new P3D(0f, 0f, 100f);
		P3D[] points = new P3D[] {
			new P3D(-10f, -10f, 90f),
			new P3D(-10f, -10f, 110f),
			new P3D(-10f, 10f, 90f),
			new P3D(-10f, 10f, 110f),
			new P3D(10f, -10f, 90f),
			new P3D(10f, -10f, 110f),
			new P3D(10f, 10f, 90f),
			new P3D(10f, 10f, 110f),
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

		public override void draw(int time, int reltime, float progress, Projection p, Graphics g) {
			int i = 0;
			foreach (P3D point in Ang.turn(points, mid, 200f * progress, 300f * progress)) {
				dots[i].update(p.Project(point));
				dots[i].draw(g);
				++i;
			}
			foreach (Sline s in lines) {
				s.draw(g);
			}

			if (g == null) {
				foreach (Sline l in lines) {
					l.update(time);
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
