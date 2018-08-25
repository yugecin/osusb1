using System;
using System.Collections.Generic;
using System.Text;

namespace osusb1 {
partial class all {

	abstract class ICommand {
		public static Stack<int> round_rot_decimals = new Stack<int>();
		public static Stack<int> round_move_decimals = new Stack<int>();
		public static Stack<int> round_fade_decimals = new Stack<int>();
		public static Stack<int> round_scale_decimals = new Stack<int>();
		public static Stack<bool> allow_mx_my = new Stack<bool>();

		static ICommand() {
			round_rot_decimals.Push(5);
			round_move_decimals.Push(1);
			round_fade_decimals.Push(1);
			round_scale_decimals.Push(5);
			allow_mx_my.Push(false);
		}

		public int easing;
		public int start, end;
		public bool isPhantom = isPhantomFrame;

		public abstract object From { get; set; }
		public abstract object To { get; set; }

		public abstract ICommand extend(int time);
		public ICommand copy() {
			ICommand c = copyImpl();
			c.easing = easing;
			c.isPhantom = isPhantom;
			return c;
		}
		protected abstract ICommand copyImpl();
		public int cost() {
			return ToString().Length + 1;
		}
		protected string endtime(int start, int end) {
			return start == end ? "" : end.ToString();
		}
	}

	class RotCommand : ICommand {
		public float from, to;
		public override object From { get { return from; } set { from = (float) value; } }
		public override object To { get { return to; } set { to = (float) value; } }
		public RotCommand(int start, int end, float from, float to) {
			this.start = start;
			this.end = end;
			this.from = from;
			this.to = to;
		}
		public override ICommand extend(int time) {
			if (from != 0f) {
				return null;
			}
			return new RotCommand(start, time, to, to);
		}
		protected override ICommand copyImpl() {
			return new RotCommand(start, end, from, to);
		}
		public static bool requiresUpdate(float prev, float current) {
			return round(prev) != round(current);
		}
		public override string ToString() {
			string _to = string.Format(
				",{0}",
				to
			);
			return string.Format(
				"_R,{0},{1},{2},{3}{4}",
				easing,
				start,
				endtime(start, end),
				round(from),
				to.Equals(from) ? "" : _to
			);
		}
		public static string round(float val) {
			return Math.Round(val, ICommand.round_rot_decimals.Peek()).ToString();
		}
	}

	class MoveCommand : ICommand {
		public vec2 from, to;
		public override object From { get { return from; } set { from = (vec2) value; } }
		public override object To { get { return to; } set { to = (vec2) value; } }
		public MoveCommand(int start, int end, vec2 from, vec2 to) {
			this.start = start;
			this.end = end;
			this.from = from;
			this.to = to;
		}
		public override ICommand extend(int time) {
			if (from != v2(0f)) {
				return null;
			}
			return new MoveCommand(start, time, to, to);
		}
		protected override ICommand copyImpl() {
			return new MoveCommand(start, end, v2(from), v2(to));
		}
		public static bool requiresUpdate(vec2 prev, vec2 current) {
			return round(prev.x) != round(current.x) || round(prev.y) != round(current.y);
		}
		public override string ToString() {
			bool xs = round(to.x) == round(from.x);
			bool ys = round(to.y) == round(from.y);
			bool s = xs && ys;
			if (allow_mx_my.Peek()) {
				if (!s && xs) {
					return string.Format(
						"_MY,{0},{1},{2},{3},{4}",
						easing,
						start,
						endtime(start, end),
						round(from.y),
						round(to.y)
					);
				}
				if (!s && ys) {
					return string.Format(
						"_MX,{0},{1},{2},{3},{4}",
						easing,
						start,
						endtime(start, end),
						round(from.x),
						round(to.x)
					);
				}
			}
			string _to = string.Format(
				",{0},{1}",
				round(to.x),
				round(to.y)
			);
			return string.Format(
				"_M,{0},{1},{2},{3},{4}{5}",
				easing,
				start,
				endtime(start, end),
				round(from.x),
				round(from.y),
				s ? "" : _to
			);
		}
		public static string round(float val) {
			return Math.Round(val, ICommand.round_move_decimals.Peek()).ToString();
		}
	}

	class ColorCommand : ICommand {
		public vec3 from, to;
		public override object From { get { return from; } set { from = (vec3) value; } }
		public override object To { get { return to; } set { to = (vec3) value; } }
		public ColorCommand(int start, int end, vec3 from, vec3 to) {
			this.start = start;
			this.end = end;
			this.from = from;
			this.to = to;
		}
		public override ICommand extend(int time) {
			if (from != v3(0f)) {
				return null;
			}
			return new ColorCommand(start, time, to, to);
		}
		protected override ICommand copyImpl() {
			return new ColorCommand(start, end, v3(from), v3(to));
		}
		public static bool requiresUpdate(vec3 prev, vec3 current) {
			return !v4(prev, 1f).col().Equals(v4(current, 1f).col());
		}
		public override string ToString() {
			string _to = string.Format(
				",{0},{1},{2}",
				(int) (255f * to.x),
				(int) (255f * to.y),
				(int) (255f * to.z)
			);
			return string.Format(
				"_C,{0},{1},{2},{3},{4},{5}{6}",
				easing,
				start,
				endtime(start, end),
				(int) (255f * from.x),
				(int) (255f * from.y),
				(int) (255f * from.z),
				from.Equals(to) ? "" : _to
			);
		}
	}

	class FadeCommand : ICommand {
		public float from, to;
		public override object From { get { return from; } set { from = (float) value; } }
		public override object To { get { return to; } set { to = (float) value; } }
		public FadeCommand(int start, int end, float from, float to) {
			this.start = start;
			this.end = end;
			this.from = from;
			this.to = to;
		}
		public override ICommand extend(int time) {
			if (from != 0f) {
				return null;
			}
			return new FadeCommand(start, time, to, to);
		}
		protected override ICommand copyImpl() {
			return new FadeCommand(start, end, from, to);
		}
		public static bool requiresUpdate(float prev, float current) {
			return round(prev) != round(current);
		}
		public override string ToString() {
			string _to = string.Format(
				",{0}",
				round(to)
			);
			return string.Format(
				"_F,{0},{1},{2},{3}{4}",
				easing,
				start,
				endtime(start, end),
				round(from),
				from.Equals(to) ? "" : _to
			);
		}
		public static string round(float val) {
			return Math.Round(val, ICommand.round_fade_decimals.Peek()).ToString();
		}
	}

	class ScaleCommand : ICommand {
		public float from, to;
		public override object From { get { return from; } set { from = (float) value; } }
		public override object To { get { return to; } set { to = (float) value; } }
		public ScaleCommand(int start, int end, float from, float to) {
			this.start = start;
			this.end = end;
			this.from = from;
			this.to = to;
		}
		public override ICommand extend(int time) {
			if (from != 0f) {
				return null;
			}
			return new ScaleCommand(start, time, to, to);
		}
		protected override ICommand copyImpl() {
			return new ScaleCommand(start, end, from, to);
		}
		public static bool requiresUpdate(float prev, float current) {
			return round(prev) != round(current);
		}
		public override string ToString() {
			string _to = string.Format(
				",{0}",
				round(to)
			);
			return string.Format(
				"_S,{0},{1},{2},{3}{4}",
				easing,
				start,
				endtime(start, end),
				round(from),
				from.Equals(to) ? "" : _to
			);
		}
		public static string round(float val) {
			return Math.Round(val, ICommand.round_scale_decimals.Peek()).ToString();
		}
	}

	class VScaleCommand : ICommand {
		public vec2 from, to;
		public override object From { get { return from; } set { from = (vec2) value; } }
		public override object To { get { return to; } set { to = (vec2) value; } }
		public VScaleCommand(int start, int end, vec2 from, vec2 to) {
			this.start = start;
			this.end = end;
			this.from = from;
			this.to = to;
		}
		public override ICommand extend(int time) {
			if (from != v2(0f)) {
				return null;
			}
			return new VScaleCommand(start, time, to, to);
		}
		protected override ICommand copyImpl() {
			return new VScaleCommand(start, end, v2(from), v2(to));
		}
		public static bool requiresUpdate(vec2 prev, vec2 current) {
			return round(prev.x) != round(current.x) || round(prev.y) != round(current.y);
		}
		public override string ToString() {
			string _to = string.Format(
				",{0},{1}",
				round(to.x),
				round(to.y)
			);
			return string.Format(
				"_V,{0},{1},{2},{3},{4}{5}",
				easing,
				start,
				endtime(start, end),
				round(from.x),
				round(from.y),
				from.Equals(to) ? "" : _to
			);
		}
		public static string round(float val) {
			return Math.Round(val, ICommand.round_scale_decimals.Peek()).ToString();
		}
	}
}
}
