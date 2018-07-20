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
	public static float clamp(float x, float a, float b) {
		return min(max(x, a), b);
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
	public static int floor(float f) {
		return (int) f;
	}
	public static float dot(vec3 a, vec3 b) {
		return a ^ b;
	}
	public static vec3 floor(vec3 v) {
		return v3(floor(v.x), floor(v.y), floor(v.z));
	}
	public static vec4 quat(vec3 angles) {
		return quat(rad(angles.x), rad(angles.y), rad(angles.z));
	}
	public static vec4 quat(float pitch, float roll, float yaw) {
		float cy = cos(yaw * .5f);
		float sy = sin(yaw * .5f);
		float cr = cos(roll * .5f);
		float sr = sin(roll * .5f);
		float cp = cos(pitch * .5f);
		float sp = sin(pitch * .5f);

		vec4 q = v4(0f);
		q.w = cy * cr * cp + sy * sr * sp;
		q.x = cy * sr * cp - sy * cr * sp;
		q.y = cy * cr * sp + sy * sr * cp;
		q.z = sy * cr * cp - cy * sr * sp;
		return q;
	}
	public static vec3 rot(vec3 v, vec4 quat) {
		vec3 r = new vec3(0f, 0f, 0f);
		float n1 = quat.x * 2f;
		float n2 = quat.y * 2f;
		float n3 = quat.z * 2f;
		float n4 = quat.x * n1;
		float n5 = quat.y * n2;
		float n6 = quat.z * n3;
		float n7 = quat.x * n2;
		float n8 = quat.x * n3;
		float n9 = quat.y * n3;
		float n10 = quat.w * n1;
		float n11 = quat.w * n2;
		float n12 = quat.w * n3;
		r.x = (1f - (n5 + n6)) * v.x + (n7 - n12) * v.y + (n8 + n11) * v.z;
		r.y = (n7 + n12) * v.x + (1f - (n4 + n6)) * v.y + (n9 - n10) * v.z;
		r.z = (n8 - n11) * v.x + (n9 + n10) * v.y + (1f - (n4 + n5)) * v.z;
		return r;
	}
	static void turn(Cube c, vec3 mid, vec4 quat) {
		foreach (Rect r in c.rects) {
			foreach (int idx in new int[] { r.a, r.b, r.c, r.d }) {
				r.pts[idx] = turn(r.pts[idx], mid, quat);
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
		return rot(p - mid, quat(0f, rad(yang), rad(xang))) + mid;
	}
	public static vec3 turn(vec3 p, vec3 mid, vec4 quat) {
		return rot(p - mid, quat) + mid;
	}
}
}
