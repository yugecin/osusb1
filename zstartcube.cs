using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace osusb1 {
partial class all {
	class Zstartcube : Z {

		vec3 mid = v3(0f, -30f, 100f);

		vec3[] points;
		vec3[] _points;
		Cube cube;
		
		/*

		  E+--------+H
		  /|       /|
		A+--------+D|
		 | |      | |
		 |F+------|-+G
		 |/       |/
		B+--------+C

		*/

		const int
			PA = 1,
			PB = 0,
			PC = 4,
			PD = 5,
			PE = 3,
			PF = 2,
			PG = 6,
			PH = 7;

		Pixelscreen screen12 = new Pixelscreen(640 / 8, 480 / 8, 8);
		Pixelscreen screen6 = new Pixelscreen(640 / 6, 480 / 6, 6);

		vec3 basecol = v3(.8f);

		public Zstartcube(int start, int stop) {
			this.start = start;
			this.stop = stop;
			framedelta = 100;

			points = new vec3[] {
				v3(-10f, -10f, -10f),
				v3(-10f, -10f, 10f),
				v3(-10f, 10f, -10f),
				v3(-10f, 10f, 10f),
				v3(10f, -10f, -10f),
				v3(10f, -10f, 10f),
				v3(10f, 10f, -10f),
				v3(10f, 10f, 10f),
			};

			move(points, mid);
			_points = new vec3[points.Length];

			Color col = basecol.col();
			cube = new Cube(
				col,
				col,
				col,
				col,
				col,
				col,
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
		}

		public override void draw(SCENE scene) {
			turn(_points, points, mid, 800f * scene.progress, 1200f * scene.progress);
			foreach (Rect r in cube.rects) {
				if (!r.shouldcull()) {
					float rv = (r.surfacenorm().norm() ^ r.rayvec().norm());
					r.setColor((basecol * (.3f + .7f * rv)).col());
				}
			}
			screen12.clear();
			screen6.clear();
			cube.draw(screen12);
			cube.draw(screen6);
			//screen12.draw(scene);
			screen6.draw(scene);
		}

		public override void fin(Writer w) {
			screen12.fin(w);
			screen6.fin(w);
		}

	}
}
}
