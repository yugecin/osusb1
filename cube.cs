using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace osusb1 {
partial class all {
	class Cube{

		private Rect[] rects = new Rect[6];

		public Cube(
			Color fcol,
			Color lcol,
			Color rcol,
			Color ucol,
			Color dcol,
			Color bcol,
			vec3[] points,
			int flt,
			int frt,
			int frd,
			int fld,
			int bld,
			int blt,
			int brt,
			int brd)
		{
			rects[0] = new Rect(fcol, points, flt, frt, fld, frd);
			rects[1] = new Rect(lcol, points, blt, flt, bld, fld);
			rects[2] = new Rect(rcol, points, frt, brt, frd, brd);
			rects[3] = new Rect(ucol, points, blt, brt, flt, frt);
			rects[4] = new Rect(dcol, points, fld, frd, bld, brd);
			rects[5] = new Rect(bcol, points, brt, blt, brd, bld);
		}

		public void draw(Pixelscreen screen)
		{
			foreach (Rect r in rects) {
				if (r.shouldcull()) {
					continue;
				}
				screen.tri(r.color, r.tri1.project(p));
				screen.tri(r.color, r.tri2.project(p));
			}
		}

	}
}
}
