using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace osusb1 {
partial class all {
	public static vec2 v2() {
		return new vec2(0f, 0f);
	}
	public static vec2 v2(float v) {
		return new vec2(v, v);
	}
	public static vec2 v2(float x, float y) {
		return new vec2(x, y);
	}
	public static vec2 v2(vec2 o) {
		return new vec2(o.x, o.y);
	}
	public class vec2 {
		public float x;
		public float y;
		internal vec2(float x, float y) {
			this.x = x;
			this.y = y;
		}
		public vec2(float v) {
			this.x = v;
			this.y = v;
		}
		public float distance(vec2 a) {
			float dx = (x - a.x);
			float dy = (y - a.y);
			return sqrt(dx * dx + dy * dy);
		}
		public override string ToString() {
			return string.Format("v2({0},{1})", x, y);
		}
	}
	public static vec3 v3() {
	       return new vec3(0f, 0f, 0f);
	}
	public static vec3 v3(float v) {
		return new vec3(v, v, v);
	}
	public static vec3 v3(vec2 xy, float z) {
		return new vec3(xy.x, xy.y, z);
	}
	public static vec3 v3(float x, float y, float z) {
		return new vec3(x, y, z);
	}
	public static vec3 v3(vec3 o) {
		return new vec3(o.x, o.y, o.z);
	}
	public class vec3 {
		public float x;
		public float y;
		public float z;
		internal vec3(float x, float y, float z) {
			this.x = x;
			this.y = y;
			this.z = z;
		}
		public vec2 xy { get { return new vec2(x, y); } }
		public static vec3 operator +(vec3 a, float v) {
			return new vec3(a.x + v, a.y + v, a.z + v);
		}
		public static vec3 operator -(vec3 a, float v) {
			return new vec3(a.x - v, a.y - v, a.z - v);
		}
		public static vec3 operator *(vec3 a, float v) {
			return new vec3(a.x * v, a.y * v, a.z * v);
		}
		public static vec3 operator /(vec3 a, float v) {
			return new vec3(a.x / v, a.y / v, a.z / v);
		}
		public static vec3 operator +(vec3 a, vec3 b) {
			return new vec3(a.x + b.x, a.y + b.y, a.z + b.z);
		}
		public static vec3 operator -(vec3 a, vec3 b) {
			return new vec3(a.x - b.x, a.y - b.y, a.z - b.z);
		}
		public static vec3 operator *(vec3 a, vec3 b) {
			return new vec3(a.x * b.x, a.y * b.y, a.z * b.z);
		}
		public static vec3 operator /(vec3 a, vec3 b) {
			return new vec3(a.x / b.x, a.y / b.y, a.z / b.z);
		}
		// cross
		public static vec3 operator %(vec3 a, vec3 b) {
			return new vec3(a.y * b.z - a.z * b.y, a.z * b.x - a.x * b.z, a.x * b.y - a.y - b.x);
		}
		// dot
		public static float operator ^(vec3 a, vec3 b) {
			return a.x * b.x + a.y * b.y + a.z * b.z;
		}
		public float distance(vec3 a) {
			float dx = (x - a.x);
			float dy = (y - a.y);
			float dz = (z - a.z);
			return sqrt(dx * dx + dy * dy + dz * dz);
		}
		public float length() {
			return sqrt(x * x + y * y + z * z);
		}
		public vec3 norm() {
			float l = length();
			return new vec3(x / l, y / l, z / l);
		}
		public Color col() {
			return Color.FromArgb((int) (255f * x), (int) (255f * y), (int) (255f * z));
		}
		public override string ToString() {
			return string.Format("v3({0},{1},{2})", x, y, z);
		}
	}
	public static vec4 v4() {
		return new vec4(0f, 0f, 0f, 0f);
	}
	public static vec4 v4(float v) {
		return new vec4(v, v, v, v);
	}
	public static vec4 v4(vec3 xyz, float w) {
		return new vec4(xyz.x, xyz.y, xyz.z, w);
	}
	public static vec4 v4(float x, float y, float z, float w) {
		return new vec4(x, y, z, w);
	}
	public static vec4 v4(vec4 o) {
		return new vec4(o.x, o.y, o.z, o.w);
	}
	public class vec4 {
		public float x;
		public float y;
		public float z;
		public float w;
		internal vec4(float x, float y, float z, float w) {
			this.x = x;
			this.y = y;
			this.z = z;
			this.w = w;
		}
		public vec2 xy { get { return new vec2(x, y); } }
		public vec3 xyz { get { return new vec3(x, y, z); } }
		public static vec4 operator +(vec4 a, float v) {
			return new vec4(a.x + v, a.y + v, a.z + v, a.w + v);
		}
		public static vec4 operator -(vec4 a, float v) {
			return new vec4(a.x - v, a.y - v, a.z - v, a.w - v);
		}
		public static vec4 operator *(vec4 a, float v) {
			return new vec4(a.x * v, a.y * v, a.z * v, a.w * v);
		}
		public static vec4 operator /(vec4 a, float v) {
			return new vec4(a.x / v, a.y / v, a.z / v, a.w / v);
		}
		public static vec4 operator +(vec4 a, vec4 b) {
			return new vec4(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);
		}
		public static vec4 operator -(vec4 a, vec4 b) {
			return new vec4(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);
		}
		public static vec4 operator *(vec4 a, vec4 b) {
			return new vec4(a.x * b.x, a.y * b.y, a.z * b.z, a.w * b.w);
		}
		public static vec4 operator /(vec4 a, vec4 b) {
			return new vec4(a.x / b.x, a.y / b.y, a.z / b.z, a.w / b.w);
		}
		public override string ToString() {
			return string.Format("v4({0},{1},{2},{3})", x, y, z, w);
		}
	}
}
}
