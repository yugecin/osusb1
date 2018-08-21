using System;
using System.Text;

namespace osusb1 {
partial class all {
	class Zlc : Z {

		public static vec3 mid = v3(campos);

		public static vec3 dp = v3(0f);
		public static vec4 lquatx = v4(0f);
		public static vec4 lquaty = v4(0f);
		public static vec4 lquatz = v4(0f);

		public Zlc(int start, int stop) {
			this.start = start;
			this.stop = stop;
			framedelta = 100;
		}

		public override void draw(SCENE scene) {
			const int DOWNTIME = 2000;
			dp = v3(0f);
			float rotprogress = clamp(scene.reltime - DOWNTIME, 0, stop);
			rotprogress /= 20000;
			dp.x = cos(rotprogress * TWOPI + PI2);
			dp.y = sin(rotprogress * TWOPI + PI2);
			float d = Zcheckerboard.SPACING * Zcheckerboard.SIZE / 2;
			d *= 1.2f;
			float x = (1f - progressx(0, DOWNTIME, scene.reltime)) * PI2;
			dp.xy *= d * cos(x);
			dp.z -= sin(x) * d + 20f;

			vec2 vd = viewdir(campos, mid + dp);
			lquatx = quat(0f, 0f, vd.x);
			lquaty = quat(0f, vd.y, 0f);

			float turnprogress = progressx(0, DOWNTIME, scene.reltime);
			turnprogress = 0f;
			lquatz = quat(turnprogress * PI4 / 2f, 0f, 0f);
		}

		public static void adjust(vec3[] points) {
			if (!rendering) {
				turn(points, mid, quat(0f, rad(mouse.y), 0f));
				turn(points, mid, quat(0f, 0f, rad(mouse.x)));
			}
			move(points, dp);
			turn(points, campos, lquatx);
			turn(points, campos, lquaty);
			turn(points, campos, lquatz);
		}

		public override void fin(Writer w) {
		}

	}
}
}
