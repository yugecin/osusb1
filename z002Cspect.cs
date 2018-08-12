using System;

namespace osusb1 {
partial class all {
	class Z002Cspect : Z002Aspect {

		public Z002Cspect(int start, int stop) : base(start, stop) {
		}

		protected override float inprogress(SCENE scene) {
			return 1f;
		}
	}
}
}
