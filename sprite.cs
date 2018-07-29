using System;
using System.Collections.Generic;
using System.Text;

namespace osusb1 {
partial class all {
	class Sprite {

		public static Dictionary<string, int> usagedata = new Dictionary<string,int>();

		string filename;
		float spritesize, scalemod;

		LinkedList<ICommand> allcmds = new LinkedList<ICommand>();
		LinkedList<MoveCommand> movecmds = new LinkedList<MoveCommand>();
		LinkedList<FadeCommand> fadecmds = new LinkedList<FadeCommand>();
		LinkedList<ColorCommand> colorcmds = new LinkedList<ColorCommand>();
		LinkedList<ScaleCommand> scalecmds = new LinkedList<ScaleCommand>();
		int starttime, endtime;

		public Sprite(string filename, float spritesize, float scalemod) {
			this.filename = filename;
			this.spritesize = spritesize;
			this.scalemod = scalemod;
			starttime = -1;
		}

		public static Sprite dot6_12() {
			return new Sprite("d", 12f, 6f);
		}

		public void update(int time, vec2 pos, vec4 color, float fade, float size) {
			if (!isPhantomFrame) {
				if (starttime == -1) {
					starttime = time;
				}
				endtime = time;
			}

			float scale = size / scalemod;
			fade *= color.w;
			vec3 col = color.xyz;

			addCmd<MoveCommand, vec2>(pos, v2(-1f), movecmds, new MoveCommand(time, time, pos, pos));
			addCmd<FadeCommand, float>(fade, 1f, fadecmds, new FadeCommand(time, time, fade, fade));
			addCmd<ColorCommand, vec3>(col, v3(1f), colorcmds, new ColorCommand(time, time, col, col));
			addCmd<ScaleCommand, float>(scale, 1f, scalecmds, new ScaleCommand(time, time, scale, scale));
		}

		private void addCmd<T, V>(V actualvalue, V defaultvalue, LinkedList<T> list, T cmd) where T : ICommand {
			V lastvalue = getLastNonPhantom<V, T>(list, defaultvalue);
			if (isPhantomFrame || !actualvalue.Equals(lastvalue)) {
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

			adjustLastFrame();

			// TODO: check for alpha 0 and hide?
			// TODO: oob

			addusagedata();
			w.ln(createsprite(initialPosition));

			foreach (ICommand cmd in allcmds) {
				w.ln(cmd.ToString());
			}
		}

		private void removePhantomCommands() {
			removePhantomCommands<MoveCommand>(movecmds);
			removePhantomCommands<FadeCommand>(fadecmds);
			removePhantomCommands<ColorCommand>(colorcmds);
			removePhantomCommands<ScaleCommand>(scalecmds);
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
			ICommand[] lastcmds = new ICommand[4];
			lastcmds[0] = movecmds.Last == null ? null : movecmds.Last.Value;
			lastcmds[1] = fadecmds.Last == null ? null : fadecmds.Last.Value;
			lastcmds[2] = colorcmds.Last == null ? null : colorcmds.Last.Value;
			lastcmds[3] = scalecmds.Last == null ? null : scalecmds.Last.Value;
			int lasttime = -1;
			for (int i = 0; i < 4; i++) {
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
			for (int i = 0; i < 4; i++) {
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
			float oost = spritesize * scale; // out-of-screen-threshold
			return pos.x < -oost || 640f + oost < pos.x ||
				pos.y < -oost || 480 + oost < pos.y;
		}

		private string createsprite(vec2 pos) {
			return "4,3,1," + filename + "," + MoveCommand.round(pos.x) + "," + MoveCommand.round(pos.y);
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
