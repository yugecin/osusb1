using System;
using System.Collections.Generic;
using System.Drawing;

namespace osusb1 {
partial class all {
	class Odot {
		
		LinkedList<int> times = new LinkedList<int>();
		LinkedList<vec3> cols = new LinkedList<vec3>();
		LinkedList<vec2> coords = new LinkedList<vec2>();

		public void update(int time, vec3 col, vec4 c) {
			if (c != null && c.z < 0) {
				this.update(time, null, null);
				return;
			}
			times.AddLast(time);
			cols.AddLast(col);
			coords.AddLast(c == null ? null : c.xy);
		}

		public void draw(Graphics g) {
			if (g != null && times.Count > 0 && cols.Last.Value != null) {
				Color col = cols.Last.Value.col();
				vec2 c = coords.Last.Value;
				g.FillRectangle(new SolidBrush(col), c.x - 1, c.y - 1, 3, 3);
			}
		}

		public void fin(Writer w) {
		}
	}
}
}
