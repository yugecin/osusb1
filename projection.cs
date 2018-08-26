using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace osusb1 {
partial class all {

	public static vec3 campos = v3(0f, -100f, 100f);

	public static vec4 project(vec3 p) {
		vec4 n = v4();
		n.x = 49.999996f + + p.y * .5f + p.x * .714074f;
		n.y = 176.946487f + p.z * -1.269465f + p.y * .5f;
		n.z = 100f + p.y;
		n.w = distance(p, campos);
		if (n.z == 0) {
			n.z = -1f;
		}
		float f = 1f / n.z;
		n.x *= f * 640f;
		n.y *= f * 448f;
		return n;
	}

	static int LOWERBOUND = 0;
	static int UPPERBOUND = 640;
	public static bool Widescreen {
		set {
			LOWERBOUND = value ? -107 : 0;
			UPPERBOUND = value ? 747 : 640;
		}
		get {
			return LOWERBOUND != 0;
		}
	}

	static bool isOnScreen(vec2 pos) {
		int x = (int) pos.x;
		int y = (int) pos.y;
		return LOWERBOUND <= x && x < UPPERBOUND && 0 <= y && y < 480;
	}

	static bool isOnScreen(vec2 pos, float size) {
		float th = (int) (size / 2f);
		int x = (int) pos.x;
		int y = (int) pos.y;
		return LOWERBOUND - th <= x && x < UPPERBOUND + th && -th <= y && y < 480 + th;
	}

	static bool isOnScreen(vec2 pos, vec2 size) {
		vec2 th = size / 2f;
		int thx = (int) th.x;
		int thy = (int) th.y;
		int x = (int) pos.x;
		int y = (int) pos.y;
		return LOWERBOUND - thx <= x && x < UPPERBOUND + thx && -thy <= y && y < 480 + thy;
	}

}
}
