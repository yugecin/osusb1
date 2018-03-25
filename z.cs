using System;
using System.Drawing;

namespace osusb1 {
partial class all {
	abstract class Z {
		public int start;
		public int stop;

		public abstract void fin(Writer w);
		public abstract void draw(SCENE scene);
	}
}
}
