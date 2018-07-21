using System;
using System.Collections.Generic;
using System.Drawing;

namespace osusb1 {
partial class all {
	class Odot {
		
		LinkedList<int> times = new LinkedList<int>();
		LinkedList<vec4> cols = new LinkedList<vec4>();
		LinkedList<vec2> coords = new LinkedList<vec2>();

		public void update(int time, vec4 col, vec4 c) {
			if (c != null && c.z < 0.2f) {
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
			bool wascheck = w.check;
			w.check = false;
			LinkedListNode<int> _time = times.First;
			LinkedListNode<vec4> _col = cols.First;
			LinkedListNode<vec2> _c = coords.First;
			bool exists = false;
			vec2 lastc = null;
			vec4 lastcol = null;
			bool emptyoutput = false;
			int lasttime = 0;
			int time = 0;
			while (_time != null) {
				time = _time.Value;
				vec4 col = _col.Value;
				vec2 c = _c.Value;
				if (col == null){
					exists = false;
					emptyoutput = false;
					goto next;
				}

				emptyoutput = true;
				if (!exists) {
					w.Sprite("1", "d", (int) c.x, (int) c.y);
					lastcol = null;
					lasttime = time;
					lastc = c;
					exists = true;
				}
				if (lasttime == time) {
					goto skip;
				}
				if (!c.Equals(lastc)) {
					int cx = (int) c.x;
					int cy = (int) c.y;
					int lx = (int) lastc.x;
					int ly = (int) lastc.y;
					if (true || lastc == null || (lx != cx && ly != cy)) {
						w._M(lasttime, time, lx, ly, cx, cy);
					} else if (lx != cx) {
						w._MX(lasttime, time, lx, cx);
					} else if (lastc.y != c.y) {
						w._MY(lasttime, time, ly, cy);
					}
					emptyoutput = false;
					lastc = c;
				}
				if (!col.Equals(lastcol)) {
					w._C(lasttime, time, lastcol.col(), col.col());
					lastcol = col;
					emptyoutput = false;
				}

skip:
				lastcol = col;
				lastc = c;
next:
				lasttime = time;
				_time = _time.Next;
				_col = _col.Next;
				_c = _c.Next;
			}
			if (emptyoutput) {
				w._MX(time, time, (int) lastc.x, (int) lastc.x);
			}
			w.check = wascheck;
		}
	}
}
}
