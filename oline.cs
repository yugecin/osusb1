using System;
using System.Collections.Generic;
using System.Drawing;

namespace osusb1 {
partial class all {
	class Oline {
		
		List<Sprite> sprites = new List<Sprite>();
		Sprite sprite;
		
		public
		vec3[] pts;
		public
		int a, b;

		vec4 col;
		vec2 pos;
		float rot;
		vec2 size;

		List<ICommand> overrides = new List<ICommand>();

		public Oline(vec3[] pts, int a, int b) {
			this.pts = pts;
			this.a = a;
			this.b = b;
		}

		public void addCommandOverride(ICommand cmd) {
			overrides.Add(cmd);
			if (sprite != null) {
				sprite.addOverride(cmd);
			}
		}

		public void update(int time, vec4 col) {
			vec4 a = p.Project(pts[this.a]);
			vec4 b = p.Project(pts[this.b]);
			if (a.z < .2f || b.z < .2f || !isOnScreen(a.xy) || !isOnScreen(b.xy)) {
				update0(time, null, 0f, null, null);
				return;
			}
			update0(time, col, angle(a.xy, b.xy), a, v2((a.xy - b.xy).length(), 1f));
		}

		private void update0(int time, vec4 col, float rot, vec4 c, vec2 size) {
			if (c != null && (c.z < 0.2f || (size.x < 1f && size.y < 1f) || col.w < 0.001f)) {
				update0(time, null, 0f, null, null);
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
				update0(time, null, 0f, null, null);
				return;
			}

			if (sprite == null) {
				sprite = new Sprite(Sprite.SPRITE_SQUARE_1_1, Sprite.ORIGIN_BOTTOMLEFT);
				sprites.Add(sprite);
				foreach (ICommand cmd in overrides) {
					sprite.addOverride(cmd.copy());
				}
			}
			
			sprite.update(time, c.xy, rot, col, 1f, size);
		}

		public void draw(Graphics g) {
			if (g != null && col != null) {
				vec2 p2 = v2(pos.x + size.x * cos(rot), pos.y + size.x * sin(rot));
				g.DrawLine(new Pen(col.col()), pos.pointf(), p2.pointf());
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
