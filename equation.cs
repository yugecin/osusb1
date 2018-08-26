using System;
using System.Collections.Generic;
using System.Text;

namespace osusb1 {
partial class all {
	public static float eq_linear(float t) {
		return t;
	}
	public static float eq_in_quad(float t) {
		return t * t;
	}
	public static float eq_out_quad(float t) {
		return -1f * t * (t - 2f);
	}
	public static float eq_in_out_quad(float t) {
		t *= 2f;
		if (t < 1f) return .5f * t * t;
		t -= 1f;
		return -.5f * (t * (t - 2f) - 1f);
	}
	public static float eq_in_cubic(float t) {
		return t * t * t;
	}
	public static float eq_out_cubic(float t) {
		t -= 1f;
		return t * t * t + 1f;
	}
	public static float eq_in_out_cubic(float t) {
		t *= 2f;
		if (t < 1f) return .5f * t * t * t;
		t -= 2f;
		return .5f * (t * t * t + 2f);
	}
	public static float eq_in_quart(float t) {
		return t * t * t * t;
	}
	public static float eq_out_quart(float t) {
		t -= 1f;
		return -1f * (t * t * t * t - 1f);
	}
	public static float eq_in_out_quart(float t) {
		t *= 2f;
		if (t < 1f) return .5f * t * t * t * t;
		t -= 2f;
		return -.5f * (t * t * t * t - 2f);
	}
	public static float eq_in_quint(float t) {
		return t * t * t * t * t;
	}
	public static float eq_out_quint(float t) {
		t -= 1f;
		return t * t * t * t * t + 1f;
	}
	public static float eq_in_out_quint(float t) {
		t *= 2f;
		if (t < 1f) return .5f * t * t * t * t * t;
		t -= 2f;
		return .5f * (t * t * t * t * t + 2f);
	}
	public static float eq_in_sine(float t) {
		return -1f * cos(t * PI2) + 1f;
	}
	public static float eq_out_sine(float t) {
		return sin(t * PI2);
	}
	public static float eq_in_out_sine(float t) {
		return (cos(PI * t) - 1f) / -2f;
	}
	public static float eq_in_expo(float t) {
		return t == 0 ? 0 : pow(2, 10f * (t - 1));
	}
	public static float eq_out_expo(float t) {
		return t == 1 ? 1 : -pow(2, -10f * t) + 1;
	}
	public static float eq_in_out_expo(float t) {
		if (t == 0f || t == 1f) return t;
		t *= 2f;
		if (t < 1f) return .5f * pow(2, 10f * (t - 1));
		t -= 1f;
		return .5f * (-pow(2, -10f * t) + 2f);
	}
	public static float eq_in_circ(float t) {
		return -1f * (sqrt(1f - t * t) - 1f);
	}
	public static float eq_out_circ(float t) {
		t -= 1f;
		return sqrt(1f - t * t);
	}
	public static float eq_in_out_circ(float t) {
		t *= 2f;
		if (t < 1f) return -.5f * (sqrt(1f - t * t) - 1f);
		t -= 2f;
		return .5f * (sqrt(1f - t * t) + 1f);
	}
	private static vec2 cub_(float t, vec2 a, vec2 b){
		float ct=1f-t;
		return a*3f*ct*ct*t+b*3f*ct*t*t+t*t*t;
	}
	public static float eq_cub(float x, vec2 a, vec2 b){
		vec2 it=v2(0f,1f);
		for (int i=0;i<7;i++) {
			float pos=(it.x+it.y)/2f;
			vec2 r=cub_(pos,a,b);
			if (r.x>x){
				it.y=pos;
			}else{
				it.x=pos;
			}
		}
		return cub_((it.x+it.y)/2f,a,b).y;
	}

	public delegate float Eq(float t);

	private static List<Pair<int, Eq>> alleqs;

	public static void eq_init() {
		alleqs = new List<Pair<int, Eq>>();
		alleqs.Add(new Pair<int, Eq>(0, eq_linear));
		alleqs.Add(new Pair<int, Eq>(3, eq_in_quad));
		alleqs.Add(new Pair<int, Eq>(4, eq_out_quad));
		alleqs.Add(new Pair<int, Eq>(5, eq_in_out_quad));
		alleqs.Add(new Pair<int, Eq>(6, eq_in_cubic));
		alleqs.Add(new Pair<int, Eq>(7, eq_out_cubic));
		alleqs.Add(new Pair<int, Eq>(8, eq_in_out_cubic));
		alleqs.Add(new Pair<int, Eq>(9, eq_in_quart));
		alleqs.Add(new Pair<int, Eq>(10, eq_out_quart));
		alleqs.Add(new Pair<int, Eq>(11, eq_in_out_quart));
		alleqs.Add(new Pair<int, Eq>(12, eq_in_quint));
		alleqs.Add(new Pair<int, Eq>(13, eq_out_quint));
		alleqs.Add(new Pair<int, Eq>(14, eq_in_out_quint));
		alleqs.Add(new Pair<int, Eq>(15, eq_in_sine));
		alleqs.Add(new Pair<int, Eq>(16, eq_out_sine));
		alleqs.Add(new Pair<int, Eq>(17, eq_in_out_sine));
		alleqs.Add(new Pair<int, Eq>(18, eq_in_expo));
		alleqs.Add(new Pair<int, Eq>(19, eq_out_expo));
		alleqs.Add(new Pair<int, Eq>(20, eq_in_out_expo));
		alleqs.Add(new Pair<int, Eq>(21, eq_in_circ));
		alleqs.Add(new Pair<int, Eq>(22, eq_out_circ));
		alleqs.Add(new Pair<int, Eq>(23, eq_in_out_circ));
	}

	public static Eq num2eq(int number) {
		foreach (var e in alleqs) {
			if (number == e.a) {
				return e.b;
			}
		}
		throw new Exception("boo!");
	}

	public static int eq2num(Eq eq) {
		foreach (var e in alleqs) {
			if (eq == e.b) {
				return e.a;
			}
		}
		throw new Exception("boo!");
	}
}
}
