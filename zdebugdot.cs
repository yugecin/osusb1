using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace osusb1 {
partial class all {
	class Zdebugdot : Z {

		Odot dot = new Odot();

		public Zdebugdot(int start, int stop) {
			this.start = start;
			this.stop = stop;
		}

		public override void draw(SCENE scene) {
			if (scene.progress > .2f && scene.progress < .5f) {
				dot.update(scene.time, null, null, 5f);
			} else {
				vec4 color = v4(1f);
				color.x *= scene.progress;
				vec4 pos = v4(100f, 100f, 1f, 10f);
				pos.x += scene.progress * 100f;
				dot.update(scene.time, color, pos, 5f);
			}
			dot.draw(scene.g);
		}

		public override void fin(Writer w) {
			dot.fin(w);
		}

	}
}
}
