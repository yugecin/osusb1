using System;
using System.Text;

namespace osusb1 {
partial class all {
	class Zsc : Z {

		public static vec3 mid = v3(0f, 30f, 100f);
		public static vec3 dp = v3(0f);
		public static vec4 lquatx = v4(0f);
		public static vec4 lquaty = v4(0f);

		public Zsc(int start, int stop) {
			this.start = start;
			this.stop = stop;
			framedelta = 100;
		}

		public override void draw(SCENE scene) {
			dp = v3(0f);
			float amp = mid.y - campos.y;
			dp.x = amp * sin(progressx(34500f, 48500f, scene.time) * TWOPI);
			dp.y = -amp + amp * cos(progressx(34500f, 48500f, scene.time) * TWOPI);
			dp.z = amp / 2f * sin(progressx(34500f, 48500f, scene.time) * TWOPI);
			vec2 vd = viewdir(campos, mid + dp);
			lquatx = quat(0f, 0f, vd.x);
			lquaty = quat(0f, vd.y, 0f);
		}

		public static void adjust(vec3[] points) {
			move(points, dp);
			turn(points, campos, lquatx);
			turn(points, campos, lquaty);
		}

		public override void fin(Writer w) {
		}

	}
}
}
