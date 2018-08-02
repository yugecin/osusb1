using System;
using System.Drawing;
using System.Text;

namespace osusb1 {
partial class all {
	class Rect {

		public object owner;
		public vec3[] pts;
		public int a, b, c, d;
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
			this.tri2 = new Tri(this, color, points, c, b, d);
			this.pts = points;
			this.a = a;
			this.b = b;
			this.c = c;
			this.d = d;
		}

		public void updatepts(int a, int b, int c, int d) {
			this.a = a;
			this.b = b;
			this.c = c;
			this.d = d;
			tri1.a = a;
			tri1.b = b;
			tri1.c = c;
			tri2.a = c;
			tri2.b = d;
			tri2.c = b;
		}

		public void setColor(Color col) {
			this.tri1.color = this.tri2.color = this.color = col;
		}

		public bool shouldcull() {
			return this.tri1.shouldcull();
		}

		public vec3 surfacenorm() {
			return this.tri1.surfacenorm();
		}

		public vec3 rayvec() {
			return this.tri1.rayvec();
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
