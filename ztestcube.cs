using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace osusb1 {
	class Ztestcube : Z {

		P3D mid = new P3D(0f, 0f, 100f);
		P3D[] dots = new P3D[] {
			new P3D(-10f, -10f, 90f),
			new P3D(-10f, -10f, 110f),
			new P3D(-10f, 10f, 90f),
			new P3D(-10f, 10f, 110f),
			new P3D(10f, -10f, 90f),
			new P3D(10f, -10f, 110f),
			new P3D(10f, 10f, 90f),
			new P3D(10f, 10f, 110f),
		};

		public Ztestcube(int start, int stop) {
			this.start = start;
			this.stop = stop;
		}

		public override void draw(int time, int reltime, float progress, Projection p, Graphics g) {
			foreach (P3D d_ in Ang.turn(dots, mid, 200f * progress, 300f * progress)) {
				P3D d = p.Project(d_);
				if (d.z < 1) {
					continue;
				}
				g.FillRectangle(new SolidBrush(Color.White), d.x - 2f, d.y - 2f, 4, 4);
			}
		}

	}
}
