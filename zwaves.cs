using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace osusb1 {
partial class all {
	class Zwaves : Z {

		// see https://gpfault.net/posts/perlin-noise.txt.html

		const int PXSIZE = 2;
		Pixelscreen screen = new Pixelscreen(640 / PXSIZE, 480 / PXSIZE, PXSIZE);

		vec3 mid = v3(0f, 50f, 100f);

		const float DIMENSION = 200f;
		const int SIZE = 64;
		const int RESOLUTION = SIZE * 2 * 2;
		const int AMOUNT = SIZE * SIZE;
		const int SCALES = 256;
		const int ELEVATION = 50;
		const int DISTANCE = 2;

		const float SPEEDMOD = 0.001f;

		Odot[] dots;

		vec3[,] rand;

		public Zwaves(int start, int stop) {
			this.start = start;
			this.stop = stop;
			rand = new vec3[RESOLUTION, RESOLUTION];
			dots = new Odot[AMOUNT];

			Random r = new Random("emily<3".GetHashCode());
			for (int a = 0; a < RESOLUTION; a++) {
				for (int b = 0; b < RESOLUTION; b++) {
					rand[a, b] = v3(r.Next(11) - 5, r.Next(11) - 5, r.Next(11) - 5).norm();
				}
			}
			for (int i = 0; i < AMOUNT; i++) {
				dots[i] = new Odot();
			}
		}

		private vec3 grad(vec3 position) {
			int a = (int) (position.x + position.z);
			int b = (int) (position.y - position.z);
			while (b < 0) b += RESOLUTION;
			return rand[a % RESOLUTION, b % RESOLUTION];
		}

		private float fade(float t) {
			return t * t * t * (t * (t * 6f - 15f) + 10f);
		}

		private float noise(vec3 p) {
			vec3 p0 = floor(p);
			vec3 p1 = p0 + v3(1f, 0f, 0f);
			vec3 p2 = p0 + v3(0f, 1f, 0f);
			vec3 p3 = p0 + v3(1f, 1f, 0f);
			vec3 p4 = p0 + v3(0f, 0f, 1f);
			vec3 p5 = p4 + v3(1f, 0f, 0f);
			vec3 p6 = p4 + v3(0f, 1f, 0f);
			vec3 p7 = p4 + v3(1f, 1f, 0f);
			
			vec3 g0 = grad(p0);
			vec3 g1 = grad(p1);
			vec3 g2 = grad(p2);
			vec3 g3 = grad(p3);
			vec3 g4 = grad(p4);
			vec3 g5 = grad(p5);
			vec3 g6 = grad(p6);
			vec3 g7 = grad(p7);

			float fade_t0 = fade(p.x - p0.x);
			float fade_t1 = fade(p.y - p0.y);
			float fade_t2 = fade(p.z - p0.z);

			float p0p1 = lerp(dot(g0, p - p0), dot(g1, p - p1), fade_t0);
			float p2p3 = lerp(dot(g2, p - p2), dot(g3, p - p3), fade_t0);

			float p4p5 = lerp(dot(g4, p - p4), dot(g5, p - p5), fade_t0);
			float p6p7 = lerp(dot(g6, p - p6), dot(g7, p - p7), fade_t0);

			float y1 = lerp(p0p1, p2p3, fade_t1);
			float y2 = lerp(p4p5, p6p7, fade_t1);

			return lerp(y1, y2, fade_t2);
		}

		private vec3 calc(vec3 position) {
			float x = position.x * RESOLUTION;
			float y = position.y * RESOLUTION;
			float z = 0f;
			z += noise(v3(x, y, position.z * 30f) / 128f);
			z += noise(v3(x, y, position.z * 30f) / 64f) / 2f;
			z += noise(v3(x, y, position.z * 64f) / 32f) / 16f;
			z = mid.z + lerp(lerp(1f, 0.2f, z + 1f), 0.1f, z * 0.5f + 0.5f) * ELEVATION;
			//z = mid.z + z * ELEVATION;
			x = (position.x - 0.5f) * DIMENSION;
			y = (position.y - 0.5f) * DIMENSION;
			return v3(x, y, z);
		}

		public override void draw(SCENE scene) {
			vec3[] points = new vec3[AMOUNT];
			for (int a = 0; a < SIZE; a++) {
				for (int b = 0; b < SIZE; b++) {
					int i = a * SIZE + b;
					vec3 point = calc(v3((float) a / SIZE, (float) b / SIZE, scene.time * SPEEDMOD));
					point = turn(point, mid, quat(0f, rad(mousey), rad(mousex)));
					points[i] = point;
				}
			}
			screen.clear();
			for (int a = 0; a < SIZE; a++) {
				for (int b = 0; b < SIZE; b++) {
					int i = a * SIZE + b;
					vec4 t = p.Project(points[i]);
					vec4 col = v4(1f);
					col.w = 1f - clamp(t.w, 0f, 90f) / 90f;

					dots[i].update(scene.time, col, t);
					dots[i].draw(scene.g);

					if (a == SIZE - 1 || b == SIZE - 1) {
						continue;
					}
					int aa = a * SIZE + b;
					int bb = (a + 1) * SIZE + b;
					int cc = a * SIZE + b + 1;
					int dd = (a + 1) * SIZE + b + 1;
					Rect r = new Rect(this, col.col(), points, aa, bb, cc, dd);
					if (r.shouldcull()) {
						continue;
					}
					col = v4(.5f, .68f, .98f, 1f);
					col *= .5f + .5f * (r.surfacenorm().norm() ^ r.rayvec().norm());
					r.setColor(col.col());
					r.draw(screen);
				}
			}
			screen.draw(scene);
		}

		public override void fin(Writer w) {
		}

	}
}
}
