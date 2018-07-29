//#define ASDOTS
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace osusb1 {
partial class all {
	class Zrub : Z {

		static Color defcol = Color.Gray;

#if ASDOTS
		const int RES = 1;
#else
		//const int RES = 6;
		const int RES = 2;
#endif
		Pixelscreen screen = new Pixelscreen(640 / RES, 480 / RES, RES);

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
		//const float SPACING = 10f;
		const float SPACING = 20f;

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
			public int dir = 1;
			public int mp = 1;
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
			this.mid = v3(0f, 20f, 100f);
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
				int[] dir = { 1, 1, 1, 1, 1, 1, 1, 1, 1 };
				foreach (string m in r.ReadToEnd().Split(' ')) {
					if (m == "<") {
						remap(mapping, Cube.R, Cube.F, Cube.B, Cube.U, Cube.D, Cube.L, FM, TMV, TMH);
						dir[TMH] *= -1;
						continue;
					}
					if (m == ">") {
						remap(mapping, Cube.L, Cube.B, Cube.F, Cube.U, Cube.D, Cube.R, FM, TMH, TMV);
						dir[TMV] *= -1;
						continue;
					}
					if (m == "^") {
						remap(mapping, Cube.D, Cube.L, Cube.R, Cube.F, Cube.B, Cube.U, TMH, FM, TMV);
						dir[TMH] *= -1;
						continue;
					}
					Mov mov = new Mov();
					moves.Add(mov);
					int l = m.Length - 1;
					if (m[l] == '2') {
						mov.mp = 2;
						l--;
					}
					if (m[l] == '\'') {
						mov.dir = -1;
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
				this.dottedrects[idx * 6 + i] = new Odottedrect(this.cubes[idx].rects[i], DOTCOUNT, 6f);
			}
			vec3 basepoint = v3(a - 1, b - 1, c - 1) * SPACING + v3(mid.x, mid.y, mid.z - SIZE / 2);
			new Pcube(this.points, pidx).set(basepoint, SIZE, SIZE, SIZE);

			this.rots[new int[] {Cube.L, TMV, Cube.R}[a]].cubes[b * 3 + c] = this.cubes[idx];
			this.rots[new int[] {Cube.F, TMH, Cube.B}[b]].cubes[a * 3 + c] = this.cubes[idx];
			this.rots[new int[] {Cube.D, FM, Cube.U}[c]].cubes[a * 3 + b] = this.cubes[idx];
		}

		// cube idx
		public int ci(int a, int b, int c) {
			return a * 9 + b * 3 + c;
		}

		// idx to cube
		public Cube i2c(int a, int b, int c) {
			return cubes[a * 9 + b * 3 + c];
		}

		// change cube points
		public void ccp(Cube cube, int flt, int frt, int frd, int fld, int bld, int blt, int brt, int brd) {
			cube.rects[Cube.F].updatepts(flt, frt, fld, frd);
			cube.rects[Cube.L].updatepts(blt, flt, bld, fld);
			cube.rects[Cube.R].updatepts(frt, brt, frd, brd);
			cube.rects[Cube.U].updatepts(blt, brt, flt, frt);
			cube.rects[Cube.D].updatepts(fld, frd, bld, brd);
			cube.rects[Cube.B].updatepts(brt, blt, brd, bld);
		}

		// rotate cube
		private void rc(Cube[] newcubes, int[,] cpi, int[,] cp, int[,] ocp, int[][] rots, int[][]orots, int a1, int b1, int c1, int a2, int b2, int c2, int[] m)
		{
			// moving 'from' to 'to'
			int fci = ci(a1, b1, c1);
			int tci = ci(a2, b2, c2);

			int[] i = new int[8];
			/*
			i[m[0]] = cpi[fci, Cube.F * 4 + 0];
			i[m[1]] = cpi[fci, Cube.F * 4 + 1];
			i[m[2]] = cpi[fci, Cube.F * 4 + 3];
			i[m[3]] = cpi[fci, Cube.F * 4 + 2];
			i[m[4]] = cpi[fci, Cube.L * 4 + 2];
			i[m[5]] = cpi[fci, Cube.U * 4 + 0];
			i[m[6]] = cpi[fci, Cube.B * 4 + 0];
			i[m[7]] = cpi[fci, Cube.D * 4 + 3];
			*/

			i[0] = cp[tci, 0];
			i[1] = cp[tci, 1];
			i[2] = cp[tci, 2];
			i[3] = cp[tci, 3];
			i[4] = cp[tci, 4];
			i[5] = cp[tci, 5];
			i[6] = cp[tci, 6];
			i[7] = cp[tci, 7];
			ccp(cubes[fci], i[0], i[1], i[2], i[3], i[4], i[5], i[6], i[7]);

			for (int j = 0; j < 8; j++) {
				//i[m[j]] = rots[fci][j];
				ocp[tci, j] = i[j];
			}

			int[] o = new int[8];
			for (int j = 0; j < 8; j++) {
				//rots[fci][j] = m[rots[fci][j]];
				/*
				for (int k = 0; k < 8; k++) {
					if (rots[fci][k] == j) {
						o[j] = rots[fci][k];
						break;
					}
				}
				*/
			}
			/*
			for (int j = 0; j < 8; j++) {
				rots[fci][j] = o[j];
			}
			*/
			//orots[tci] = rots[fci];

			cubes[fci].rots.Add(m);

			/*
			i[m[0]] = ocp[tci, 0];
			i[m[1]] = ocp[tci, 1];
			i[m[2]] = ocp[tci, 2];
			i[m[3]] = ocp[tci, 3];
			i[m[4]] = ocp[tci, 4];
			i[m[5]] = ocp[tci, 5];
			i[m[6]] = ocp[tci, 6];
			i[m[7]] = ocp[tci, 7];
			ccp(cubes[fci], i[0], i[1], i[2], i[3], i[4], i[5], i[6], i[7]);

			/*
			i[0] = ocp[tci, m[0]];
			i[1] = ocp[tci, m[1]];
			i[2] = ocp[tci, m[2]];
			i[3] = ocp[tci, m[3]];
			i[4] = ocp[tci, m[4]];
			i[5] = ocp[tci, m[5]];
			i[6] = ocp[tci, m[6]];
			i[7] = ocp[tci, m[7]];
			for (int j = 0; j < 8; j++) {
				ocp[tci, j] = i[j];
			}
			*/

			/*
			i[m[0]] = ocp[tci, 0];
			i[m[1]] = ocp[tci, 1];
			i[m[2]] = ocp[tci, 2];
			i[m[3]] = ocp[tci, 3];
			i[m[4]] = ocp[tci, 4];
			i[m[5]] = ocp[tci, 5];
			i[m[6]] = ocp[tci, 6];
			i[m[7]] = ocp[tci, 7];
			ccp(cubes[tci], i[0], i[1], i[2], i[3], i[4], i[5], i[6], i[7]);
			*/

			/*
			int[] o = new int[8];
			for (int j = 0; j < 8; j++) {
				o[j] = cp[tci, j];
			}
			for (int j = 0; j < 8; j++) {
				ocp[tci, j] = o[m[j]];
			}
			*/

			newcubes[tci] = cubes[fci];
		}

		private void rc2(int[,] cp, int idx, int[] m)
		{
			int[] i = new int[8];
			/*
			i[m[0]] = cp[idx, 0];
			i[m[1]] = cp[idx, 1];
			i[m[2]] = cp[idx, 2];
			i[m[3]] = cp[idx, 3];
			i[m[4]] = cp[idx, 4];
			i[m[5]] = cp[idx, 5];
			i[m[6]] = cp[idx, 6];
			i[m[7]] = cp[idx, 7];
			*/
			i[m[0]] = cubes[idx].rects[Cube.F].a;
			i[m[1]] = cubes[idx].rects[Cube.F].b;
			i[m[2]] = cubes[idx].rects[Cube.F].d;
			i[m[3]] = cubes[idx].rects[Cube.F].c;
			i[m[4]] = cubes[idx].rects[Cube.L].c;
			i[m[5]] = cubes[idx].rects[Cube.U].a;
			i[m[6]] = cubes[idx].rects[Cube.B].a;
			i[m[7]] = cubes[idx].rects[Cube.D].d;
			ccp(cubes[idx], i[0], i[1], i[2], i[3], i[4], i[5], i[6], i[7]);
		}

		private void rc3(Cube c) {
			int[] m = new int[8];
			for (int j = 0; j < 8; j++) {
				m[j] = j;
			}
			foreach (int[] r in c.rots) {
				int[] n = new int[8];
				Array.Copy(m, n, 8);
				for (int j = 0; j < 8; j++) {
					n[j] = m[r[j]];
				}
				m = n;
			}
			int[] i = new int[8];
			i[m[0]] = c.rects[Cube.F].a;
			i[m[1]] = c.rects[Cube.F].b;
			i[m[2]] = c.rects[Cube.F].d;
			i[m[3]] = c.rects[Cube.F].c;
			i[m[4]] = c.rects[Cube.L].c;
			i[m[5]] = c.rects[Cube.U].a;
			i[m[6]] = c.rects[Cube.B].a;
			i[m[7]] = c.rects[Cube.D].d;
			ccp(c, i[0], i[1], i[2], i[3], i[4], i[5], i[6], i[7]);
		}

		public override void draw(SCENE scene) {
			screen.clear();
			for (int i = 0; i < points.Length; i++) {
				this._points[i] = v3(this.points[i]);
			}
			int currentmove = scene.reltime / this.movetime;
			/*
			if (currentmove < this.moves.Count) {
				float moveprogress = (scene.reltime - currentmove * this.movetime) / (float) this.movetime;
				Mov mov = this.moves[currentmove];
				Rot rot = this.rots[mov.axis];
				foreach (Cube c in rot.cubes) {
					turn(c, this.mid, quat(rot.angles * moveprogress * 30f * mov.dir * mov.mp));
				}
			}
			*/

			Cube[] originalCubePositions = new Cube[cubes.Length];
			Array.Copy(cubes, originalCubePositions, cubes.Length);
			int[,] originalCubePoints = cubePointIndices();

			// a0 left
			// a2 right
			// b0 front
			// b2 back
			// c0 down
			// c2 up
			// axis  F L R U D B FM TMH TMV

			int[,] cp = new int[cubes.Length, 8];
			for (int i = 0; i < cubes.Length; i++) {
				cp[i, 0] = cubes[i].rects[Cube.F].a;
				cp[i, 1] = cubes[i].rects[Cube.F].b;
				cp[i, 2] = cubes[i].rects[Cube.F].d;
				cp[i, 3] = cubes[i].rects[Cube.F].c;
				cp[i, 4] = cubes[i].rects[Cube.L].c;
				cp[i, 5] = cubes[i].rects[Cube.U].a;
				cp[i, 6] = cubes[i].rects[Cube.B].a;
				cp[i, 7] = cubes[i].rects[Cube.D].d;
			}

			int[][] rots = new int[cubes.Length][];
			for (int i = 0; i < cubes.Length; i++) {
				rots[i] = new int[8];
				for (int j = 0; j < 8; j++) {
					rots[i][j] = j;
				}
			}

			for (int i = 0; i < cubes.Length; i++) {
				cubes[i].rots.Clear();
			}

			int[] dirfix = { 0, 2, 0, 0, 2, 2, 0, 2, 2 };
			for (int i = 0; i < currentmove && i < this.moves.Count; i++) {
				Cube[] newcubes = new Cube[cubes.Length];
				int axis = this.moves[i].axis;
				int mp = this.moves[i].mp;
				int amount = mp;
				if (amount != 2 && this.moves[i].dir == -1f) {
					amount += 2;
				}
				if (mp != 2) {
					amount = (amount + dirfix[axis]) % 4;
				}
				int[,] cpi = null;
				int a = 0, b = 0, c = 0;
				int[] mvmnt;
				const int flt = 0;
				const int frt = 1;
				const int frd = 2;
				const int fld = 3;
				const int bld = 4;
				const int blt = 5;
				const int brt = 6;
				const int brd = 7;
				switch (this.moves[i].axis) {
				case Cube.B:
					b++;
					goto case TMH;
				case TMH:
					b++;
					goto case Cube.F;
				case Cube.F:
					mvmnt = new int[] { fld, flt, frt, frd, brd, bld, blt, brt };
					//mvmnt = new int[] { Cube.F, Cube.D, Cube.U, Cube.L, Cube.R, Cube.B };
					while (amount-- > 0) {
						Array.Copy(cubes, newcubes, cubes.Length);
						cpi = cubePointIndices();
						int[,] ocp = new int[cubes.Length, 8];
						for (int j = 0; j < cubes.Length; j++) {
							for (int k = 0; k < 8; k++) {
								ocp[j, k] = cp[j, k];
							}
						}
						int[][] orots = new int[cubes.Length][];
						for (int j = 0; j < cubes.Length; j++) {
							orots[j] = rots[j];
						}
						rc(newcubes, cpi, cp, ocp, rots, orots, 0, b, 0, 0, b, 2, mvmnt);
						rc(newcubes, cpi, cp, ocp, rots, orots, 0, b, 1, 1, b, 2, mvmnt);
						rc(newcubes, cpi, cp, ocp, rots, orots, 0, b, 2, 2, b, 2, mvmnt);
						rc(newcubes, cpi, cp, ocp, rots, orots, 1, b, 2, 2, b, 1, mvmnt);
						rc(newcubes, cpi, cp, ocp, rots, orots, 2, b, 2, 2, b, 0, mvmnt);
						rc(newcubes, cpi, cp, ocp, rots, orots, 2, b, 1, 1, b, 0, mvmnt);
						rc(newcubes, cpi, cp, ocp, rots, orots, 2, b, 0, 0, b, 0, mvmnt);
						rc(newcubes, cpi, cp, ocp, rots, orots, 1, b, 0, 0, b, 1, mvmnt);
						cp = ocp;
						rots = orots;
						Array.Copy(newcubes, cubes, cubes.Length);
					}
					break;
				case Cube.R:
					a++;
					goto case TMV;
				case TMV:
					a++;
					goto case Cube.L;
				case Cube.L:
					mvmnt = new int[] { fld, frd, brd, bld, blt, flt, frt, brt };
					//mvmnt = new int[] { Cube.U, Cube.L, Cube.R, Cube.B, Cube.F, Cube.D };
					while (amount-- > 0) {
						Array.Copy(cubes, newcubes, cubes.Length);
						cpi = cubePointIndices();
						int[,] ocp = new int[cubes.Length, 8];
						for (int j = 0; j < cubes.Length; j++) {
							for (int k = 0; k < 8; k++) {
								ocp[j, k] = cp[j, k];
							}
						}
						int[][] orots = new int[cubes.Length][];
						for (int j = 0; j < cubes.Length; j++) {
							orots[j] = rots[j];
						}
						rc(newcubes, cpi, cp, ocp, rots, orots, a, 0, 2, a, 2, 2, mvmnt);
						rc(newcubes, cpi, cp, ocp, rots, orots, a, 0, 1, a, 1, 2, mvmnt);
						rc(newcubes, cpi, cp, ocp, rots, orots, a, 0, 0, a, 0, 2, mvmnt);
						rc(newcubes, cpi, cp, ocp, rots, orots, a, 1, 0, a, 0, 1, mvmnt);
						rc(newcubes, cpi, cp, ocp, rots, orots, a, 2, 0, a, 0, 0, mvmnt);
						rc(newcubes, cpi, cp, ocp, rots, orots, a, 2, 1, a, 1, 0, mvmnt);
						rc(newcubes, cpi, cp, ocp, rots, orots, a, 2, 2, a, 2, 0, mvmnt);
						rc(newcubes, cpi, cp, ocp, rots, orots, a, 1, 2, a, 2, 1, mvmnt);
						cp = ocp;
						rots = orots;
						Array.Copy(newcubes, cubes, cubes.Length);
					}
					break;
				}
				// a0 left
				// a2 right
				// b0 front
				// b2 back
				// c0 down
				// c2 up
				// axis  F L R U D B FM TMH TMV
			}

			for (int i = 0; i < cubes.Length; i++) {
				rc2(cubePointIndices(), i, rots[i]);
			}
			for (int i = 0; i < cubes.Length; i++) {
				rc3(cubes[i]);
			}

			turn(_points, _points, this.mid, scene.progress * 200f + all.mousex, scene.progress * 900f + all.mousey);
			foreach (Cube c in this.cubes) {
				c.draw(screen);
			}
#if ASDOTS
			foreach (Odottedrect o in this.dottedrects) {
				o.draw(scene, screen);
			}
#else
			screen.draw(scene);
#endif

			if (scene.g != null) {
				Font font = new Font("Tahoma", 14f);
				Brush brown = new SolidBrush(Color.Brown);
				Brush black = new SolidBrush(Color.Black);
				for (int i = 0; i < cubes.Length; i++) {
					Cube c = cubes[i];
					vec3 middle = v3(0f);
					for (int j = 0; j < 6; j++) {
						middle += _points[c.rects[j].a];
						middle += _points[c.rects[j].b];
						middle += _points[c.rects[j].c];
						middle += _points[c.rects[j].d];
					}
					middle /= 6 * 4;
					vec4 pt = p.Project(middle);
					string s = (i / 9) + "," + (i / 3) % 3 + "," + i % 3;
					SizeF size = scene.g.MeasureString(s, font);
					scene.g.DrawString(s, font, black, pt.x - size.Width / 2 + 1, pt.y + 1);
					//scene.g.DrawString(s, font, black, pt.x - size.Width / 2 + 1, pt.y - 1);
					//scene.g.DrawString(s, font, black, pt.x - size.Width / 2 - 1, pt.y + 1);
					//scene.g.DrawString(s, font, black, pt.x - size.Width / 2 - 1, pt.y - 1);
					scene.g.DrawString(s, font, brown, pt.x - size.Width / 2, pt.y);

					if (s == "0,0,2") {
						s = "0,0,2:";
						for (int j = 0; j < 8; j++) {
							s += "," + rots[i][j];
						}
						scene.g.DrawString(s, font, brown, 2, 2);
					}
					if (s == "0,1,2") {
						s = "0,1,2:";
						for (int j = 0; j < 8; j++) {
							s += "," + rots[i][j];
						}
						scene.g.DrawString(s, font, brown, 2, 16);
					}
				}
			}

			Array.Copy(originalCubePositions, cubes, cubes.Length);

			for (int i = 0; i < cubes.Length; i++) {
				for (int j = 0; j < cubes[i].rects.Length; j++) {
					int a = originalCubePoints[i, j * 4 + 0];
					int b = originalCubePoints[i, j * 4 + 1];
					int c = originalCubePoints[i, j * 4 + 2];
					int d = originalCubePoints[i, j * 4 + 3];
					cubes[i].rects[j].updatepts(a, b, c, d);
				}
			}
		}

		private int[,] cubePointIndices() {
			int[,] indices = new int[cubes.Length, 4 * 6];
			for (int i = 0; i < cubes.Length; i++) {
				for (int j = 0; j < cubes[i].rects.Length; j++) {
					indices[i, j * 4 + 0] = cubes[i].rects[j].a;
					indices[i, j * 4 + 1] = cubes[i].rects[j].b;
					indices[i, j * 4 + 2] = cubes[i].rects[j].c;
					indices[i, j * 4 + 3] = cubes[i].rects[j].d;
				}
			}
			return indices;
		}

		public override void fin(Writer w) {
#if ASDOTS
			foreach (Odottedrect o in this.dottedrects) {
				o.fin(w);
			}
#else
			screen.fin(w);
#endif
		}

	}
}
}
