using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace osusb1 {
partial class all {

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
			w.Sprite(/*origin*/ "Centre", /*sprite*/ "", /*x*/ x, /*y*/ y);
			Color lastcol = Color.Transparent;
			bool ishidden = false;
			int lasttime = 0;
			for (int i = 0; i < col.Count; i++) {
				int time = col[i].a;
				Color c = col[i].b;
				if (c == Color.Transparent) {
					if (ishidden) {
						continue;
					}
					ishidden = true;
					lasttime = time;
					w._Ci(time, Color.Black);
					lastcol = Color.Black;
					continue;
				}
				if (ishidden) {
					ishidden = false;
				}
				if (lastcol != c) {
					w._Ci(time, c);
					lastcol = c;
					lasttime = time;
				}
			}
			// TODO: merge this with previous color
			if (lasttime != col[col.Count - 1].a && !ishidden) {
				w._Fi(col[col.Count - 1].a, 1f);
			}
		}

	}
}
}
