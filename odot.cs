using System;
using System.Collections.Generic;
using System.Drawing;

namespace osusb1 {
partial class all {
	class Odot {
		
		List<Sprite> sprites = new List<Sprite>();
		Sprite sprite;

		vec4 col;
		vec2 pos;
		float size;
		int spritesettings;
		bool wasOOB;

		public Odot() : this(0) { }

		public Odot(int spritesettings) {
			this.spritesettings = spritesettings;
		}

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

			if (isOnScreen(pos, size)) {
				wasOOB = false;
			} else {
				// this check is to allow one frame offscreen
				// so if it's interpolated it will move oob
				// instead of disappear just before going oob
				// TODO: this should be only done when mov interpolation is used
				// TODO: YUK this depends on state D:
				if (wasOOB) {
					update(time, null, null, 0f);
					return;
				}
				wasOOB = true;
			}

			if (sprite == null) {
				sprite = new Sprite(Sprite.SPRITE_DOT_6_12, spritesettings);
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
