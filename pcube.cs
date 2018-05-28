using System;
using System.Collections.Generic;
using System.Text;
 
namespace osusb1 {
partial class all {
	class Pcube {

		public vec3[] points;
		public int flt;
		public int frt;
		public int frd;
		public int fld;
		public int bld;
		public int blt;
		public int brt;
		public int brd;

		public Pcube(
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
			this.points = points;
			this.flt = flt;
			this.frt = frt;
			this.frd = frd;
			this.fld = fld;
			this.bld = bld;
			this.blt = blt;
			this.brt = brt;
			this.brd = brd;
		}

		public Pcube(vec3[] points, int bi) : this(
			points, bi, bi + 1, bi + 2, bi + 3, bi + 4, bi + 5, bi + 6, bi + 7
		) { }

		public void set(vec3 basepoint, float width, float depth, float height) {
			float w2 = width / 2f;
			float d2 = depth / 2f;
			float ah = basepoint.z + height;
			points[fld] = v3(basepoint.x - w2, basepoint.y - d2, basepoint.z);
			points[frd] = v3(basepoint.x + w2, basepoint.y - d2, basepoint.z);
			points[frt] = v3(basepoint.x + w2, basepoint.y - d2, ah);
			points[flt] = v3(basepoint.x - w2, basepoint.y - d2, ah);
			points[blt] = v3(basepoint.x - w2, basepoint.y + d2, ah);
			points[brt] = v3(basepoint.x + w2, basepoint.y + d2, ah);
			points[brd] = v3(basepoint.x + w2, basepoint.y + d2, basepoint.z);
			points[bld] = v3(basepoint.x - w2, basepoint.y + d2, basepoint.z);
		}

		public void setheight(float height) {
			float ah = points[fld].z + height;
			points[flt].z = ah;
			points[frt].z = ah;
			points[brt].z = ah;
			points[blt].z = ah;
		}

	}
}
}
