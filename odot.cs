using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace osusb1 {
	class Odot : O {

		public P3D p;

		public Odot() {

		}

		public Odot(P3D p) {
			this.p = p;
		}

		public void update(P3D p) {
			this.p = p;
		}

		public override void draw(Graphics g) {
			if (g == null) {
				return;
			}
			if (p.z < 1) {
				return;
			}
			g.FillRectangle(new SolidBrush(Color.White), p.x - 2f, p.y - 2f, 4, 4);
		}

	}
}
