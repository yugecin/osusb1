using System;
using System.Collections.Generic;
using System.Text;

namespace osusb1 {
partial class all {
	class Sprite {

		private struct SDATA {
			public float size;
			public float scalemod;
			public SDATA(float size, float scalemod) {
				this.size = size;
				this.scalemod = scalemod;
			}
		}

		public const int INTERPOLATE_MOVE = 0x1;
		public const int ORIGIN_BOTTOMLEFT = 0x2;
		public const int EASE_FADE = 0x4;
		public const int EASE_SCALE = 0x8;
		public const int NO_ADJUST_LAST = 0x10;
		public const string SPRITE_DOT_6_12 = "d";
		public const string SPRITE_TRI = "t";
		public const string SPRITE_SQUARE_1_1 = "1";
		public const string SPRITE_SQUARE_2_2 = "2";
		public const string SPRITE_SQUARE_3_3 = "3";
		public const string SPRITE_SQUARE_6_6 = "";
		public const string SPRITE_PIXEL = "s";

		public static Dictionary<string, int> usagedata = new Dictionary<string,int>();
		public static int easeCommandsSaved = 0;
		public static int easeResultSuccess = 0;
		public static int easeResultFailed = 0;
		public static int framedelta; // ew

		private static Dictionary<string, SDATA> spritedata = new Dictionary<string,SDATA>();

		static Sprite() {
			spritedata.Add(SPRITE_SQUARE_6_6, new SDATA(6f, 6f));
			spritedata.Add(SPRITE_SQUARE_1_1, new SDATA(1f, 1f));
			spritedata.Add(SPRITE_SQUARE_2_2, new SDATA(2f, 2f));
			spritedata.Add(SPRITE_SQUARE_3_3, new SDATA(3f, 3f));
			spritedata.Add(SPRITE_DOT_6_12, new SDATA(12f, 6f));
			spritedata.Add(SPRITE_TRI, new SDATA(600f, 600f));
			spritedata.Add(SPRITE_PIXEL, new SDATA(1f, 1f));
		}

		public static float Size(string filename) {
			return spritedata[filename].size;
		}

		LinkedList<ICommand> allcmds = new LinkedList<ICommand>();
		LinkedList<RotCommand> rotcmds = new LinkedList<RotCommand>();
		public
		LinkedList<MoveCommand> movecmds = new LinkedList<MoveCommand>();
		public
		LinkedList<FadeCommand> fadecmds = new LinkedList<FadeCommand>();
		public
		LinkedList<ColorCommand> colorcmds = new LinkedList<ColorCommand>();
		LinkedList<ScaleCommand> scalecmds = new LinkedList<ScaleCommand>();
		LinkedList<VScaleCommand> vscalecmds = new LinkedList<VScaleCommand>();

		List<ICommand> overrides = new List<ICommand>();

		List<string> raws = new List<string>();

		string filename;
		SDATA sdata;
		int settings;
		public int starttime, endtime;

		public Sprite(string filename, int settings) {
			this.filename = filename;
			this.settings = settings;
			this.sdata = spritedata[filename];
			starttime = -1;
		}

		public void addRaw(string raw) {
			raws.Add(raw);
		}

		public void addOverride(ICommand cmd) {
			this.overrides.Add(cmd);
		}

		public void addMove(MoveCommand cmd) {
			movecmds.AddLast(cmd);
			allcmds.AddLast(cmd);
		}

		public void addColor(ColorCommand cmd) {
			colorcmds.AddLast(cmd);
			allcmds.AddLast(cmd);
		}

		public void addFade(FadeCommand cmd) {
			fadecmds.AddLast(cmd);
			allcmds.AddLast(cmd);
		}

		public void update(int time, vec2 pos, float rot, vec4 color, float fade, vec2 size) {
			if (!isPhantomFrame) {
				if (starttime == -1) {
					starttime = time;
				}
				endtime = time;
			}

			vec2 scale = size / sdata.scalemod;
			fade *= color.w;
			vec3 col = color.xyz;

			addCmd<RotCommand, float>(rot, 0f, rotcmds, new RotCommand(time, time, rot, rot), RotCommand.requiresUpdate);
			rud<vec2> del = MoveCommand.requiresUpdate;
			if ((settings & INTERPOLATE_MOVE) > 0) {
				del = rud_true<vec2>;
			}
			addCmd<MoveCommand, vec2>(pos, v2(-1f), movecmds, new MoveCommand(time, time, pos, pos), del);
			addCmd<FadeCommand, float>(fade, 1f, fadecmds, new FadeCommand(time, time, fade, fade), FadeCommand.requiresUpdate);
			addCmd<ColorCommand, vec3>(col, v3(1f), colorcmds, new ColorCommand(time, time, col, col), ColorCommand.requiresUpdate);
			if (scale.x == scale.y) {
				goto squarescale;
			}
			addCmd<VScaleCommand, vec2>(scale, v2(1f), vscalecmds, new VScaleCommand(time, time, scale, scale), VScaleCommand.requiresUpdate);
			return;
squarescale:
			addCmd<ScaleCommand, float>(scale.x, 1f, scalecmds, new ScaleCommand(time, time, scale.x, scale.x), ScaleCommand.requiresUpdate);
		}

		private delegate bool rud<V>(V from, V next);
		private bool rud_true<T>(T from, T next) {
			return true;
		}
		private void addCmd<T, V>(V actualvalue, V defaultvalue, LinkedList<T> list, T cmd, rud<V> reqUpdateDelegate) where T : ICommand {
			V lastvalue = getLastNonPhantom<V, T>(list, defaultvalue);
			if (isPhantomFrame || reqUpdateDelegate(lastvalue, actualvalue)) {
				list.AddLast(cmd);
				allcmds.AddLast(cmd);
			}
		}

		private V getLastNonPhantom<V, L>(LinkedList<L> list, V defaultValue) where L : ICommand {
			LinkedListNode<L> last = list.Last;
			while (last != null) {
				if (!last.Value.isPhantom) {
					return (V) last.Value.To;
				}
				last = last.Previous;
			}
			return defaultValue;
		}

		public void fin(Writer w) {

			if ((settings & INTERPOLATE_MOVE) > 0) {
				interpolateMovement();
			}
			if ((settings & EASE_FADE) > 0) {
				easeFloatCommands<FadeCommand>(fadecmds);
			}
			if ((settings & EASE_SCALE) > 0) {
				easeFloatCommands<ScaleCommand>(scalecmds);
			}
			removePhantomCommands();

			// do not place this check under the deletion of the only movecmd
			// or white sprites might disappear (actually happend with zrub)
			if (allcmds.Count == 0) {
			     return;
			}

			vec2 initialPosition = v2(0f);
			if (movecmds.Count == 1 && movecmds.First.Value.to.Equals(movecmds.First.Value.from)) {
				initialPosition = movecmds.First.Value.to;
				allcmds.Remove(movecmds.First.Value);
				movecmds.Clear();
			}

			processOverrides();

			if ((settings & INTERPOLATE_MOVE) == 0) {
				if ((settings & NO_ADJUST_LAST) == 0) {
					adjustLastFrame();
				}
			} else {
				// ztunnel has seen sprites with single
				// color and scale command for one frame,
				// effectively invisible.
				int firsttime = -1;
				bool remove = true;
				foreach (ICommand cmd in allcmds) {
					if (cmd.start != cmd.end) {
						remove = false;
						break;
					}
					if (firsttime == -1) {
						firsttime = cmd.start;
					} else if (firsttime != cmd.start) {
						remove = false;
						break;
					}
				}
				if (remove) {
					return;
				}
			}

			addusagedata();
			w.ln(createsprite(initialPosition));

			foreach (ICommand cmd in allcmds) {
				w.ln(cmd.ToString());
			}
			foreach (string raw in raws) {
				w.ln(raw);
			}
		}

		private void processOverrides() {
			// TODO complete this when actually needed
			int actualendtime = endtime;
			if ((settings & INTERPOLATE_MOVE) == 0 && (settings & NO_ADJUST_LAST) == 0) {
			     actualendtime += framedelta;
			}
			foreach (ICommand o in overrides) {
				if (o is ColorCommand) {
					ColorCommand c = (ColorCommand) o;
					if (c.end <= starttime || c.start >= actualendtime) {
						continue;
					}
					if (c.start < starttime) {
						Equation e = Equation.fromNumber(c.easing);
						float x = e.calc(progress(c.start, c.end, starttime));
						c.start = starttime;
						c.from = lerp(c.from, c.to, x);
					}
					if (c.end > actualendtime) {
						Equation e = Equation.fromNumber(c.easing);
						float x = e.calc(progress(c.start, c.end, actualendtime));
						c.end = actualendtime;
						c.to = lerp(c.from, c.to, x);
					}
					colorcmds.AddLast(c);
					addOrdened(c);
				}
				if (o is FadeCommand) {
					FadeCommand f = (FadeCommand) o;
					if (f.end <= starttime || f.start >= actualendtime) {
						continue;
					}
					if (f.start < starttime) {
						Equation e = Equation.fromNumber(f.easing);
						float x = e.calc(progress(f.start, f.end, starttime));
						f.start = starttime;
						f.from = lerp(f.from, f.to, x);
					}
					if (f.end > actualendtime) {
						Equation e = Equation.fromNumber(f.easing);
						float x = e.calc(progress(f.start, f.end, actualendtime));
						f.end = actualendtime;
						f.to = lerp(f.from, f.to, x);
					}
					fadecmds.AddLast(f);
					addOrdened(f);
				}
			}
		}

		private void addOrdened(ICommand c) {
			LinkedListNode<ICommand> n = allcmds.First;
			if (n == null) {
				allcmds.AddFirst(c);
				return;
			}
			for (;;) {
				if (n.Value.start > c.start) {
					allcmds.AddBefore(n, c);
					return;
				}
				LinkedListNode<ICommand> next = n.Next;
				if (next == null) {
					allcmds.AddAfter(n, c);
					return;
				}
				n = next;
			}
		}

		private void easeFloatCommands<T>(LinkedList<T> cmds) where T : ICommand {
			if (cmds.Count < 2) {
				return;
			}
			LinkedListNode<T> node = cmds.First;
			LinkedList<T> batch = new LinkedList<T>();
			float prevval = 0f;
			float prevdif = 0f;
			while (node != null) {
				float curval = (float) node.Value.From;
				float dif = curval - prevval;
				if (dif * prevdif < 0f) {
					easeFloatCommandBatch<T>(batch, cmds);
					batch.Clear();
				}
				batch.AddLast(node.Value);
				prevdif = dif;
				prevval = curval;
				node = node.Next;
			}
			easeFloatCommandBatch<T>(batch, cmds);
		}

		private void easeFloatCommandBatch<T>(LinkedList<T> cmds, LinkedList<T> originalList) where T : ICommand {
			if (cmds.Count < 2) {
				return;
			}
			LinkedListNode<T> node = cmds.First;
			float from = (float) node.Value.From;
			float to = (float) cmds.Last.Value.To;
			int mintime = node.Value.start;
			int maxtime = cmds.Last.Value.end;
			List<Pair<float, float>> values = new List<Pair<float,float>>();
			float timediff = (float) maxtime - mintime;
			float valuemod = 1f / (to - from);
			float valuemin = min(to, from);
			while (node != null) {
				float t = (node.Value.start - mintime) / timediff;
				values.Add(new Pair<float,float>(t, (float) node.Value.From));
				node = node.Next;
			}
			int chosenEquation = -1;
			float bestscore = 1f;
			foreach (Equation e in Equation.all) {
				float maxdif = 0f;
				float avgdif = 0f;
				foreach (Pair<float, float> v in values) {
					float dif = abs(lerp(from, to, e.calc(v.a)) - v.b);
					maxdif = max(maxdif, dif);
					avgdif += dif;
				}
				avgdif /= values.Count;
				float score = avgdif + maxdif * 2f;
				if (score < bestscore) {
					bestscore = score;
					chosenEquation = e.number;
				}
			}
			if (chosenEquation == -1) {
				easeResultFailed++;
				return;
			}
			T cmd = cmds.First.Value;
			cmd.start = mintime;
			cmd.end = maxtime;
			cmd.easing = chosenEquation;
			cmd.From = from;
			cmd.To = to;
			cmd.isPhantom = false;
			easeCommandsSaved += cmds.Count - 1;
			node = cmds.First.Next;
			while (node != null) {
				LinkedListNode<T> next = node.Next;
				cmds.Remove(node);
				originalList.Remove(node.Value);
				allcmds.Remove(node.Value);
				node = next;
			}
			easeResultSuccess++;
		}

		private void interpolateMovement() {
			LinkedListNode<MoveCommand> mc = movecmds.First;
			if (mc == null) {
				return;
			}
			MoveCommand cmd = mc.Value;
			if (cmd.isPhantom) {
				cmd.isPhantom = false;
			}
			for (;;) {
				do {
					mc = mc.Next;
					if (mc == null) {
						goto exit;
					}
				} while (mc.Value.isPhantom);
				MoveCommand next = mc.Value;
				if (MoveCommand.requiresUpdate(cmd.to, next.from) || !next.from.Equals(next.to)) {
					cmd.end = next.start;
					cmd.to = next.from;
				}
				cmd = next;
			}
exit:
			MoveCommand n = movecmds.Last.Value;
			if (cmd != n) {
				cmd.end = n.start;
				cmd.to = n.from;
				n.isPhantom = true; // mark for removal
			}
		}

		private void removePhantomCommands() {
			removePhantomCommands<RotCommand>(rotcmds);
			removePhantomCommands<MoveCommand>(movecmds);
			removePhantomCommands<FadeCommand>(fadecmds);
			removePhantomCommands<ColorCommand>(colorcmds);
			removePhantomCommands<ScaleCommand>(scalecmds);
			removePhantomCommands<VScaleCommand>(vscalecmds);
		}

		private void removePhantomCommands<T>(LinkedList<T> list) where T : ICommand {
			LinkedListNode<T> n = list.First;
			while (n != null) {
				T cmd = n.Value;
				n = n.Next;
				if (cmd.isPhantom) {
					list.Remove(cmd);
					allcmds.Remove(cmd);
				}
			}
		}

		private void adjustLastFrame() {
			int actualendtime = endtime + framedelta;
			ICommand[] lastcmds = {
				rotcmds.Last == null ? null : rotcmds.Last.Value,
				movecmds.Last == null ? null : movecmds.Last.Value,
				fadecmds.Last == null ? null : fadecmds.Last.Value,
				colorcmds.Last == null ? null : colorcmds.Last.Value,
				scalecmds.Last == null ? null : scalecmds.Last.Value,
				vscalecmds.Last == null ? null : vscalecmds.Last.Value,
			};
			int lasttime = -1;
			for (int i = 0; i < lastcmds.Length; i++) {
				if (lastcmds[i] != null) {
					int end = lastcmds[i].end;
					if (end == actualendtime) {
						// nothing to do
						return;
					}
					if (!lastcmds[i].From.Equals(lastcmds[i].To) || end < lasttime) {
						lastcmds[i] = null;
						continue;
					}
					lasttime = lastcmds[i].end;
				}
			}
			for (int i = 0; i < lastcmds.Length; i++) {
				if (lastcmds[i] != null) {
					lastcmds[i].end = actualendtime;
					return;
				}
			}
			FadeCommand cmd = new FadeCommand(endtime, actualendtime, 1f, 1f);
			if (fadecmds.Last != null) {
				cmd.to = cmd.from = fadecmds.Last.Value.to;
			}
			fadecmds.AddLast(cmd);
			allcmds.AddLast(cmd);
		}

		private bool isoob(vec2 pos, float scale) {
			return all.isOnScreen(pos, sdata.size * scale);
		}

		private string createsprite(vec2 pos) {
			int origin = 1;
			if ((settings & ORIGIN_BOTTOMLEFT) > 0) {
				origin = 8;
			}
			return "4,3," + origin + "," + filename + "," + MoveCommand.round(pos.x) + "," + MoveCommand.round(pos.y);
		}

		private void addusagedata() {
			int count = 1;
			if (usagedata.ContainsKey(filename)) {
				count += usagedata[filename];
				usagedata.Remove(filename);
			}
			usagedata.Add(filename, count);
		}
	}
}
}
