using System;
using System.Drawing;

namespace osusb1 {
partial class all {
	abstract class Z {
		public int start;
		public int stop;
		public int framedelta;
		public int phantomframedelta;

		public abstract void fin(Writer w);
		public abstract void draw(SCENE scene);

		protected int sync(int time) {
			return time - time % framedelta;
		}

		public static int sync(int time, int framedelta) {
			return time - time % framedelta;
		}
	}
}
}
