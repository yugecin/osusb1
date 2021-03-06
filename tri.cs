using System;
using System.Collections.Generic;
using System.Drawing;
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

		public vec4[] project() {
			return new vec4[] {
				all.project(this.points[a]),
				all.project(this.points[b]),
				all.project(this.points[c])
			};
		}

		public vec3 surfacenorm() {
			return (points[b] - points[a]) % (points[c] - points[a]);
		}

		public vec3 rayvec() {
			return points[a] - all.campos;
		}

		public bool shouldcull() {
			return (surfacenorm() ^ rayvec()) < 0f;
		}


	}

}
}
