using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace osusb1 {
	class Sline {

		public Odot a, b;

		private List<BIDATA> pos = new List<BIDATA>();
		private List<BIDATAf> scale = new List<BIDATAf>();
		private List<DATAf> rot = new List<DATAf>();

		public Sline(Odot a, Odot b) {
			this.a = a;
			this.b = b;
		}

		public void update(int time) {
			float dy = b.p.y - a.p.y;
			float dx = b.p.x - a.p.x;
			float rot = (float) Math.Atan2(dy, dx);
			this.rot.Add(new DATAf(time, rot));
			this.pos.Add(new BIDATA(time, (int) a.p.x, (int) a.p.y));
			float length = (float) Math.Sqrt(dy * dy + dx * dx);
			this.scale.Add(new BIDATAf(time, length / 100f, 5f / 100f));
		}

		public void draw(Graphics g) {
			if (g != null) {
				g.DrawLine(new Pen(new SolidBrush(Color.Magenta)), a.p.x, a.p.y, b.p.x, b.p.y);
			}
		}

		public void fin(Writer w) {
			if (pos.Count == 0) {
				return;
			}

			w.Sprite(origin: "TopLeft", sprite: "s.png", x: pos[0].a, y: pos[0].b);
			for (int i = 0; i < pos.Count - 1; i++) {
				int starttime = pos[i].time;
				int endtime = pos[i+1].time - 1;
				endtime = starttime;
				w._M(
					starttime: starttime,
					endtime: endtime,
					startx: pos[i].a,
					starty: pos[i].b,
					endx: pos[i+1].a,
					endy: pos[i+1].b
				);
				w._V(
					starttime: starttime,
					endtime: endtime,
					startscalex: scale[i].a,
					startscaley: scale[i].b,
					endscalex: scale[i+1].a,
					endscaley: scale[i+1].b
				);
				w._R(
					starttime: starttime,
					endtime: endtime,
					startrotate: rot[i].a,
					endrotate: rot[i+1].a
				);
			}
			w.ln("");
		}

	}
}
