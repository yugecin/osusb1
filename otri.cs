using System;
using System.Collections.Generic;
using System.Drawing;

namespace osusb1 {
partial class all {
	class Otri {
		
		List<Sprite> sprites = new List<Sprite>();
		Sprite sprite;

		vec4 col;
		vec2 pos;
		float rot;
		vec2 size;

		public void update(int time, vec4 col, float rot, vec4 c, vec2 size) {
			if (c != null && (c.z < 0.2f || (size.x < 1f && size.y < 1f))) {
				update(time, null, 0f, null, v2(0f));
				return;
			}

			this.col = col;
			this.pos = c == null ? null : c.xy;
			this.rot = rot;
			this.size = size;

			if (!rendering) {
				return;
			}

			if (c == null) {
				sprite = null;
				return;
			}

			if (!isOnScreen(pos, size)) {
				update(time, null, 0f, null, v2(0f));
				return;
			}

			if (sprite == null) {
				sprite = new Sprite(Sprite.SPRITE_TRI, Sprite.ORIGIN_BOTTOMLEFT);
				sprites.Add(sprite);
			}
			
			sprite.update(time, c.xy, rot, col, 1f, size);
		}

		public void draw(Graphics g) {
			if (g != null && col != null) {
				vec2 p1 = pos + v2(size.x * cos(rot), size.x * sin(rot));
				float r2 = rot + PI2;
				vec2 p2 = p1 - v2(size.y * cos(r2), size.y * sin(r2));
				g.FillPolygon(new SolidBrush(col.col()), new PointF[] { pos.pointf(), p1.pointf(), p2.pointf() });
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
