using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace osusb1 {
partial class all {
	class Ztestcube2 : Z {

		vec3 mid = v3(0f, 0f, 100f);

		vec3[] points;
		vec3[] _points;
		Rect[] rects;
		
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

		//Pixelscreen screen = new Pixelscreen(500, 400, 1);
		//Pixelscreen screen = new Pixelscreen(250, 175, 2);
		//Pixelscreen screen = new Pixelscreen(125, 100, 4);
		Pixelscreen screen = new Pixelscreen(100, 75, 6);
		//Pixelscreen screen = new Pixelscreen(14, 12, 8);

		public Ztestcube2(int start, int stop) {
			this.start = start;
			this.stop = stop;

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

			rects = new Rect[] {
				new Rect(Color.Cyan, _points, PA, PD, PB, PC), // F
				new Rect(Color.Lime, _points, PE, PA, PF, PB), // L
				new Rect(Color.Red, _points, PD, PH, PC, PG), // R
				new Rect(Color.Blue, _points, PH, PE, PG, PF), // B
				new Rect(Color.Yellow, _points, PE, PH, PA, PD), // U
				new Rect(Color.Orange, _points, PB, PC, PF, PG), // D
			};
		}

		public override void draw(SCENE scene) {
			turn(_points, points, mid, 800f * scene.progress, 1200f * scene.progress);
			screen.clear();
			foreach (Rect r in rects) {
				if (r.shouldcull()) {
					continue;
				}
				screen.tri(r.color, r.tri1.project(scene.projection));
				screen.tri(r.color, r.tri2.project(scene.projection));
			}
			screen.draw(scene);
		}

		public override void fin(Writer w) {
			screen.fin(w);
		}

	}
}
}
