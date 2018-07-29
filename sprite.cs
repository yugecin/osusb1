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
			if (starttime == -1) {
				starttime = time;
			}
			endtime = time;

			float scale = size / scalemod;
			fade *= color.w;
			vec3 col = color.xyz;

			vec2 lastPosition = movecmds.Last == null ? v2(-1f) : movecmds.Last.Value.to;
			if (!pos.Equals(lastPosition)) {
				MoveCommand cmd = new MoveCommand(time, time, pos, pos);
				movecmds.AddLast(cmd);
				allcmds.AddLast(cmd);
			}

			float lastFade = fadecmds.Last == null ? 1f : fadecmds.Last.Value.to;
			if (!fade.Equals(lastFade)) {
				FadeCommand cmd = new FadeCommand(time, time, fade, fade);
				fadecmds.AddLast(cmd);
				allcmds.AddLast(cmd);
			}

			vec3 lastColor = colorcmds.Last == null ? v3(1f) : colorcmds.Last.Value.to;
			if (!col.Equals(lastColor)) {
				ColorCommand cmd = new ColorCommand(time, time, col, col);
				colorcmds.AddLast(cmd);
				allcmds.AddLast(cmd);
			}

			float lastScale = scalecmds.Last == null ? 1f : scalecmds.Last.Value.to;
			if (!scale.Equals(lastScale)) {
				ScaleCommand cmd = new ScaleCommand(time, time, scale, scale);
				scalecmds.AddLast(cmd);
				allcmds.AddLast(cmd);
			}
		}

		public void fin(Writer w) {

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

		private void adjustLastFrame() {
			ICommand[] lastcmds = new ICommand[4];
			lastcmds[0] = movecmds.Last == null ? null : movecmds.Last.Value;
			lastcmds[1] = fadecmds.Last == null ? null : fadecmds.Last.Value;
			lastcmds[2] = colorcmds.Last == null ? null : colorcmds.Last.Value;
			lastcmds[3] = scalecmds.Last == null ? null : scalecmds.Last.Value;
			int lasttime = -1;
			for (int i = 0; i < 4; i++) {
				if (lastcmds[i] != null) {
					int end = lastcmds[i].end;
					if (!lastcmds[i].From.Equals(lastcmds[i].To) || end < lasttime) {
						lastcmds[i] = null;
						continue;
					}
					lasttime = lastcmds[i].end;
				}
			}
			for (int i = 0; i < 4; i++) {
				if (lastcmds[i] != null) {
					lastcmds[i].end = endtime + framedelta;
					return;
				}
			}
			FadeCommand cmd = new FadeCommand(endtime, endtime + framedelta, 1f, 1f);
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
