using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace osusb1 {
partial class all {
	class Cube{

		public const int F = 0;
		public const int L = 1;
		public const int R = 2;
		public const int U = 3;
		public const int D = 4;
		public const int B = 5;

		public Rect[] rects = new Rect[6];

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
			rects[F] = new Rect(this, fcol, points, flt, frt, fld, frd);
			rects[L] = new Rect(this, lcol, points, blt, flt, bld, fld);
			rects[R] = new Rect(this, rcol, points, frt, brt, frd, brd);
			rects[U] = new Rect(this, ucol, points, blt, brt, flt, frt);
			rects[D] = new Rect(this, dcol, points, fld, frd, bld, brd);
			rects[B] = new Rect(this, bcol, points, brt, blt, brd, bld);
		}

		public Cube(Color[] cols, vec3[] points, int bi) : this(
			cols[0], cols[1], cols[2], cols[3], cols[4], cols[5],
			points,
			bi, bi + 1, bi + 2, bi + 3, bi + 4, bi + 5, bi + 6, bi + 7
		) { }

		public void draw(Pixelscreen screen)
		{
			foreach (Rect r in rects) {
				if (r.shouldcull()) {
					continue;
				}
				screen.tri(r.tri1, r.tri1.project());
				screen.tri(r.tri2, r.tri2.project());
			}
		}

	}
}
}
