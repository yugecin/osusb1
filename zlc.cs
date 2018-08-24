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

		const int T1 = 121000;
		const int T2 = 125400;
		const int T3 = 129800;
		const int T4 = 134000;
		const int T5 = 138200;

		public Zlc(int start, int stop) {
			this.start = start;
			this.stop = stop;
			framedelta = 100;
		}

		public override void draw(SCENE scene) {
			dp = v3(0f);
			vec3 dir = v3(0f);
			float rz = 0f;

			if (scene.time < T2) {
				float pr = progress(T1, T2, scene.time);
				vec3 fr = v3(Zltext.SIZE * 25, 5f, -10f);
				vec3 to = v3(Zltext.SIZE * -15, 40f, -10f);
				dp += lerp(fr, to, pr);
				fr = v3(1f, 0f, 0f);
				to = v3(-.3f, 1f, 0f);
				dir = lerp(fr, to, pr);
				rz = lerp(-PI2, 0, pr);
			} else if (scene.time < T3) {
				dir = v3(0f, 1f, 0f);
				float pr = progress(T2, T3, scene.time);
				vec3 fr = v3(Zltext.SIZE * -20, 25f, -10f);
				vec3 to = v3(Zltext.SIZE * 15, 25f, -10f);
				dp += lerp(fr, to, pr);
			} else if (scene.time < T4) {
				float pr = progress(T3, T4, scene.time);
				float frangle = -PI2;
				float toangle = PI4;
				vec3 fr = v3(0f, 25f + 10f * cos(frangle), -10f + 10f * sin(frangle));
				vec3 to = v3(0f, 25f + 10f * cos(toangle), -10f + 10f * sin(toangle));
				dp += lerp(fr, to, pr);
				dir = dp - v3(0f, 0f, -10f);
				float fx = 25;
				float tx = -5;
				dp.x = lerp(fx, tx, eq_cub(pr, v2(.5f, .6f), v2(.9f, 1f))) * Zltext.SIZE;
				rz = PI4 / 3f;
			} else {
				float pr = progress(T4, T5, scene.time);
				vec3 fr = v3(Zltext.SIZE * -35, 25f, -10f);
				vec3 to = v3(Zltext.SIZE * 35, 25f, -10f);
				dp += lerp(fr, to, eq_cub(pr, v2(.2f, .4f), v2(.9f, .6f)));
				dir = dp - v3(0f, 0f, -10f);
				//rz = lerp(-PI2, 0, pr);
				rz = 0f;
			}

			vec2 vd = viewdir(dir);
			lquatx = quat(0f, 0f, vd.x);
			lquaty = quat(0f, vd.y, 0f);
			lquatz = quat(rz, 0f, 0f);
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
