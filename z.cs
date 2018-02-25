using System;
using System.Drawing;

namespace osusb1 {
	abstract class Z {
		public int start;
		public int stop;

		public abstract void draw(int time, int reltime, float progress, Projection p, Graphics g);
	}
}
