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

			LinkedListNode<Frame> _cf = frames.First;
			// drop the last frame so it will be combined with the one-but-last frame
			frames.Last.Value.hidden = true;

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
			bool wasvisiblelastframe = false;

			MoveCommand lastmove = null;
			FadeCommand lastfade = null;
			ScaleCommand lastscale = null;
			ColorCommand lastcolor = null;

			while (_cf != null) {
				cf = _cf.Value;
				bool islastframe = _cf.Next == null;

				/*
				if frame hidden:
				  modify one of the last command to make it extend to current time
				  > find the lowest cost (or introduce new fade command)
				*/

				if (isoob(cf.pos, cf.scale)) {
					cf.hidden = true;
				}

				if (cf.hidden) {
					if (wasvisiblelastframe) {
						// it's hidden now but last frame was visible, this means last frame
						// wasn't shown because that needs a frame after it for it to be visible,
						// thus, extend a previous command to include current time or add a fade
						// command with current time so the previous frame gets drawn.

						wasvisiblelastframe = false;
						if (!hassprite) {
							goto next;
						}

						FadeCommand _f = new FadeCommand(cf.time, cf.time, 0f, 0f);
						if (lastfade == null) {
							_f.from = 1f;
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

						bool shouldrecreate = false;
						LinkedListNode<Frame> _nv = nextvisibleframe(_cf.Next);
						if (_nv != null) {
							Frame nv = _nv.Value;
							// calculate TOTAL cost to go from previous frame to
							// next frame using both methods (fade & recreate)

							int fadecost = 0;
							fadecost += new FadeCommand(cf.time, cf.time, 0f, 0f).cost();
							if (nv.actualfade == 1f) {
								fadecost += new FadeCommand(nv).cost();
							} else {
								fadecost += new FadeCommand(nv.time, nv.time, 1f, 1f).cost();
							}
							if (MoveCommand.requiresupdate(nv.pos, lf.pos)) {
								fadecost += new MoveCommand(nv).cost();
							}
							if (ScaleCommand.requiresupdate(nv.scale, lf.scale)) {
								fadecost += new ScaleCommand(nv).cost();
							}
							if (ColorCommand.requiresupdate(nv.col, lf.col)) {
								fadecost += new ColorCommand(nv).cost();
							}

							int recreatecost = 0;
							recreatecost += extendcost;
							recreatecost += createsprite(nv.pos).Length + 1;
							// no need to check for move, coordinates are given when sprite is made
							if (ColorCommand.requiresupdate(initialframe.col, nv.col)) {
								recreatecost += new ColorCommand(nv).cost();
							}
							if (ScaleCommand.requiresupdate(initialframe.scale, nv.scale)) {
								recreatecost += new ScaleCommand(nv).cost();
							}
							if (FadeCommand.requiresupdate(initialframe.actualfade, nv.actualfade)) {
								recreatecost += new FadeCommand(nv).cost();
							}

							if (recreatecost <= fadecost || true) {
								lf = initialframe;
								initialframe.pos = nv.pos;
								initialpos = nv.pos;
								shouldrecreate = true;
								firstframe = nv;
								updateinitialpos(_nv, firstframe.pos);
							} else {
								chosencmd = -1;
								lf.actualfade = 0f;
							}
						}

						if (chosencmd != -1) {
							cmds[chosencmd] = combinedcmd;
						}

						foreach (ICommand c in cmds) {
							hassprite = write(w, hassprite, c);
						}

						if (chosencmd == -1) {
							write(w, hassprite, _f);
							lf.fade = 0f;
						}

						if (shouldrecreate) {
							hassprite = false;
						}

						if (islastframe) {
							break;
						}
					}
					goto next;
				}

				wasvisiblelastframe = true;
				cf.actualfade = cf.fade * cf.col.w;

				if (MoveCommand.requiresupdate(cf.pos, lf.pos)) {
					bool hadmove = lastmove != null;
					if (hadmove) {
						hassprite = write(w, hassprite, lastmove);
					}
					lastmove = new MoveCommand(cf);
					if (!hadmove) {
						lastmove.from = firstframe.pos;
					}
				}
				if (FadeCommand.requiresupdate(cf.actualfade, lf.actualfade)) {
					bool hadfade = lastfade != null;
					if (hadfade) {
						hassprite = write(w, hassprite, lastfade);
					}
					lastfade = new FadeCommand(cf);
					if (!hadfade) {
						lastfade.from = firstframe.actualfade;
					}
				}
				if (ScaleCommand.requiresupdate(cf.scale, lf.scale)) {
					bool hadscale = lastscale != null;
					if (hadscale) {
						hassprite = write(w, hassprite, lastscale);
					}
					lastscale = new ScaleCommand(cf);
					if (!hadscale) {
						lastscale.from = firstframe.scale;
					}
				}
				if (ColorCommand.requiresupdate(cf.col, lf.col)) {
					bool hadcolor = lastcolor != null;
					if (hadcolor) {
						hassprite = write(w, hassprite, lastcolor);
					}
					lastcolor = new ColorCommand(cf);
					if (!hadcolor) {
						lastcolor.from = firstframe.col.xyz;
					}
				}
				lf = cf;

next:

				//lf = cf;
				_cf = _cf.Next;
			}
		}

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
				return string.Format(
					"_F,0,{0},{1},{2},{3}",
					start,
					endtime(start, end),
					round(from),
					round(to)
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
