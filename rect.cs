﻿using System;
using System.Drawing;
using System.Text;

namespace osusb1 {
partial class all {
	class Rect {

		public Tri tri1;
		public Tri tri2;
		public Color color;

		public Rect(Color color, vec3[] points, int a, int b, int c, int d) {
			/*
			 * a-b
			 * | |
			 * c-d
			 */
			this.color = color;
			this.tri1 = new Tri(color, points, a, b, c);
			this.tri2 = new Tri(color, points, c, d, b);
		}

		public bool shouldcull() {
			return this.tri1.shouldcull();
		}

	}
}
}