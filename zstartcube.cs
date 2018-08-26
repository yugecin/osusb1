using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace osusb1 {
partial class all {
	class Zstartcube : Z {

		vec3 mid = v3(0f, -50f, 100f);

		vec3[] points;
		vec3[] _points;
		Cube cube;
		LINE[] lines;
		TEXT[] texts;

		Pixelscreen screen = new Pixelscreen(640, 480, 1);

		struct LINE {
			public Oline line;
			public Rect r1, r2;
			public vec3[] pts;
			public int a, b;
			public LINE(Rect r1, Rect r2, int a, int b) {
				pts = new vec3[2];
				line = new Oline(pts, 0, 1);
				this.r1 = r1;
				this.r2 = r2;
				this.a = a;
				this.b = b;
			}
		}

		const float SIZE = 15f;

		public Zstartcube(int start, int stop) {
			this.start = start;
			this.stop = stop;
			framedelta = 50;

			points = new vec3[8];
			_points = new vec3[8];
			new Pcube(points, 0).set(mid - v3(0f, 0f, SIZE / 2f), SIZE, SIZE, SIZE);
			Color[] cols = {
				Color.Red,
				Color.Blue,
				Color.Orange,
				Color.Purple,
				Color.Magenta,
				Color.Cyan,
			};
			cube = new Cube(cols, _points, 0);
			Rect[] r = cube.rects;
			lines = new LINE[] {
				new LINE(r[Cube.F], r[Cube.U], 0, 1),
				new LINE(r[Cube.F], r[Cube.L], 3, 0),
				new LINE(r[Cube.F], r[Cube.R], 1, 2),
				new LINE(r[Cube.F], r[Cube.D], 2, 3),
				new LINE(r[Cube.L], r[Cube.U], 0, 5),
				new LINE(r[Cube.L], r[Cube.B], 5, 4),
				new LINE(r[Cube.L], r[Cube.D], 4, 3),
				new LINE(r[Cube.B], r[Cube.D], 4, 7),
				new LINE(r[Cube.R], r[Cube.U], 6, 1),
				new LINE(r[Cube.R], r[Cube.B], 7, 6),
				new LINE(r[Cube.R], r[Cube.D], 2, 7),
				new LINE(r[Cube.B], r[Cube.U], 6, 5),
			};

			texts = new TEXT[] {
				gentext(cols[Cube.R], PI2, 0f, 0f, "robin_be", "presents"),
				gentext(cols[Cube.U], PI2, PI2, -PI2, "a", "storyboard"),
				gentext(cols[Cube.L], -PI2, 0f, 0f, "without", "a story"),
				gentext(cols[Cube.B], PI, 0f, -PI2, "made with", "5 sprites"),
				gentext(cols[Cube.D], PI, -PI2, 0f, "additional", "help by", "Emily <3"),
			};
		}

		public override void draw(SCENE scene) {
			ICommand.round_move_decimals.Push(5);
			copy(_points, points);

			vec4 qm1 = quat(0f, 0f, mouse.x / 100f);
			vec4 qm2 = quat(0f, mouse.y / 100f, 0f);
			vec4 q1 = quat(0f, 0f, -scene.reltime / 4000f);
			vec4 q2 = quat(0f, -scene.reltime / 5600f, 0f);
			vec4 q3 = quat(scene.reltime / 8000f, 0f, 0f);
			turn(_points, mid, qm1);
			turn(_points, mid, qm2);
			turn(_points, mid, q1);
			turn(_points, mid, q2);
			turn(_points, mid, q3);

			screen.clear();
			cube.draw(screen);

			float linex = progressx(0, 2000, scene.reltime);
			linex = 1f;
			bool cull = linex == 1f;
			foreach (LINE l in lines) {
				if (cull && l.r1.shouldcull() && l.r2.shouldcull()) {
					l.line.update(scene.time, null);
					continue;
				}
				l.pts[0] = _points[l.a];
				l.pts[1] = _points[l.b];
				if (linex != 1f) {
					l.pts[1] = lerp(l.pts[0], l.pts[1], linex);
				}
				l.line.update(scene.time, v4(1f));
				l.line.draw(scene.g);
			}

			foreach (TEXT t in texts) {
				copy(t._points, t.points);
				turn(t._points, mid, qm1);
				turn(t._points, mid, qm2);
				turn(t._points, mid, q1);
				turn(t._points, mid, q2);
				turn(t._points, mid, q3);
				for (int i = 0; i < t.odots.Length; i++) {
					vec4 p = project(t._points[i]);
					var owner = screen.ownerAt(p.xy);
					if (owner is Tri && (owner as Tri).color == t.col) {
						t.odots[i].update(scene.time, v4(1f), p);
						t.odots[i].draw(scene.g);
						continue;
					}
					t.odots[i].update(scene.time, null, null);
				}
			}
			ICommand.round_move_decimals.Pop();
		}

		public override void fin(Writer w) {
			ICommand.round_move_decimals.Push(5);
			foreach (LINE l in lines) {
				l.line.fin(w);
			}
			foreach (TEXT t in texts) {
				foreach (Odot o in t.odots) {
					o.fin(w);
				}
			}
			ICommand.round_move_decimals.Pop();
		}

		struct TEXT {
			public vec3[] points;
			public vec3[] _points;
			public Odot[] odots;
			public Color col;
		}

		private TEXT gentext(Color col, float rx, float ry, float rz, params string[] lines) {
			int pointc = 0;
			foreach (string line in lines) {
				pointc += font.calcPointCount(line);
			}
			vec3[] points = new vec3[pointc];

			vec4 qx = quat(0f, 0f, rx);
			vec4 qy = quat(0f, ry, 0f);
			vec4 qz = quat(rz, 0f, 0f);

			const float SPACING = .2f;

			float startz = (lines.Length * font.charheight + (lines.Length - 1)) * SPACING / 2f;
			int pointidx = 0;
			for (int z = 0; z < lines.Length; z++) {
				string line = lines[z];
				int width = font.textWidth(line);
				vec3 topleft = mid - v3(width / 2f * SPACING, 0f, -startz);
				topleft.z -= z * (font.charheight + 1) * SPACING;
				for (int i = 0; i < line.Length; i++) {
					int c = line[i] - 32;
					vec3 pos = v3(topleft);
					for (int j = 0; j < font.charheight; j++) {
						int cw = font.charwidth[c];
						for (int k = 0; k < font.charwidth[c]; k++) {
							if (((font.chardata[c][j] >> k) & 1) == 1) {
								vec3 p = pos + v3(k * SPACING, 0f, 0f);
								p = turn(p, mid, qx);
								p = turn(p, mid, qy);
								p = turn(p, mid, qz);
								points[pointidx++] = p;
							}
						}
						pos.z -= SPACING;
					}
					topleft.x += (font.charwidth[c] + 1) * SPACING;
				}
			}


			vec3[] _points = new vec3[points.Length];
			Odot[] dots = new Odot[_points.Length];
			for (int i = 0; i < points.Length; i++) {
				dots[i] = new Odot(Sprite.SPRITE_SQUARE_2_2, 0);
			}

			TEXT text;
			text.points = points;
			text._points = _points;
			text.odots = dots;
			text.col = col;
			return text;
		}
	}
}
}
