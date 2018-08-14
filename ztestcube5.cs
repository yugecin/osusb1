using System;
using System.Drawing;
using System.Text;

namespace osusb1 {
partial class all {
	class Ztestcube5 : Z {

		vec3 mid = v3(0f, 0f, 100f);

		vec3[] points;
		vec3[] _points;
		Cube cube;
		Oline[] lines;

		const int
			PA = 1,
			PB = 0,
			PC = 4,
			PD = 5,
			PE = 3,
			PF = 2,
			PG = 6,
			PH = 7;

		public Ztestcube5(int start, int stop) {
			this.start = start;
			this.stop = stop;
			framedelta = 100;

			points = new vec3[] {
				v3(-10f, -10f, 90f),
				v3(-10f, -10f, 110f),
				v3(-10f, 10f, 90f),
				v3(-10f, 10f, 110f),
				v3(10f, -10f, 90f),
				v3(10f, -10f, 110f),
				v3(10f, 10f, 90f),
				v3(10f, 10f, 110f),
			};

			_points = new vec3[points.Length];

			cube = new Cube(
				Color.Cyan,
				Color.Lime,
				Color.Red,
				Color.Blue,
				Color.Yellow,
				Color.Orange,
				_points,
				PA,
				PD,
				PC,
				PB,
				PF,
				PE,
				PH,
				PG
			);

			lines = new Oline[12];
			Rect[] rs = { cube.rects[Cube.F], cube.rects[Cube.B] };
			for (int i = 0; i < 2; i++) {
				lines[i * 4 + 0] = new Oline(rs[i].pts, rs[i].a, rs[i].b);
				lines[i * 4 + 1] = new Oline(rs[i].pts, rs[i].c, rs[i].d);
				lines[i * 4 + 2] = new Oline(rs[i].pts, rs[i].a, rs[i].c);
				lines[i * 4 + 3] = new Oline(rs[i].pts, rs[i].b, rs[i].d);
			}
			lines[8] = new Oline(rs[0].pts, rs[0].a, rs[1].b);
			lines[9] = new Oline(rs[0].pts, rs[0].b, rs[1].a);
			lines[10] = new Oline(rs[0].pts, rs[0].c, rs[1].d);
			lines[11] = new Oline(rs[0].pts, rs[0].d, rs[1].c);
		}

		public override void draw(SCENE scene) {
			ICommand.round_move_decimals.Push(5);
			turn(_points, points, mid, 800f * scene.progress, 1200f * scene.progress);

			foreach (Oline line in lines) {
				line.update(scene.time, v4(1f));
				line.draw(scene.g);
			}
			ICommand.round_move_decimals.Pop();
		}

		public override void fin(Writer w) {
			ICommand.round_move_decimals.Push(5);
			foreach (Oline line in lines) {
				line.fin(w);
			}
			ICommand.round_move_decimals.Pop();
		}

	}
}
}
