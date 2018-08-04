using System;
using System.Drawing;
using System.Text;

namespace osusb1 {
partial class all {
	class Ztestcube4 : Z {

		vec3 mid = v3(0f, 0f, 100f);

		vec3[] points;
		vec3[] _points;
		Cube cube;
		Orect[] rects;
		Odot light1;
		Odot light2;
		Odot light3;

		const int
			PA = 1,
			PB = 0,
			PC = 4,
			PD = 5,
			PE = 3,
			PF = 2,
			PG = 6,
			PH = 7;

		public Ztestcube4(int start, int stop) {
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

			rects = new Orect[6];
			for (int i = 0; i < rects.Length; i++) {
				rects[i] = new Orect(cube.rects[i], 0);
				cube.rects[i].setColor(Color.White);
			}

			light1 = new Odot(Sprite.SPRITE_DOT_6_12, 0);
			light2 = new Odot(Sprite.SPRITE_DOT_6_12, 0);
			light3 = new Odot(Sprite.SPRITE_DOT_6_12, 0);
		}

		public override void draw(SCENE scene) {
			turn(_points, points, mid, 800f * scene.progress, 1200f * scene.progress);

			float ang = scene.progress * 20f;
			vec3 light1pos = mid + v3(30f * sin(ang), 20f * cos(ang), 20f);
			vec3 light1col = v3(1f, 0f, 0f);
			light1.update(scene.time, v4(light1col, 1f), p.Project(light1pos), 12f);
			light1.draw(scene.g);

			vec3 light2pos = mid + v3(30f * sin(ang * 1.7f), 0f, 20f * cos(ang * 1.7f));
			light2pos = turn(light2pos, mid, quat(rad(45f), rad(0f), 0f));
			vec3 light2col = v3(0f, 0f, 1f);
			light2.update(scene.time, v4(light2col, 1f), p.Project(light2pos), 12f);
			light2.draw(scene.g);

			vec3 light3pos = mid + v3(30f * sin(ang * 0.5f), 0f, 20f * cos(ang * 0.5f));
			light3pos = turn(light3pos, mid, quat(0f, rad(-45f), 0f));
			vec3 light3col = v3(0f, .4f, 0f);
			light3.update(scene.time, v4(light3col, 1f), p.Project(light3pos), 12f);
			light3.draw(scene.g);

			for (int i = 0; i < 6; i++) {
				Rect r = cube.rects[i];
				Orect o = rects[i];

				vec3 rectmid = v3(0f);
				rectmid += r.pts[r.a];
				rectmid += r.pts[r.b];
				rectmid += r.pts[r.c];
				rectmid += r.pts[r.d];
				rectmid /= 4f;


				float aval = 0.3f;
				float bval = 0.7f;

				vec3 result = col(r.color).xyz * aval;
				result += light1col * bval / pow(distance(light1pos, rectmid) / 15, 2);
				result += light2col * bval / pow(distance(light2pos, rectmid) / 15, 2);
				result += light3col * bval / pow(distance(light3pos, rectmid) / 15, 2);

				result.x = min(1f, result.x);
				result.y = min(1f, result.y);
				result.z = min(1f, result.z);

				Color original = r.color;
				r.setColor(v4(result, 1f).col());
				o.update(scene);
				r.setColor(original);
			}
		}

		public override void fin(Writer w) {
			foreach (Orect r in rects) {
				r.fin(w);
			}
			light1.fin(w);
			light2.fin(w);
			light3.fin(w);
		}

	}
}
}
