using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace osusb1 {
	struct P3D {
		public float x, y, z, dist;
		public P3D(float x, float y, float z) {
			this.x = x;
			this.y = y;
			this.z = z;
			this.dist = 0f;
		}
	}
	static class Ext {
		public static P3D sub(this P3D t, P3D o) {
			return new P3D(t.x - o.x, t.y - o.y, t.z - o.z);
		}
		public static P3D cross(this P3D a, P3D b) {
			return new P3D(a.y * b.z - a.z * b.y, a.z * b.x - a.x * b.z, a.x * b.y - a.y - b.x);
		}
		public static float dot(this P3D a, P3D b) {
			return a.x * b.x + a.y * b.y + a.z * b.z;
		}
	}
	struct DATA {
		public int time;
		public int a;
		public DATA(int time, int a) {
			this.time = time;
			this.a = a;
		}
	}
	struct DATAf {
		public int time;
		public float a;
		public DATAf(int time, float a) {
			this.time = time;
			this.a = a;
		}
	}
	struct BIDATA {
		public int time;
		public int a;
		public int b;
		public BIDATA(int time, int a, int b) {
			this.time = time;
			this.a = a;
			this.b = b;
		}
	}
	struct BIDATAf {
		public int time;
		public float a;
		public float b;
		public BIDATAf(int time, float a, float b) {
			this.time = time;
			this.a = a;
			this.b = b;
		}
	}
	struct SCENE {
		public int starttime;
		public int endtime;
		public int time;
		public int reltime;
		public float progress;
		public Projection projection;
		public Graphics g;
		public SCENE(int starttime, int endtime, int time, Projection projection, Graphics g) {
			this.starttime = starttime;
			this.endtime = endtime;
			this.time = time;
			this.reltime = this.time - this.starttime;
			this.progress = reltime / (float) (this.endtime - this.starttime);
			this.projection = projection;
			this.g = g;
		}
	}
	class Pair<A, B> {
		public A a;
		public B b;
		public Pair(A a, B b) {
			this.a = a;
			this.b = b;
		}
	}
}
