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

	class Equation {
		public delegate float Eq(float t);

		public static List<Equation> all;

		static Equation() {
			all = new List<Equation>();
			all.Add(new Equation(0, eq_linear));
			all.Add(new Equation(3, eq_in_quad));
			all.Add(new Equation(4, eq_out_quad));
			all.Add(new Equation(5, eq_in_out_quad));
			all.Add(new Equation(6, eq_in_cubic));
			all.Add(new Equation(7, eq_out_cubic));
			all.Add(new Equation(8, eq_in_out_cubic));
			all.Add(new Equation(9, eq_in_quart));
			all.Add(new Equation(10, eq_out_quart));
			all.Add(new Equation(11, eq_in_out_quart));
			all.Add(new Equation(12, eq_in_quint));
			all.Add(new Equation(13, eq_out_quint));
			all.Add(new Equation(14, eq_in_out_quint));
			all.Add(new Equation(15, eq_in_sine));
			all.Add(new Equation(16, eq_out_sine));
			all.Add(new Equation(17, eq_in_out_sine));
			all.Add(new Equation(18, eq_in_expo));
			all.Add(new Equation(19, eq_out_expo));
			all.Add(new Equation(20, eq_in_out_expo));
			all.Add(new Equation(21, eq_in_circ));
			all.Add(new Equation(22, eq_out_circ));
			all.Add(new Equation(23, eq_in_out_circ));
		}

		public static Equation fromNumber(int number) {
			foreach (Equation e in all) {
				if (number == e.number) {
					return e;
				}
			}
			throw new Exception("boo!");
		}

		public static Equation fromEquation(Eq eq) {
			foreach (Equation e in all) {
				if (eq == e.calc) {
					return e;
				}
			}
			throw new Exception("boo!");
		}

		public readonly int number;
		public readonly Eq calc;
		public Equation(int number, Eq calc) {
			this.number = number;
			this.calc = calc;
		}
	}
}
}
