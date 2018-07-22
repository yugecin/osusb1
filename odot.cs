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

		public void update(int time, vec4 col, vec4 c, float size) {
			if (c != null && (c.z < 0.2f || size < 1f)) {
				this.update(time, null, null, 0f);
				return;
			}
			times.AddLast(time);
			cols.AddLast(col);
			coords.AddLast(c == null ? null : c.xy);
			sizes.AddLast(size);
		}

		public void draw(Graphics g) {
			if (g != null && times.Count > 0 && cols.Last.Value != null) {
				Color col = cols.Last.Value.col();
				vec2 c = coords.Last.Value;
				int size = (int) sizes.Last.Value;
				int size2 = size / 2;
				g.FillRectangle(new SolidBrush(col), c.x - size2, c.y - size2, size, size);
			}
		}

		public void fin(Writer w) {
			bool wascheck = w.check;
			w.check = false;
			LinkedListNode<int> _time = times.First;
			LinkedListNode<vec4> _col = cols.First;
			LinkedListNode<vec2> _pos = coords.First;
			LinkedListNode<float> _size = sizes.First;

			Sprite s = null;
			const float spritesize = 6f;

			while (_time != null) {

				if (_col.Value == null) {
					if (s == null) {
						goto next;
					}
					s.startframe(_time.Value);
					s.hide();
					goto next;
				}

				if (s == null) {
					s = new Sprite("d", _pos.Value);
				}
				s.startframe(_time.Value);

				s.move(_pos.Value);
				s.color(_col.Value);
				s.scale(_size.Value / spritesize);

next:
				_time = _time.Next;
				_pos = _pos.Next;
				_col = _col.Next;
				_size = _size.Next;
			}

			if (s != null) {
				s.fin(w);
			}
		}
	}
}
}
