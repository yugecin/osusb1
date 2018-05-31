using System;
using System.Collections.Generic;
using System.Text;

namespace osusb1 {
partial class all {
	public static float deg(float rad) {
		return (float) (rad * 180d / Math.PI);
	}
	public static float rad(float deg) {
		return (float) (deg * Math.PI / 180d);
	}
	public static int sqrt(int a) {
		return (int) Math.Sqrt(a);
	}
	public static float cos(float a) {
		return (float) Math.Cos(a);
	}
	public static float tan(float a) {
		return (float) Math.Tan(a);
	}
	public static float sin(float a) {
		return (float) Math.Sin(a);
	}
	public static float atan2(float a, float b) {
		return (float) Math.Atan2(a, b);
	}
	public static float sqrt(float a) {
		return (float) Math.Sqrt(a);
	}
	public static float min(float a, float b) {
		return (float) Math.Min(a, b);
	}
	public static float max(float a, float b) {
		return (float) Math.Max(a, b);
	}
	public static float min(float a, float b, float c) {
		return (float) Math.Min(Math.Min(a, b), c);
	}
	public static float max(float a, float b, float c) {
		return (float) Math.Max(Math.Max(a, b), c);
	}
	public static int min(int a, int b) {
		return Math.Min(a, b);
	}
	public static int max(int a, int b) {
		return Math.Max(a, b);
	}
	public static float lerp(float a, float b, float x) {
		return a + (b - a) * x;
	}
	public static float progress(float a, float b, float x) {
		return (x - a) / (b - a);
	}
	public static vec3 lerp(vec3 a, vec3 b, float x) {
		return v3(lerp(a.x, b.x, x), lerp(a.y, b.y, x), lerp(a.z, b.z, x));
	}
	public static float distance(vec2 a, vec2 b) {
		return a.distance(b);
	}
	public static float distance(vec3 a, vec3 b) {
		return a.distance(b);
	}
	static void turn(Cube c, vec3 mid, float xang, float yang) {
		foreach (Rect r in c.rects) {
			foreach (int idx in new int[] { r.a, r.b, r.c, r.d }) {
				r.pts[idx] = turn(r.pts[idx], mid, xang, yang);
			}
		}
	}
	public static vec3[] turn(vec3[] p, vec3 mid, float xang, float yang) {
		vec3[] np = new vec3[p.Length];
		turn(np, p, mid, xang, yang);
		return np;
	}
	public static void turn(vec3[] _out, vec3[] p, vec3 mid, float xang, float yang) {
		for (int i = 0; i < p.Length; i++) {
			_out[i] = turn(p[i], mid, xang, yang);
		}
	}
	public static vec3 turn(vec3 p, vec3 mid, float xang, float yang) {
		vec3 _out = v3();

		float xout = p.x;
		float yout = p.y;
		float zout = p.z;

		if (yang != 0f) {
			float dy = yout - mid.y;
			float dz = p.z - mid.z;
			float ang = atan2(dy, dz);
			float len = sqrt(dy * dy + dz * dz);
			ang += rad(yang);
			yout = mid.y + sin(ang) * len;
			zout = mid.z + cos(ang) * len;
		}

		if (xang != 0f) {
			float dy = yout - mid.y;
			float dx = xout - mid.x;
			float ang = atan2(dy, dx);
			float len = sqrt(dy * dy + dx * dx);
			ang += rad(xang);
			xout = mid.x + cos(ang) * len;
			yout = mid.y + sin(ang) * len;
		}

		_out.x = xout;
		_out.y = yout;
		_out.z = zout;

		return _out;
	}
}
}
