using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace osusb1 {
partial class all {
	class Odot : O {

		public vec4 p;

		public Odot() {

		}

		public Odot(vec4 p) {
			this.p = p;
		}

		public void update(vec4 p) {
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
}
