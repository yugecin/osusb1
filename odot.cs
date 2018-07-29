using System;
using System.Collections.Generic;
using System.Drawing;

namespace osusb1 {
partial class all {
	class Odot {
		
		LinkedList<int> times = new LinkedList<int>();
		LinkedList<vec4> cols = new LinkedList<vec4>();
		LinkedList<vec2> coords = new LinkedList<vec2>();
		LinkedList<float> sizes = new LinkedList<float>();

		List<Sprite> sprites = new List<Sprite>();
		Sprite sprite;

		vec4 col;
		vec2 pos;
		float size;

		public void update(int time, vec4 col, vec4 c, float size) {
			if (c != null && (c.z < 0.2f || size < 1f)) {
				update(time, null, null, 0f);
				return;
			}
			this.col = col;
			this.pos = c == null ? null : c.xy;
			this.size = size;
			if (!rendering) {
				return;
			}

			if (c == null) {
				sprite = null;
				return;
			}

			if (sprite == null) {
				sprite = Sprite.dot6_12();
				sprites.Add(sprite);
			}
			
			sprite.update(time, c.xy, col, 1f, size);
		}

		public void draw(Graphics g) {
			if (g != null && col != null) {
				int size2 = (int) size / 2;
				g.FillRectangle(new SolidBrush(col.col()), pos.x - size2, pos.y - size2, size, size);
			}
		}

		public void fin(Writer w) {
			foreach (Sprite s in sprites) {
				s.fin(w);
			}
			sprites.Clear();
		}
	}
}
}
