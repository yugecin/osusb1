using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace osusb1 {
	class Ztestcube2 : Z {

		P3D mid = new P3D(0f, 0f, 100f);

		P3D[] points;
		P3D[] _points;
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
		//Pixelscreen screen = new Pixelscreen(100, 75, 5);

		public Ztestcube2(int start, int stop) {
			this.start = start;
			this.stop = stop;

			points = new P3D[] {
				new P3D(-10f, -10f, 90f),
				new P3D(-10f, -10f, 110f),
				new P3D(-10f, 10f, 90f),
				new P3D(-10f, 10f, 110f),
				new P3D(10f, -10f, 90f),
				new P3D(10f, -10f, 110f),
				new P3D(10f, 10f, 90f),
				new P3D(10f, 10f, 110f),
			};

			_points = new P3D[points.Length];

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
			Ang.turn(_points, points, mid, 800f * scene.progress, 1200f * scene.progress);
			screen.clear();
			for (int i = 0; i < tris.Length; i++) {
				if (tris[i].shouldcull(scene.projection)) {
					continue;
				}
				Tri t = tris[i].project(scene.projection);
				screen.tri(t.color, t.getpoints());
			}
			if (scene.g != null) {
				screen.draw(scene.g);
			}
			foreach (P3D p in _points) {
				P3D _p = scene.projection.Project(p);
				scene.g.FillRectangle(new SolidBrush(Color.Green), _p.x - 2, _p.y - 2, 4, 4);
			}
		}

		public override void fin(Writer w) {
		}

	}
}
