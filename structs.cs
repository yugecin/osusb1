using System;
using System.Drawing;

namespace osusb1 {
partial class all {
	struct SCENE {
		public int starttime;
		public int endtime;
		public int time;
		public int reltime;
		public float progress;
		public Graphics g;
		public SCENE(int starttime, int endtime, int time, Graphics g) {
			this.starttime = starttime;
			this.endtime = endtime;
			this.time = time;
			this.reltime = this.time - this.starttime;
			this.progress = reltime / (float) (this.endtime - this.starttime);
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
}
