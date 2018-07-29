using System;
using System.Collections.Generic;
using System.Text;

namespace osusb1 {
partial class all {

	abstract class ICommand {
		public static int round_move_decimals = 1;
		public static int round_fade_decimals = 1;
		public static int round_scale_decimals = 1;

		public int start, end;

		public abstract object From { get; }
		public abstract object To { get; }

		public abstract ICommand extend(int time);
		public int cost() {
			return ToString().Length + 1;
		}
		protected string endtime(int start, int end) {
			return start == end ? "" : end.ToString();
		}
	}

	class MoveCommand : ICommand {
		public vec2 from, to;
		public override object From { get { return from; } }
		public override object To { get { return to; } }
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
		public static bool requiresupdate(vec2 prev, vec2 current) {
			return round(prev.x) != round(current.x) || round(prev.y) != round(current.y);
		}
		public override string ToString() {
			string _to = string.Format(
				",{0},{1}",
				round(to.x),
				round(to.y)
			);
			return string.Format(
				"_M,0,{0},{1},{2},{3}{4}",
				start,
				endtime(start, end),
				round(from.x),
				round(from.y),
				to.Equals(from) ? "" : _to
			);
		}
		public static string round(float val) {
			return Math.Round(val, ICommand.round_move_decimals).ToString();
		}
	}

	class ColorCommand : ICommand {
		public vec3 from, to;
		public override object From { get { return from; } }
		public override object To { get { return to; } }
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
		public static bool requiresupdate(vec4 prev, vec4 current) {
			return !v4(prev.xyz, 1f).col().Equals(v4(current.xyz, 1f).col());
		}
		public override string ToString() {
			string _to = string.Format(
				",{0},{1},{2}",
				(int) (255f * to.x),
				(int) (255f * to.y),
				(int) (255f * to.z)
			);
			return string.Format(
				"_C,0,{0},{1},{2},{3},{4}{5}",
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
		public override object From { get { return from; } }
		public override object To { get { return to; } }
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
		public static bool requiresupdate(float prev, float current) {
			return round(prev) != round(current);
		}
		public override string ToString() {
			string _to = string.Format(
				",{0}",
				round(to)
			);
			return string.Format(
				"_F,0,{0},{1},{2}{3}",
				start,
				endtime(start, end),
				round(from),
				from.Equals(to) ? "" : _to
			);
		}
		public static string round(float val) {
			return Math.Round(val, ICommand.round_fade_decimals).ToString();
		}
	}

	class ScaleCommand : ICommand {
		public float from, to;
		public override object From { get { return from; } }
		public override object To { get { return to; } }
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
		public static bool requiresupdate(float prev, float current) {
			return round(prev) != round(current);
		}
		public override string ToString() {
			string _to = string.Format(
				",{0}",
				round(to)
			);
			return string.Format(
				"_S,0,{0},{1},{2}{3}",
				start,
				endtime(start, end),
				round(from),
				from.Equals(to) ? "" : _to
			);
		}
		public static string round(float val) {
			return Math.Round(val, ICommand.round_scale_decimals).ToString();
		}
	}
}
}
