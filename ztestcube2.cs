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
		Tri[] tris;
		
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

		Pixelscreen screen = new Pixelscreen(500, 400, 1);
		//Pixelscreen screen = new Pixelscreen(250, 175, 2);
		//Pixelscreen screen = new Pixelscreen(125, 100, 4);
		//Pixelscreen screen = new Pixelscreen(100, 75, 6);
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

			tris = new Tri[] {
				// F
				new Tri(Color.Cyan, _points, PC, PB, PA),
				new Tri(Color.Cyan, _points, PA, PD, PC),
				// L
				new Tri(Color.Lime, _points, PB, PF, PA),
				new Tri(Color.Lime, _points, PF, PE, PA),
				// R
				new Tri(Color.Red, _points, PG, PD, PH),
				new Tri(Color.Red, _points, PD, PG, PC),
				// B
				new Tri(Color.Blue, _points, PH, PE, PF),
				new Tri(Color.Blue, _points, PF, PG, PH),
				// U
				new Tri(Color.Yellow, _points, PD, PA, PE),
				new Tri(Color.Yellow, _points, PE, PH, PD),
				// D
				new Tri(Color.Orange, _points, PB, PC, PF),
				new Tri(Color.Orange, _points, PG, PF, PC),
			};
		}

		public override void draw(SCENE scene) {
			turn(_points, points, mid, 800f * scene.progress, 1200f * scene.progress);
			screen.clear();
			for (int i = 0; i < tris.Length; i++) {
				if (tris[i].shouldcull()) {
					//continue;
				}
				vec4[] t = tris[i].project(scene.projection);
				screen.tri(tris[i].color, t);
			}
			screen.draw(scene);
		}

		public override void fin(Writer w) {
			screen.fin(w);
		}

	}
}
}
