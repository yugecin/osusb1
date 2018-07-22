using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace osusb1 {
partial class all {
	class Sprite {

		vec2 initialpos;
		string createcommand;
		float spritesize, scalemod;

		LinkedList<Frame> frames = new LinkedList<Frame>();	
		Frame currentframe;

		public Sprite(string filename, float spritesize, float scalemod, vec2 pos) {
			this.spritesize = spritesize;
			this.scalemod = scalemod;
			initialpos = pos;
			createcommand = "4,3,1," + filename + "," + (int) pos.x + "," + (int) pos.y;
		}

		public static Sprite dot6_12(vec2 pos) {
			return new Sprite("d", 12f, 6f, pos);
		}

		public void startframe(int time) {
			currentframe = new Frame(time, currentframe);
			frames.AddLast(currentframe);
		}

		public void hide() {
			currentframe.hidden = true;
		}

		public void move(vec2 pos) {
			currentframe.pos = pos;
		}

		public void color(vec4 col) {
			currentframe.col = col;
		}

		public void size(float size) {
			currentframe.scale = size / scalemod;
		}

		/*
		public void scale(float scale) {
			currentframe.scale = scale;
		}
		*/

		public void fin(Writer w) {
			if (frames.Count == 0) {
				return;
			}

			LinkedListNode<Frame> _cf = frames.First;

			Frame lf = new Frame(0, null);
			lf.scale = 1f;
			lf.fade = 1f;
			lf.actualfade = 1f;
			lf.col = v4(1f);
			lf.pos = initialpos;
			Frame initialframe = lf;
			Frame cf = _cf.Value;

			bool hassprite = false;

			MoveCommand lastmove = null;
			FadeCommand lastfade = null;
			ScaleCommand lastscale = null;
			ColorCommand lastcolor = null;

			while (_cf != null) {
				cf = _cf.Value;
				/*
				if frame hidden:
				  modify one of the last command to make it extend to current time
				  > find the lowest cost (or introduce new fade command)
				*/

				hassprite = write(w, hassprite, lastmove);
				hassprite = write(w, hassprite, lastfade);
				hassprite = write(w, hassprite, lastscale);
				hassprite = write(w, hassprite, lastcolor);

				float oost = spritesize * cf.scale; // out-of-screen-threshold
				if (cf.pos.x < -oost || 640f + oost < cf.pos.x ||
					cf.pos.y < -oost || 480 + oost < cf.pos.y) {
					cf.hidden = true;
				}

				lastmove = null;
				lastfade = null;
				lastscale = null;
				lastcolor = null;
				if (cf.hidden) {
				} else {
					cf.actualfade = cf.fade * cf.col.w;

					if (MoveCommand.requiresupdate(cf.pos, lf.pos)) {
						lastmove = new MoveCommand(cf);
					}
					if (FadeCommand.requiresupdate(cf.actualfade, lf.actualfade)) {
						lastfade = new FadeCommand(cf);
					}
					if (ScaleCommand.requiresupdate(cf.scale, lf.scale)) {
						lastscale = new ScaleCommand(cf);
					}
					if (ColorCommand.requiresupdate(cf.col.xyz, lf.col.xyz)) {
						lastcolor = new ColorCommand(cf);
					}
					lf = cf;
				}

				//lf = cf;
				_cf = _cf.Next;
			}
		}

		private bool write(Writer w, bool hassprite, object command) {
			if (command == null) {
			     return hassprite;
			}
			if (!hassprite) {
				w.ln(createcommand);
			}
			w.ln(command.ToString());
			return true;
		}

		class Frame {
			public int time;
			public vec2 pos;
			public vec4 col;
			public float fade;
			public float scale;
			public float actualfade;
			public bool hidden;
			public Frame(int time, Frame prev) {
				this.time = time;
				if (prev == null) {
					fade = 1f;
					scale = 1f;
					return;
				}
				pos = prev.pos;
				col = prev.col;
				fade = prev.fade;
				scale = prev.scale;
			}
		}

		class MoveCommand {
			public int start, end;
			public vec2 from, to;
			public MoveCommand(int start, int end, vec2 from, vec2 to) {
				this.start = start;
				this.end = end;
				this.from = from;
				this.to = to;
			}
			public MoveCommand(Frame f): this(f.time, f.time, v2(0f), f.pos) {}
			public static bool requiresupdate(vec2 prev, vec2 current) {
				return round(prev.x) != round(current.x) || round(prev.y) != round(current.y);
			}
			public override string ToString() {
				return string.Format(
					"_M,0,{0},{1},{2},{3},{4},{5}",
					start,
					endtime(start, end),
					round(from.x),
					round(from.y),
					round(to.x),
					round(to.y)
				);
			}
		}

		class ColorCommand {
			public int start, end;
			public vec3 from, to;
			public ColorCommand(int start, int end, vec3 from, vec3 to) {
				this.start = start;
				this.end = end;
				this.from = from;
				this.to = to;
			}
			public ColorCommand(Frame f): this(f.time, f.time, v3(0f), f.col.xyz) {}
			public static bool requiresupdate(vec3 prev, vec3 current) {
				return !v4(prev, 1f).col().Equals(v4(current, 1f).col());
			}
			public override string ToString() {
				return string.Format(
					"_C,0,{0},{1},{2},{3},{4},{5},{6},{7}",
					start,
					endtime(start, end),
					(int) (255f * from.x),
					(int) (255f * from.y),
					(int) (255f * from.z),
					(int) (255f * to.x),
					(int) (255f * to.y),
					(int) (255f * to.z)
				);
			}
		}

		class FadeCommand {
			public int start, end;
			public float from, to;
			public FadeCommand(int start, int end, float from, float to) {
				this.start = start;
				this.end = end;
				this.from = from;
				this.to = to;
			}
			public FadeCommand(Frame f): this(f.time, f.time, 0f, f.actualfade) {}
			public static bool requiresupdate(float prev, float current) {
				return round(prev) != round(current);
			}
			public override string ToString() {
				return string.Format(
					"_F,0,{0},{1},{2},{3}",
					start,
					endtime(start, end),
					round(from),
					round(to)
				);
			}
		}

		class ScaleCommand {
			public int start, end;
			public float from, to;
			public ScaleCommand(int start, int end, float from, float to) {
				this.start = start;
				this.end = end;
				this.from = from;
				this.to = to;
			}
			public ScaleCommand(Frame f): this(f.time, f.time, 0f, f.scale) {}
			public static bool requiresupdate(float prev, float current) {
				return round(prev) != round(current);
			}
			public override string ToString() {
				return string.Format(
					"_S,0,{0},{1},{2},{3}",
					start,
					endtime(start, end),
					round(from),
					round(to)
				);
			}
		}

		public static string round(float val) {
			return Math.Round(val, 1).ToString();
		}

		public static string endtime(int start, int end) {
			return start == end ? "" : end.ToString();
		}
	}
}
}
