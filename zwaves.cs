using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace osusb1 {
partial class all {
	class Zwaves : Z {

		vec3 mid = v3(0f, 50f, 100f);

		const int SIZE = 128;
		const int AMOUNT = SIZE * SIZE;
		const int SCALES = 256;
		const int ELEVATION = 900;
		const int DISTANCE = 2;

		const float MOVEMENTPERSECOND = 1.3f;

		vec3[] points;
		Odot[] dots;

		public Zwaves(int start, int stop) {
			this.start = start;
			this.stop = stop;
			points = new vec3[AMOUNT];
			dots = new Odot[AMOUNT];

			Random rand = new Random("emily<3".GetHashCode());
			float[,] noise = new float[SIZE,SIZE];
			for (int a = 0; a < SIZE; a++) {
				for (int b = 0; b < SIZE; b++) {
					noise[a,b] = rand.Next(ELEVATION);
				}
			}
			for (int a = 0; a < SIZE; a++) {
				for (int b = 0; b < SIZE; b++) {
					dots[a * SIZE + b] = new Odot();
					float y;
					y = turb(noise, a, b, SCALES);
					points[a * SIZE + b] = mid + v3((a - SIZE / 2) * DISTANCE, (b - SIZE / 2) * DISTANCE, y);
				}
			}
		}

		// basicallt https://lodev.org/cgtutor/randomnoise.html
		private float turb(float[,] noise, float x, float y, float size) {
			float initialsize = size;
			float val = 0f;
			while (size >= 1f) {
				val += interpnoise(noise, x / size, y / size);
				size /= 2f;
			}
			return val / initialsize;
		}

		private float interpnoise(float[,] noise, float x, float y) {
			float fx = x - (int) x;
			float fy = y - (int) y;
			int x1 = (int) x;
			int x2 = (x1 + 1) % SIZE;
			int y1 = (int) y;
			int y2 = (y1 + 1) % SIZE;
			float v1 = lerp(noise[x1, y1], noise[x2, y1], fx);
			float v2 = lerp(noise[x1, y2], noise[x2, y2], fx);
			return lerp(v1, v2, fy);
		}

		public override void draw(SCENE scene) {
			float offset = MOVEMENTPERSECOND * (scene.endtime - scene.starttime) / 1000f * scene.progress;
			float offsetx = 8 * sin(offset / 4f);
			offsetx = offset * .3f;
			float offsety = offset;
			while (offsetx < 0) offsetx += SIZE;
			while (offsety < 0) offsety += SIZE;
			for (int a = 0; a < SIZE; a++) {
				for (int b = 0; b < SIZE; b++) {
					int i = a * SIZE + b;
					vec3 point = v3(points[i].xy, zAtOffset(a, b, offsetx, offsety));
					point = turn(point, mid, quat(0f, rad(mousey), rad(mousex)));
					vec4 t = p.Project(point);
					vec4 col = v4(1f);
					//col.w = 1f - clamp(t.w, 0f, 90f) / 90f;
					dots[i].update(scene.time, col, t);
					dots[i].draw(scene.g);
				}
			}
		}

		private float zAtOffset(int a, int b, float offsetx, float offsety) {
			int x = (int) offsetx;
			int y = (int) offsety;
			float fx = offsetx - x;
			float fy = offsety - y;
			a += x;
			b += y;
			float[,] vals = new float[2,2];
			vals[0,0] = points[(a % SIZE) * SIZE + (b % SIZE)].z;
			vals[0,1] = points[(a % SIZE) * SIZE + (b + 1) % SIZE].z;
			vals[1,0] = points[((a + 1) % SIZE) * SIZE + (b % SIZE)].z;
			vals[1,1] = points[((a + 1) % SIZE) * SIZE + (b + 1) % SIZE].z;
			return interpnoise(vals, fx, fy);
		}

		public override void fin(Writer w) {
		}

	}
}
}
