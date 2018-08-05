#define ASDOTS
//#define ASRECTS
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace osusb1 {
partial class all {
	class R : Dictionary<Cube, List<int[]>> { }
	class Zrub : Z {

		static Color defcol = Color.Gray;

#if ASDOTS
		const int RES = 1;
#else
		//const int RES = 6;
		const int RES = 2;
#endif
		Pixelscreen screen = new Pixelscreen(640 / RES, 480 / RES, RES);
#if ASRECTS
		Orect[] orects = new Orect[6 * 27];
#endif

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
		//const float SPACING = 20f;

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
		const int SPRITESETTINGS = Sprite.INTERPOLATE_MOVE;

		public static vec3 mid = v3(0f, 30f, 100f);

		public Zrub(int start, int stop) {
			this.start = start;
			this.stop = stop;
			framedelta = 100;
			phantomframedelta = 20;

			this.moves = new List<Mov>();
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

#if ASRECTS
			List<Rect> coloredrects = new List<Rect>();
			int idx = 0;
			foreach (Cube c in cubes) {
				for (int i = 0; i < 6; i++) {
					Rect r = c.rects[i];
					if (r.color == defcol) {
						orects[idx++] = new Orect(r, 0);
						continue;	
					}
					coloredrects.Add(c.rects[i]);
				}
			}
			foreach (Rect r in coloredrects) {
				orects[idx++] = new Orect(r, 0);
			}
#endif
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
				int index = idx * 6 + i;
				Rect rect = cubes[idx].rects[i];
				this.dottedrects[index] = new Odottedrect(rect, DOTCOUNT, 6f, SPRITESETTINGS);
			}
			vec3 basepoint = v3(a - 1, b - 1, c - 1) * SPACING + mid;
			basepoint.z -= SIZE / 2;
			new Pcube(this.points, pidx).set(basepoint, SIZE, SIZE, SIZE);

			this.rots[new int[] {Cube.L, TMV, Cube.R}[a]].cubes[b * 3 + c] = this.cubes[idx];
			this.rots[new int[] {Cube.F, TMH, Cube.B}[b]].cubes[a * 3 + c] = this.cubes[idx];
			this.rots[new int[] {Cube.D, FM, Cube.U}[c]].cubes[a * 3 + b] = this.cubes[idx];
		}

		public int ci(int a, int b, int c) {
			return a * 9 + b * 3 + c;
		}

		public Cube i2c(int a, int b, int c) {
			return cubes[a * 9 + b * 3 + c];
		}

		public void ccp(Cube cube, int[] i) {
			cube.rects[Cube.F].updatepts(i[0], i[1], i[3], i[2]);
			cube.rects[Cube.L].updatepts(i[5], i[0], i[4], i[3]);
			cube.rects[Cube.R].updatepts(i[1], i[6], i[2], i[7]);
			cube.rects[Cube.U].updatepts(i[5], i[6], i[0], i[1]);
			cube.rects[Cube.D].updatepts(i[3], i[2], i[4], i[7]);
			cube.rects[Cube.B].updatepts(i[6], i[5], i[7], i[4]);
		}

		private void rc(Cube[] newcubes, int[,] cp, R rotations, int[] coords, int[] coordmods, int[] m)
		{
			// moving 'from' to 'to'
			int fci = ci(coords[0] + coordmods[0], coords[1] + coordmods[1], coords[2] + coordmods[2]);
			int tci = ci(coords[3] + coordmods[0], coords[4] + coordmods[1], coords[5] + coordmods[2]);

			int[] i = new int[8];
			for (int j = 0; j < 8; j++) {
				i[j] = cp[tci, j];
			}
			ccp(cubes[fci], i);

			rotations[cubes[fci]].Add(m);
			newcubes[tci] = cubes[fci];
		}

		private void rc3(int cube, List<int[]> rots) {
			int[] m = new int[8];
			for (int j = 0; j < 8; j++) {
				m[j] = j;
			}
			foreach (int[] r in rots) {
				int[] n = new int[8];
				Array.Copy(m, n, 8);
				for (int j = 0; j < 8; j++) {
					n[j] = m[r[j]];
				}
				m = n;
			}
			int[] i = new int[8];
			for (int j = 0; j < 8; j++) {
				i[m[j]] = pt(cube, j);
			}
			ccp(cubes[cube], i);
		}

		private int pt(int idx, int i) {
			switch (i) {
			case 0: return cubes[idx].rects[Cube.F].a;
			case 1: return cubes[idx].rects[Cube.F].b;
			case 2: return cubes[idx].rects[Cube.F].d;
			case 3: return cubes[idx].rects[Cube.F].c;
			case 4: return cubes[idx].rects[Cube.L].c;
			case 5: return cubes[idx].rects[Cube.U].a;
			case 6: return cubes[idx].rects[Cube.B].a;
			case 7: return cubes[idx].rects[Cube.D].d;
			}
			throw new Exception("hi");
		}

		int[,][] movmat = {{new int[]{0,0,0,0,0,2},new int[]{0,0,1,1,0,2},new int[]{0,0,2,2,0,2},
		                    new int[]{1,0,2,2,0,1},new int[]{2,0,2,2,0,0},new int[]{2,0,1,1,0,0},
				    new int[]{2,0,0,0,0,0},new int[]{1,0,0,0,0,1},new int[]{1,0,1,1,0,1}},
		                   {new int[]{0,0,2,0,2,2},new int[]{0,0,1,0,1,2},new int[]{0,0,0,0,0,2},
				    new int[]{0,1,0,0,0,1},new int[]{0,2,0,0,0,0},new int[]{0,2,1,0,1,0},
				    new int[]{0,2,2,0,2,0},new int[]{0,1,2,0,2,1},new int[]{0,1,1,0,1,1}},
		                   {new int[]{0,0,0,0,2,0},new int[]{0,1,0,1,2,0},new int[]{0,2,0,2,2,0},
				    new int[]{1,2,0,2,1,0},new int[]{2,2,0,2,0,0},new int[]{2,1,0,1,0,0},
				    new int[]{2,0,0,0,0,0},new int[]{1,0,0,0,1,0},new int[]{1,1,0,1,1,0}}};
		int[][] emovmat = {new int[]{0,0,0},new int[]{0,0,0},new int[]{2,0,0},
		                   new int[]{0,0,2},new int[]{0,0,0},new int[]{0,2,0},
		                   new int[]{0,0,1},new int[]{0,1,0},new int[]{1,0,0}};
		int[][] rotmat = {new int[]{3,0,1,2,7,4,5,6},
		                  new int[]{3,2,7,4,5,0,1,6},
		                  new int[]{1,6,7,2,3,0,5,4}};
		int[] rmref = {0,1,1,2,2,0,2,0,1};
		int[] dirfix = {0,2,0,0,2,2,0,2,2};

		public override void draw(SCENE scene) {
			screen.clear();
			for (int i = 0; i < points.Length; i++) {
				this._points[i] = v3(this.points[i]);
			}
			int currentmove = scene.reltime / this.movetime;
			if (currentmove < this.moves.Count) {
				float moveprogress = (scene.reltime - currentmove * this.movetime) / (float) this.movetime;
				Mov mov = this.moves[currentmove];
				Rot rot = this.rots[mov.axis];
				foreach (Cube c in rot.cubes) {
					turn(c, mid, quat(rot.angles * moveprogress * 30f * mov.dir * mov.mp));
				}
			}

			Cube[] originalCubePositions = new Cube[cubes.Length];
			Array.Copy(cubes, originalCubePositions, cubes.Length);
			int[,] originalCubePoints = new int[cubes.Length, 4 * 6];
			int[,] cp = new int[cubes.Length, 8];
			R rots = new R();
			for (int i = 0; i < cubes.Length; i++) {
				for (int j = 0; j < cubes[i].rects.Length; j++) {
					originalCubePoints[i, j * 4 + 0] = cubes[i].rects[j].a;
					originalCubePoints[i, j * 4 + 1] = cubes[i].rects[j].b;
					originalCubePoints[i, j * 4 + 2] = cubes[i].rects[j].c;
					originalCubePoints[i, j * 4 + 3] = cubes[i].rects[j].d;
				}
				for (int j = 0; j < 8; j++) {
					cp[i, j] = pt(i, j);
				}
				rots.Add(cubes[i], new List<int[]>());
			}

			Cube[] nc = new Cube[cubes.Length];
			for (int i = 0; i < currentmove && i < moves.Count; i++) {
				int axis = moves[i].axis;
				int amount = (moves[i].mp + (1 - (moves[i].mp - 1)) * (((moves[i].dir >> 1) & 1) * 2 + dirfix[axis])) % 4;
				while (amount-- > 0) {
					Array.Copy(cubes, nc, cubes.Length);
					for (int j = 0; j < 9; j++) {
						rc(nc, cp, rots, movmat[rmref[axis], j], emovmat[axis], rotmat[rmref[axis]]);
					}
					Array.Copy(nc, cubes, cubes.Length);
				}
			}

			for (int i = 0; i < cubes.Length; i++) {
				rc3(i, rots[cubes[i]]);
			}

			//turn(_points, _points, mid, scene.progress * 200f + all.mousex, scene.progress * 900f + all.mousey);
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
#if ASRECTS
			foreach (Orect o in orects) {
				o.update(scene);
			}
#endif

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

		public override void fin(Writer w) {
#if ASRECTS
			foreach (Orect o in orects) {
				o.fin(w);
			}
#else
#if ASDOTS
			foreach (Odottedrect o in this.dottedrects) {
				o.fin(w);
			}
#else
			screen.fin(w);
#endif
#endif
		}

	}
}
}
