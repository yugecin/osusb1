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
		public const string SPRITE_DOT_6_12 = "d";
		public const string SPRITE_TRI = "t";
		public const string SPRITE_SQUARE_6_6 = "";

		public static Dictionary<string, int> usagedata = new Dictionary<string,int>();
		public static int easeResultSuccess = 0;
		public static int easeResultFailed = 0;

		private static Dictionary<string, SDATA> spritedata = new Dictionary<string,SDATA>();

		static Sprite() {
			spritedata.Add(SPRITE_SQUARE_6_6, new SDATA(6f, 6f));
			spritedata.Add(SPRITE_DOT_6_12, new SDATA(12f, 6f));
			spritedata.Add(SPRITE_TRI, new SDATA(600f, 600f));
		}

		public static float Size(string filename) {
			return spritedata[filename].size;
		}

		LinkedList<ICommand> allcmds = new LinkedList<ICommand>();
		LinkedList<RotCommand> rotcmds = new LinkedList<RotCommand>();
		LinkedList<MoveCommand> movecmds = new LinkedList<MoveCommand>();
		LinkedList<FadeCommand> fadecmds = new LinkedList<FadeCommand>();
		LinkedList<ColorCommand> colorcmds = new LinkedList<ColorCommand>();
		LinkedList<ScaleCommand> scalecmds = new LinkedList<ScaleCommand>();
		LinkedList<VScaleCommand> vscalecmds = new LinkedList<VScaleCommand>();

		string filename;
		SDATA sdata;
		int settings;
		int starttime, endtime;

		public Sprite(string filename, int settings) {
			this.filename = filename;
			this.settings = settings;
			this.sdata = spritedata[filename];
			starttime = -1;
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
			addCmd<MoveCommand, vec2>(pos, v2(-1f), movecmds, new MoveCommand(time, time, pos, pos), MoveCommand.requiresUpdate);
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
			if (movecmds.Count == 1) {
				initialPosition = movecmds.First.Value.to;
				allcmds.Remove(movecmds.First.Value);
				movecmds.Clear();
			}

			if ((settings & INTERPOLATE_MOVE) == 0) {
				adjustLastFrame();
			}

			// TODO: check for alpha 0 and hide?
			// TODO: oob

			addusagedata();
			w.ln(createsprite(initialPosition));

			foreach (ICommand cmd in allcmds) {
				w.ln(cmd.ToString());
			}
		}

		private void easeFloatCommands<T>(LinkedList<T> cmds) where T : ICommand {
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
			float valuemod = 1f / abs(to - from);
			float valuemin = min(to, from);
			while (node != null) {
				float t = (node.Value.start - mintime) / timediff;
				float adj = valuemod * ((float) node.Value.From - valuemin);
				values.Add(new Pair<float,float>(t, adj));
				node = node.Next;
			}
			int chosenEquation = -1;
			float bestscore = 0.5f;
			foreach (Equation e in Equation.all) {
				float maxdif = 0f;
				float avgdif = 0f;
				foreach (Pair<float, float> v in values) {
					float dif = abs(e.calc(v.a) - v.b);
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
			node = cmds.First.Next;
			while (node != null) {
				LinkedListNode<T> next = node.Next;
				cmds.Remove(node);
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
				cmd.end = next.start;
				cmd.to = next.from;
				cmd = next;
			}
exit:
			MoveCommand n = movecmds.Last.Value;
			if (cmd == n) {
				return;
			}
			cmd.end = n.start;
			cmd.to = n.from;
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
				cmd.from = fadecmds.Last.Value.to;
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
