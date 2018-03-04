using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace osusb1 {
	class Spixelscreendot {

		private int x, y;
		private List<Pair<int, Color>> col = new List<Pair<int, Color>>();

		public Spixelscreendot(int x, int y, int time, Color color) {
			this.x = x;
			this.y = y;
			this.col.Add(new Pair<int, Color>(time, color));
		}

		public void update(int time, Color color) {
			this.col.Add(new Pair<int, Color>(time, color));
		}

		public void hide(int time) {
			this.col.Add(new Pair<int, Color>(time, Color.Transparent));
		}

		public void draw(Graphics g) {
		}

		public void fin(Writer w) {
			w.Sprite(origin: "TopLeft", sprite: "~", x: x, y: y);
			Color lastcol = Color.Transparent;
			bool ishidden = false;
			int lasttime = 0;
			bool BLACKINSTEADOFTRANS = true;
			for (int i = 0; i < col.Count; i++) {
				int time = col[i].a;
				Color c = col[i].b;
				if (c == Color.Transparent) {
					if (ishidden) {
						continue;
					}
					ishidden = true;
					lasttime = time;
					if (BLACKINSTEADOFTRANS) {
						w._Ci(time, Color.Black);
						lastcol = Color.Black;
					} else {
						w._Fi(time, 1f, 0f);
					}
					continue;
				}
				if (ishidden) {
					ishidden = false;
					if (!BLACKINSTEADOFTRANS) {
						w._Fi(time, 0f, 1f);
					}
				}
				if (lastcol != c) {
					w._Ci(time, c);
					lastcol = c;
					lasttime = time;
				}
			}
			if (lasttime != col[col.Count - 1].a && !ishidden) {
				w._Fi(col[col.Count - 1].a, 1f, 1f);
			}
			w.ln("");
		}

	}
}
