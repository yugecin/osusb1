using System;
using System.Drawing;
using System.Text;

namespace osusb1 {
partial class all {
	class Rect {

		public object owner;
		public Tri tri1;
		public Tri tri2;
		public Color color;

		public Rect(object owner, Color color, vec3[] points, int a, int b, int c, int d) {
			/*
			 * a-b
			 * | |
			 * c-d
			 */
			this.owner = owner;
			this.color = color;
			this.tri1 = new Tri(this, color, points, a, b, c);
			this.tri2 = new Tri(this, color, points, c, d, b);
		}

		public bool shouldcull() {
			return this.tri1.shouldcull();
		}

		public void draw(Pixelscreen screen) {
			if (this.shouldcull()) {
				return;
			}
			screen.tri(this.tri1, this.tri1.project(p));
			screen.tri(this.tri2, this.tri2.project(p));
		}

	}
}
}
