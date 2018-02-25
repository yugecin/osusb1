using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace osusb1 {
	class Ang {
		public static double Deg(double rad) {
			return rad * 180d / Math.PI;
		}
		public static double Rad(double deg) {
			return deg * Math.PI / 180d;
		}
		public static P3D[] turn(P3D[] p, P3D mid, float xang, float yang) {
			P3D[] np = new P3D[p.Length];
			for (int i = 0; i < p.Length; i++) {
				np[i] = turn(p[i], mid, xang, yang);
			}
			return np;
		}
		public static P3D turn(P3D p, P3D mid, float xang, float yang) {
			P3D _out;

			float xout = p.x;
			float yout = p.y;

			if (yang == 0f) {
				_out.z = p.z;
			} else {
				double dy = yout - mid.y;
				double dz = p.z - mid.z;
				double ang = Math.Atan2(dy, dz);
				double len = Math.Sqrt(dy * dy + dz * dz);
				ang += Ang.Rad(yang);
				yout = (float) (mid.y + Math.Sin(ang) * len);
				_out.z = (float) (mid.z + Math.Cos(ang) * len);
			}

			if (xang != 0f) {
				double dy = yout - mid.y;
				double dx = xout - mid.x;
				double ang = Math.Atan2(dy, dx);
				double len = Math.Sqrt(dy * dy + dx * dx);
				ang += Ang.Rad(xang);
				xout = (float) (mid.x + Math.Cos(ang) * len);
				yout = (float) (mid.y + Math.Sin(ang) * len);
			}

			_out.x = xout;
			_out.y = yout;
			return _out;
		}
	}
}
