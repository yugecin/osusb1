using System;

namespace osusb1 {
partial class all {
	class Z002Aspect : Z0020spect {

		public Z002Aspect(int start, int stop) : base(start, stop) {
		}

		protected override int reorder(int idx) {
			return orects.Length - idx - 1;
		}

		protected override bool shoulddrawcube(Cube c) {
			return !base.shoulddrawcube(c);
		}
	}
}
}
