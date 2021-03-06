﻿using System;
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
		public static vec2 operator +(vec2 a, float v) {
			return new vec2(a.x + v, a.y + v);
		}
		public static vec2 operator -(vec2 a, float v) {
			return new vec2(a.x - v, a.y - v);
		}
		public static vec2 operator *(vec2 a, float v) {
			return new vec2(a.x * v, a.y * v);
		}
		public static vec2 operator /(vec2 a, float v) {
			return new vec2(a.x / v, a.y / v);
		}
		public static vec2 operator +(vec2 a, vec2 b) {
			return new vec2(a.x + b.x, a.y + b.y);
		}
		public static vec2 operator -(vec2 a, vec2 b) {
			return new vec2(a.x - b.x, a.y - b.y);
		}
		public static vec2 operator *(vec2 a, vec2 b) {
			return new vec2(a.x * b.x, a.y * b.y);
		}
		public static vec2 operator /(vec2 a, vec2 b) {
			return new vec2(a.x / b.x, a.y / b.y);
		}
		public float distance(vec2 a) {
			float dx = (x - a.x);
			float dy = (y - a.y);
			return sqrt(dx * dx + dy * dy);
		}
		public float length() {
			return sqrt(x * x + y * y);
		}
		public Point point() {
			return new Point((int) x, (int) y);
		}
		public PointF pointf() {
			return new PointF(x, y);
		}
		public override string ToString() {
			return string.Format("v2({0},{1})", x, y);
		}
		public override bool Equals(object obj) {
			vec2 o = obj as vec2;
			return o != null && x == o.x && y == o.y;
		}
		public override int GetHashCode() {
			float v = x;
			v = v * 31 + y;
			return (int) (v * 100f);
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
	public static vec3 v3(string x, string y, string z) {
		return new vec3(float.Parse(x), float.Parse(y), float.Parse(z));
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
		public vec2 xy {
			get { return new vec2(x, y); }
			set { this.x = value.x; this.y = value.y; }
		}
		public vec2 yz {
			get { return new vec2(y, z); }
			set { this.y = value.x; this.z = value.y; }
		}
		public vec2 xz {
			get { return new vec2(x, z); }
			set { this.x = value.x; this.z = value.y; }
		}
		public float this[int comp] {
			get { return new float[] { x, y, z }[comp]; }
			set { switch (comp) {
				case 0: x = value; break;
				case 1: y = value; break;
				case 2: z = value; break;
			} }
		}
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
			return new vec3(a.y * b.z - a.z * b.y, a.z * b.x - a.x * b.z, a.x * b.y - a.y * b.x);
		}
		// dot
		public static float operator ^(vec3 a, vec3 b) {
			return a.x * b.x + a.y * b.y + a.z * b.z;
		}
		public Color col() {
			return v4(this, 1f).col();
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
		public override string ToString() {
			return string.Format("v3({0},{1},{2})", x, y, z);
		}
		public override bool Equals(object obj) {
			vec3 o = obj as vec3;
			return o != null && x == o.x && y == o.y && z == o.z;
		}
		public override int GetHashCode() {
			float v = x;
			v = v * 31 + y;
			v = v * 31 + z;
			return (int) (v * 100f);
		}
	}
	public static vec4 col(Color c) {
		return new vec4(c.R / 255f, c.G / 255f, c.B / 255f, c.A / 255f);
	}
	public static vec4 v4() {
		return new vec4(0f, 0f, 0f, 0f);
	}
	public static vec4 v4(float v) {
		return new vec4(v, v, v, v);
	}
	public static vec4 v4(vec2 xy, float z, float w) {
		return new vec4(xy.x, xy.y, z, w);
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
		public vec3 xyz {
			get { return new vec3(x, y, z); }
			set { this.x = value.x; this.y = value.y; this.z = value.z; this.w = value.z; }
		}
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
		public Color col() {
			return Color.FromArgb((int) (255 * w),(int) (255f * x), (int) (255f * y), (int) (255f * z));
		}
		public override string ToString() {
			return string.Format("v4({0},{1},{2},{3})", x, y, z, w);
		}
		public override bool Equals(object obj) {
			vec4 o = obj as vec4;
			return o != null && x == o.x && y == o.y && z == o.z && w == o.w;
		}
		public override int GetHashCode() {
			float v = x;
			v = v * 31 + y;
			v = v * 31 + z;
			v = v * 31 + w;
			return (int) (v * 100f);
		}
	}
}
}
