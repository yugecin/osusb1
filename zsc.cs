using System;
using System.Text;

namespace osusb1 {
partial class all {
	class Zsc : Z {

		public static vec3 mid = v3(0f, 30f, 100f);
		public static vec3 dp = v3(0f);
		public static vec4 lquatx = v4(0f);
		public static vec4 lquaty = v4(0f);

		public const int TUNNEL_RAD = 132; // mid.y - campos.y + 2

		public Zsc(int start, int stop) {
			this.start = start;
			this.stop = stop;
			framedelta = 100;
		}

		public override void draw(SCENE scene) {
			dp = v3(0f);
			float x = progressx(17350f, 50000, scene.time);
			//float amp = lerp(120f, 90f, sin(_x / PI));
			float amp = 120f;
			dp.x = -amp * sin(x * PI);
			dp.y = -amp + amp * cos(x * PI);
			dp.z = -amp / 2f * .6f * sin(x * PI);
			vec2 vd = viewdir(campos, mid + dp);
			lquatx = quat(0f, 0f, vd.x);
			lquaty = quat(0f, vd.y, 0f);
		}

		public static void adjust(vec3[] points) {
			turn(points, mid, quat(0f, rad(mouse.y), rad(mouse.x)));
			move(points, dp);
			turn(points, campos, lquatx);
			turn(points, campos, lquaty);
		}

		public override void fin(Writer w) {
		}

	}
}
}
