using System;

namespace osusb1 {
partial class all {
	class Z002Bspect : Z002Aspect {

		public Z002Bspect(int start, int stop) : base(start, stop) {
		}

		protected override float outprogress(SCENE scene) {
			return 1f;
		}
	}
}
}
