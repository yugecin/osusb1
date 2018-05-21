using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace osusb1 {
partial class all {
	class Tri {

		public object owner;
		public Color color;
		public vec3[] points;
		public int a, b, c;

		public Tri(object owner, Color color, vec3[] points, int a, int b, int c)
		{
			this.owner = owner;
			this.color = color;
			this.points = points;
			this.a = a;
			this.b = b;
			this.c = c;
		}

		public vec3[] getpoints() {
			return new vec3[] { points[a], points[b], points[c] };
		}

		public vec4[] project(Projection p) {
			return new vec4[] {
				p.Project(this.points[a]),
				p.Project(this.points[b]),
				p.Project(this.points[c])
			};
		}

		public bool shouldcull() {
			vec3 norm = (points[b] - points[a]) % (points[c] - points[a]);
			vec3 v = points[a] - all.campos;
			return (norm ^ v) < 0f;
		}


	}

}
}
