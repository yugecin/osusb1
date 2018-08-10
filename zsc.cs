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
			float x = progressx(34500f, 50000f, scene.time);
			float _x = x;
			x = eq_out_sine(x);
			/*
			float amp1 = 120f;
			float amp2 = 90;
			float amp3 = 140f;
			float amp4 = 130f;
			float amp5 = 110f;
			float amp = 0f;
			amp += step(x, .2f) * lerp(amp1, amp2, eq_out_sine(progressx(0f, .2f, x)));
			amp += steq(.19999f, x, .4f) * lerp(amp2, amp3, eq_in_out_sine(progressx(.2f, .4f, x)));
			amp += steq(.39999f, x, .7f) * lerp(amp3, amp4, eq_in_out_sine(progressx(.4f, .7f, x)));
			amp += step(.69999f, x) * lerp(amp4, amp5, eq_in_sine(progressx(.7f, 1f, x)));
			*/
			float amp = lerp(120f, 90f, sin(_x / PI));
			dp.x = amp * sin(x * TWOPI);
			dp.y = -amp + amp * cos(x * TWOPI);
			dp.z = amp / 2f * sin(x * TWOPI);
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
