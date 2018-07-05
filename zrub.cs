using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace osusb1 {
partial class all {
	class Zrub : Z {

		static Color defcol = Color.Gray;

		Pixelscreen screen = new Pixelscreen(640, 480, 1);

		vec3[] points;
		vec3[] _points;
		Cube[] cubes;
		Rot[] rots;
		Odottedrect[] dottedrects;

		const int FM = 6;
		const int TMH = 7;
		const int TMV = 8;

		string[] SIDES = { "F", "L", "R", "T", "D", "B", "FM", "TMH", "TMV" };

		const float SIZE = 10f;
		const float SPACING = 10f;

		const int DOTCOUNT = 5;

		class Rot {
			public Cube[] cubes;
			public vec3 angles;
			public Rot(vec3 angles) {
				this.angles = angles;
				this.cubes = new Cube[9];
			}
		}

		class Mov {
			public int axis;
			public float dir = 1f;
			public float mp = 1f;
		}

		List<Mov> moves;
		readonly int movetime;

		const int DONETIME = 500;
		const int MOVEDELAY = 0;

		vec3 mid;

		public Zrub(int start, int stop) {
			this.start = start;
			this.stop = stop;

			this.moves = new List<Mov>();
			this.mid = v3(0f, 50f, 100f);
			this.points = new vec3[27 * 8];
			this._points = new vec3[27 * 8];
			this.cubes = new Cube[27];
			this.dottedrects = new Odottedrect[27 * 6];
			this.rots = new Rot[6 + 3];
			this.rots[Cube.L] = new Rot(v3(0f, +1f, 0f));
			this.rots[Cube.R] = new Rot(v3(0f, -1f, 0f));
			this.rots[Cube.F] = new Rot(v3(+1f, 0f, 0f));
			this.rots[Cube.B] = new Rot(v3(-1f, 0f, 0f));
			this.rots[Cube.D] = new Rot(v3(0f, 0f, +1f));
			this.rots[Cube.U] = new Rot(v3(0f, 0f, -1f));
			this.rots[FM] = new Rot(v3(0f, 0f, -1f));
			this.rots[TMH] = new Rot(v3(-1f, 0f, 0f));
			this.rots[TMV] = new Rot(v3(0f, +1f, 0f));

			for (int a = 0; a < 3; a++) {
				for (int b = 0; b < 3; b++) {
					for (int c = 0; c < 3; c++) {
						gen(a, b, c);
					}
				}
			}

			using (StreamReader r = new StreamReader("rub.txt")) {
				Dictionary<string, int> mapping = new Dictionary<string,int>();
				for (int i = 0; i < SIDES.Length; i++) {
					mapping.Add(SIDES[i], i);
				}
				float[] dir = { 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f };
				foreach (string m in r.ReadToEnd().Split(' ')) {
					if (m == "<") {
						remap(mapping, Cube.R, Cube.F, Cube.B, Cube.U, Cube.D, Cube.L, FM, TMV, TMH);
						dir[TMH] *= -1f;
						continue;
					}
					if (m == ">") {
						remap(mapping, Cube.L, Cube.B, Cube.F, Cube.U, Cube.D, Cube.R, FM, TMH, TMV);
						dir[TMV] *= -1f;
						continue;
					}
					if (m == "^") {
						remap(mapping, Cube.D, Cube.L, Cube.R, Cube.F, Cube.B, Cube.U, TMH, FM, TMV);
						dir[TMH] *= -1f;
						continue;
					}
					Mov mov = new Mov();
					moves.Add(mov);
					int l = m.Length - 1;
					if (m[l] == '2') {
						mov.mp = 2f;
						l--;
					}
					if (m[l] == '\'') {
						mov.dir = -1f;
						l--;
					}
					mov.axis = mapping[m.Substring(0, l + 1)];
					mov.dir *= dir[mov.axis];
				}
			}
			this.movetime = ((stop - start) - DONETIME + MOVEDELAY) / this.moves.Count - MOVEDELAY;
		}

		private void remap(Dictionary<string, int> mapping, params int[] nm) {
			int[] prevvals = new int[SIDES.Length];
			for (int i = 0; i < SIDES.Length; i++) {
				prevvals[i] = mapping[SIDES[i]];
				mapping.Remove(SIDES[i]);
			}
			for (int i = 0; i < SIDES.Length; i++) {
				mapping.Add(SIDES[i], prevvals[nm[i]]);
			}
		}

		private void gen(int a, int b, int c) {
			Color[] cols = new Color[6];
			for (int i = 0; i < 6; i++) {
				cols[i] = Color.Gray;
			}
			cols[Cube.L] = new Color[] { Color.Red, defcol, defcol }[a];
			cols[Cube.R] = new Color[] { defcol, defcol, Color.Orange }[a];
			cols[Cube.F] = new Color[] { Color.White, defcol, defcol }[b];
			cols[Cube.B] = new Color[] { defcol, defcol, Color.Yellow }[b];
			cols[Cube.D] = new Color[] { Color.Blue, defcol, defcol }[c];
			cols[Cube.U] = new Color[] { defcol, defcol, Color.Green }[c];
			int idx = a * 9 + b * 3 + c;
			int pidx = idx * 8;
			this.cubes[idx] = new Cube(cols, this._points, pidx);
			for (int i = 0; i < 6; i++) {
				this.dottedrects[idx * 6 + i] = new Odottedrect(this.cubes[idx].rects[i], DOTCOUNT);
			}
			vec3 basepoint = v3(a - 1, b - 1, c - 1) * SPACING + v3(mid.x, mid.y, mid.z - SIZE / 2);
			new Pcube(this.points, pidx).set(basepoint, SIZE, SIZE, SIZE);

			this.rots[new int[] {Cube.L, TMV, Cube.R}[a]].cubes[b * 3 + c] = this.cubes[idx];
			this.rots[new int[] {Cube.F, TMH, Cube.B}[b]].cubes[a * 3 + c] = this.cubes[idx];
			this.rots[new int[] {Cube.D, FM, Cube.U}[c]].cubes[a * 3 + b] = this.cubes[idx];
		}

		public override void draw(SCENE scene) {
			screen.clear();
			for (int i = 0; i < points.Length; i++) {
				this._points[i] = v3(this.points[i]);
			}
			Color[] prevcols = new Color[this.cubes.Length * 6];
			for (int a = 0; a < this.cubes.Length; a++) {
				for (int b = 0; b < 6; b++) {
					prevcols[a * 6 + b] = this.cubes[a].rects[b].color;
				}
			}
			int currentmove = scene.reltime / this.movetime;
			if (currentmove < this.moves.Count) {
				float moveprogress = (scene.reltime - currentmove * this.movetime) / (float) this.movetime;
				Mov mov = this.moves[currentmove];
				Rot rot = this.rots[mov.axis];
				foreach (Cube c in rot.cubes) {
					turn(c, this.mid, quat(rot.angles * moveprogress * 30f * mov.dir * mov.mp));
				}
			}
			int[] order = { 0, 1, 2, 2, 5, 8, 8, 7, 6, 6, 3, 0 };
			int[] dirfix = { 0, 2, 0, 0, 2, 2, 0, 2, 2 };
			int[] movmat = {
				Cube.L, Cube.U, Cube.R, Cube.D,
				Cube.F, Cube.U, Cube.B, Cube.D,
				Cube.F, Cube.U, Cube.B, Cube.D,
				Cube.L, Cube.B, Cube.R, Cube.F,
				Cube.L, Cube.B, Cube.R, Cube.F,
				Cube.L, Cube.U, Cube.R, Cube.D,
				Cube.L, Cube.B, Cube.R, Cube.F,
				Cube.L, Cube.U, Cube.R, Cube.D,
				Cube.F, Cube.U, Cube.B, Cube.D,
		        };
			int[] movmat2 = { Cube.F, Cube.L, Cube.R, Cube.U, Cube.D, Cube.B, Cube.U, Cube.F, Cube.L };
			Rect[] rects = new Rect[12];
			Rect[] rects2 = new Rect[12];
			Color[] cols = new Color[12];
			Color[] cols2 = new Color[12];
			for (int i = 0; i < currentmove && i < this.moves.Count; i++) {
				int axis = this.moves[i].axis;
				Cube[] cubs = this.rots[axis].cubes;
				int mp = (int) this.moves[i].mp;
				int amount = mp;
				if (amount != 2 && this.moves[i].dir == -1f) {
					amount += 2;
				}
				if (mp != 2) {
					amount = (amount + dirfix[axis]) % 4;
				}
				while (amount-- > 0) {
					for (int z = 0; z < 12; z++) {
						rects[z] = cubs[order[z]].rects[movmat[axis * 4 + z / 3]];
						cols[z] = rects[z].color;
						rects2[z] = cubs[order[z]].rects[movmat2[axis]];
						cols2[z] = rects2[z].color;
					}
					for (int z = 0; z < rects.Length; z++) {
						rects[z].setColor(cols[(z + 9) % 12]);
						rects2[z].setColor(cols2[(z + 9) % 12]);
					}
				}
			}
			turn(_points, _points, this.mid, scene.progress * 200f + all.mousex, scene.progress * 900f + all.mousey);
			foreach (Cube c in this.cubes) {
				c.draw(screen);
			}
			//screen.draw(scene);
			foreach (Odottedrect o in this.dottedrects) {
				o.draw(scene, screen);
			}
			for (int a = 0; a < this.cubes.Length; a++) {
				for (int b = 0; b < 6; b++) {
					this.cubes[a].rects[b].color = prevcols[a * 6 + b];
				}
			}
		}

		public override void fin(Writer w) {
			foreach (Odottedrect o in this.dottedrects) {
				o.fin(w);
			}
		}

	}
}
}
