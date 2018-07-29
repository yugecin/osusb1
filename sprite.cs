﻿using System;
using System.Collections.Generic;
using System.Text;

namespace osusb1 {
partial class all {
	class Sprite {

		public static Dictionary<string, int> usagedata = new Dictionary<string,int>();

		vec2 initialpos;
		string filename;
		float spritesize, scalemod;

		LinkedList<Frame> frames = new LinkedList<Frame>();	
		Frame currentframe;

		public Sprite(string filename, float spritesize, float scalemod) {
			this.filename = filename;
			this.spritesize = spritesize;
			this.scalemod = scalemod;
		}

		public static Sprite dot6_12() {
			return new Sprite("d", 12f, 6f);
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

			List<LinkedList<Frame>> batches = new List<LinkedList<Frame>>();

			LinkedList<Frame> currentbatch = null;

			foreach (Frame frame in frames) {
				frame.actualfade = frame.fade * frame.col.w;
				if (isoob(frame.pos, frame.scale)) {
					frame.hidden = true;
				}
				if (frame.hidden) {
					currentbatch = null;
					continue;
				}
				if (currentbatch == null) {
					currentbatch = new LinkedList<Frame>();
					batches.Add(currentbatch);
				}
				currentbatch.AddLast(frame);
			}

			foreach (LinkedList<Frame> f in batches) {
				LinkedListNode<Frame> _cf = f.First;

				Frame cf = _cf.Value;
				Frame firstframe = cf;
				updateinitialpos(_cf, firstframe.pos);
				Frame lf = new Frame(0, null);
				lf.scale = 1f;
				lf.fade = 1f;
				lf.actualfade = 1f;
				lf.col = v4(1f);
				lf.pos = cf.pos;
				Frame initialframe = lf;

				bool hassprite = false;

				MoveCommand lastmove = null;
				FadeCommand lastfade = null;
				ScaleCommand lastscale = null;
				ColorCommand lastcolor = null;

				while (_cf != null) {
					cf = _cf.Value;

					if (_cf.Next == null) {
						break;
					}
					Frame nf = _cf.Next.Value;
					/*
					if (MoveCommand.requiresupdate(cf.pos, lf.pos)) {
						hassprite = write(w, hassprite, new MoveCommand(cf.time, nf.time, cf.pos, nf.pos));
					}
					if (FadeCommand.requiresupdate(cf.actualfade, lf.actualfade)) {
						hassprite = write(w, hassprite, new FadeCommand(cf.time, nf.time, cf.actualfade, nf.actualfade));
					}
					if (ScaleCommand.requiresupdate(cf.scale, lf.scale)) {
						hassprite = write(w, hassprite, new ScaleCommand(cf.time, nf.time, cf.scale, nf.scale));
					}
					if (ColorCommand.requiresupdate(cf.col, lf.col)) {
						hassprite = write(w, hassprite, new ColorCommand(cf.time, nf.time, cf.col.xyz, nf.col.xyz));
					}
					*/
					if (MoveCommand.requiresupdate(cf.pos, lf.pos)) {
						hassprite = write(w, hassprite, new MoveCommand(cf.time, cf.time, cf.pos, cf.pos));
					}
					if (FadeCommand.requiresupdate(cf.actualfade, lf.actualfade)) {
						hassprite = write(w, hassprite, new FadeCommand(cf.time, cf.time, cf.actualfade, cf.actualfade));
					}
					if (ScaleCommand.requiresupdate(cf.scale, lf.scale)) {
						hassprite = write(w, hassprite, new ScaleCommand(cf.time, cf.time, cf.scale, cf.scale));
					}
					if (ColorCommand.requiresupdate(cf.col, lf.col)) {
						hassprite = write(w, hassprite, new ColorCommand(cf.time, cf.time, cf.col.xyz, cf.col.xyz));
					}

					lf = cf;
					_cf = _cf.Next;
				}

				if (!hassprite) {
					continue;
				}

				FadeCommand _f = new FadeCommand(cf.time, cf.time, lf.actualfade, 0f);
				if (lastfade == null) {
					_f.from = lf.actualfade;
				}

				int extendcost = _f.cost();
				int chosencmd = -1;

				ICommand[] cmds = { lastmove, lastfade, lastscale, lastcolor };
				ICommand combinedcmd = null;
				int[] len = new int[cmds.Length];
				for (int i = 0; i < cmds.Length; i++) {
					if (cmds[i] != null) {
						ICommand ccmd = cmds[i].extend(cf.time);
						if (ccmd == null) {
							continue;
						}
						len[i] = ccmd.cost() - cmds[i].cost();
						if (len[i] < extendcost) {
							extendcost = len[i];
							chosencmd = i;
							combinedcmd = ccmd;
						}
					}
				}

				lastmove = null;
				lastfade = null;
				lastscale = null;
				lastcolor = null;

				if (chosencmd != -1) {
					cmds[chosencmd] = combinedcmd;
				}

				foreach (ICommand c in cmds) {
					hassprite = write(w, hassprite, c);
				}

				if (chosencmd == -1) {
					write(w, hassprite, _f);
				}
			}

		}

		/*
		private void updateinitialpos(LinkedListNode<Frame> _cf, vec2 firstpos) {
			while (_cf != null) {
				if (_cf.Value.hidden || isoob(_cf.Value.pos, _cf.Value.scale)) {
					break;
				}
				if (_cf.Value.pos != firstpos) {
					firstpos = v2(0f);
					break;
				}
				_cf = _cf.Next;
			}
			initialpos = firstpos;
		}
		*/

		// set initial pos to 0,0 if one or more move commands are needed
		private void updateinitialpos(LinkedListNode<Frame> _cf, vec2 firstpos) {
			while (_cf != null) {
				if (_cf.Value.pos != firstpos) {
					firstpos = v2(0f);
					break;
				}
				_cf = _cf.Next;
			}
			initialpos = firstpos;
		}

		private LinkedListNode<Frame> nextvisibleframe(LinkedListNode<Frame> _cf) {
			while (_cf != null) {
				if (!_cf.Value.hidden && !isoob(_cf.Value.pos, _cf.Value.scale)) {
					return _cf;
				}
				_cf = _cf.Next;
			}
			return null;
		}

		private bool isoob(vec2 pos, float scale) {
			float oost = spritesize * scale; // out-of-screen-threshold
			return pos.x < -oost || 640f + oost < pos.x ||
				pos.y < -oost || 480 + oost < pos.y;
		}

		private string createsprite(vec2 pos) {
			return "4,3,1," + filename + "," + round(pos.x) + "," + round(pos.y);
		}

		private bool write(Writer w, bool hassprite, params object[] commands) {
			foreach (object c in commands) {
				hassprite = write(w, hassprite, c);
			}
			return hassprite;
		}

		private bool write(Writer w, bool hassprite, object command) {
			if (command == null) {
			     return hassprite;
			}
			if (!hassprite) {
				w.ln(createsprite(initialpos));
				addusagedata();
			}
			w.ln(command.ToString());
			return true;
		}

		private void addusagedata() {
			int count = 1;
			if (usagedata.ContainsKey(filename)) {
				count += usagedata[filename];
				usagedata.Remove(filename);
			}
			usagedata.Add(filename, count);
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

		interface ICommand {
			ICommand extend(int time);
			int cost();
		}

		class MoveCommand : ICommand {
			public int start, end;
			public vec2 from, to;
			public MoveCommand(int start, int end, vec2 from, vec2 to) {
				this.start = start;
				this.end = end;
				this.from = from;
				this.to = to;
			}
			public MoveCommand(Frame f): this(f.time, f.time, v2(0f), f.pos) {}
			ICommand ICommand.extend(int time) {
				if (from != v2(0f)) {
					return null;
				}
				return new MoveCommand(start, time, to, to);
			}
			public int cost() {
				return ToString().Length + 1;
			}
			public static bool requiresupdate(vec2 prev, vec2 current) {
				return roundm(prev.x) != roundm(current.x) || roundm(prev.y) != roundm(current.y);
			}
			public override string ToString() {
				string _to = string.Format(
					",{0},{1}",
					roundm(to.x),
					roundm(to.y)
				);
				return string.Format(
					"_M,0,{0},{1},{2},{3}{4}",
					start,
					endtime(start, end),
					roundm(from.x),
					roundm(from.y),
					to.Equals(from) ? "" : _to
				);
			}
		}

		class ColorCommand : ICommand {
			public int start, end;
			public vec3 from, to;
			public ColorCommand(int start, int end, vec3 from, vec3 to) {
				this.start = start;
				this.end = end;
				this.from = from;
				this.to = to;
			}
			public ColorCommand(Frame f): this(f.time, f.time, v3(0f), f.col.xyz) {}
			ICommand ICommand.extend(int time) {
				if (from != v3(0f)) {
					return null;
				}
				return new ColorCommand(start, time, to, to);
			}
			public int cost() {
				return ToString().Length + 1;
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
			public int start, end;
			public float from, to;
			public FadeCommand(int start, int end, float from, float to) {
				this.start = start;
				this.end = end;
				this.from = from;
				this.to = to;
			}
			public FadeCommand(Frame f): this(f.time, f.time, 0f, f.actualfade) {}
			ICommand ICommand.extend(int time) {
				if (from != 0f) {
					return null;
				}
				return new FadeCommand(start, time, to, to);
			}
			public int cost() {
				return ToString().Length + 1;
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
		}

		class ScaleCommand : ICommand {
			public int start, end;
			public float from, to;
			public ScaleCommand(int start, int end, float from, float to) {
				this.start = start;
				this.end = end;
				this.from = from;
				this.to = to;
			}
			public ScaleCommand(Frame f): this(f.time, f.time, 0f, f.scale) {}
			ICommand ICommand.extend(int time) {
				if (from != 0f) {
					return null;
				}
				return new ScaleCommand(start, time, to, to);
			}
			public int cost() {
				return ToString().Length + 1;
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
		}

		public static string round(float val) {
			return Math.Round(val, 1).ToString();
		}

		public static string roundm(float val) {
			return round(val);
			//return ((int) val).ToString();
		}

		public static string endtime(int start, int end) {
			return start == end ? "" : end.ToString();
		}
	}
}
}
