using System;
using System.Collections.Generic;
using System.Drawing;

namespace osusb1 {
partial class all {
	class Odot {
		
		List<Sprite> sprites = new List<Sprite>();
		Sprite sprite;

		List<ICommand> overrides = new List<ICommand>();

		vec4 col;
		vec2 pos;
		float size;
		string spritename;
		int spritesettings;
		bool wasOOB;

		public Odot(string spritename, int spritesettings) {
			this.spritename = spritename;
			this.spritesettings = spritesettings;
			wasOOB = true;
		}

		public void addCommandOverride(ICommand cmd) {
			overrides.Add(cmd);
			if (sprite != null) {
				sprite.addOverride(cmd);
			}
		}

		public void update(int time, vec4 col, vec4 c) {
			update(time, col, c, Sprite.Size(spritename));
		}

		public void update(int time, vec4 col, vec4 c, float size) {
			if (c != null && (c.z < 0.2f || size < 1f || col.w == 0f)) {
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
				// TODO: YUK this depends on state D:
				if (wasOOB || (spritesettings & Sprite.INTERPOLATE_MOVE) == 0) {
					update(time, null, null, 0f);
					return;
				}
				wasOOB = true;
			}

			if (sprite == null) {
				sprite = new Sprite(spritename, spritesettings);
				sprites.Add(sprite);
				foreach (ICommand cmd in overrides) {
					sprite.addOverride(cmd.copy());
				}
			}
			
			sprite.update(time, c.xy, 0f, col, 1f, v2(size));
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
