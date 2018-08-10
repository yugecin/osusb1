using System;
using System.Text;

namespace osusb1 {
partial class all {
	class Zdebugdot2 : Z {

		Odot dot;

		public Zdebugdot2(int start, int stop) {
			this.start = start;
			this.stop = stop;
			framedelta = 300;
			dot = new Odot(Sprite.SPRITE_DOT_6_12, 0);
		}

		public override void draw(SCENE scene) {
			float r = (scene.time / framedelta) % 2 == 1 ? 1f : 0f;
			vec4 color = v4(v3(r, 0f, 1f), 1f);
			vec4 pos = v4(100f, 100f, 1f, 10f);
			dot.update(scene.time, color, pos, 6f);
			dot.draw(scene.g);
		}

		public override void fin(Writer w) {
			dot.fin(w);
		}

	}
}
}
