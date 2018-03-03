using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace osusb1 {
	class Tri {

		public Color color;
		public P3D[] points;
		public int a, b, c;

		public Tri(Color color, P3D[] points, int a , int b, int c) {
			this.color = color;
			this.points = points;
			this.a = a;
			this.b = b;
			this.c = c;
		}

		public P3D[] getpoints() {
			return new P3D[] { points[a], points[b], points[c] };
		}

		public Tri project(Projection p) {
			P3D[] points = new P3D[] {
				p.Project(this.points[a]),
				p.Project(this.points[b]),
				p.Project(this.points[c])
			};
			return new Tri(color, points, 0, 1, 2);
		}

	}
}
