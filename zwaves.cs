﻿//#define FILL
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace osusb1 {
partial class all {
	class Zwaves : Z {

		// see https://gpfault.net/posts/perlin-noise.txt.html

		const int PXSIZE = 6;
		Pixelscreen screen = new Pixelscreen(750 / PXSIZE, 480 / PXSIZE, PXSIZE);


		const int SIZEMP = 2;
		const float DIMENSION = 400f * SIZEMP * 1.4f;
		const int SIZE = 64 * SIZEMP;
		const int RESOLUTION = SIZE * 2 * SIZEMP;
		const int AMOUNT = SIZE * SIZE;
		const int ELEVATION = 50;

		const float SPEEDMOD = 0.001f;

		Odot[] dots;
		vec3[,] points;
		vec3[,] rand;

		public Zwaves(int start, int stop) {
			this.start = start;
			this.stop = stop;
			framedelta = 200;

			rand = new vec3[RESOLUTION, RESOLUTION];
			points = new vec3[SIZE, SIZE];
			dots = new Odot[AMOUNT];

			Random r = new Random("Emily<3".GetHashCode());
			for (int a = 0; a < RESOLUTION; a++) {
				for (int b = 0; b < RESOLUTION; b++) {
					rand[a, b] = v3(r.Next(11) - 5, r.Next(11) - 5, r.Next(11) - 5).norm();
				}
			}
			for (int a = 0; a < SIZE; a++) {
				for (int b = 0; b < SIZE; b++) {
					points[a, b] = calc(v3((float) a / SIZE, (float) b / SIZE, 1));
					int s = Sprite.INTERPOLATE_MOVE;
					s |= Sprite.EASE_FADE;
					s |= Sprite.EASE_SCALE;
					s |= Sprite.SESDSM;
					dots[a * SIZE + b] = new Odot(Sprite.SPRITE_DOT_6_12, s);
				}
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
			z += noise(v3(x, y, position.z * 64f) / 16f) / 24f;
			//z = lerp(lerp(1f, 0.2f, z + 1f), 0.1f, z * 0.5f + 0.5f) * ELEVATION;
			z = z * ELEVATION / 2f;
			x = (position.x - 0.5f) * DIMENSION;
			y = (position.y - 0.5f) * DIMENSION;
			return v3(x, y, z);
		}

		private float heightat(float x, float y, int o) {
			float offset = (float) o / 35000f;
			float h = 0f;
			h += heightat(x + offset / 1, y + offset / 1) / 1;
			h += heightat(x + offset / 2, y + offset / 2) / 2;
			/*
			h += heightat(x + offset / 4, y + offset / 4) / 2;
			h += heightat(x + offset / 8, y + offset / 8) / 4;
			*/
			return h;
		}

		private float heightat(float a, float b) {
			while (a < 0f) a += 1f;
			while (b < 0f) b += 1f;
			float x = a * SIZE;
			float y = b * SIZE;
			int x1 = ((int) x) % SIZE;
			int y1 = ((int) y) % SIZE;
			int x2 = (x1 + 1) % SIZE;
			int y2 = (y1 + 1) % SIZE;
			float px = x - (int) x;
			float xx1 = lerp(points[x1, y1].z, points[x2, y1].z, px);
			float xx2 = lerp(points[x1, y2].z, points[x2, y2].z, px);
			return lerp(xx1, xx2, y - (int) y);
		}

		public override void draw(SCENE scene) {
			ICommand.round_scale_decimals.Push(1);

			vec3[] points = new vec3[AMOUNT];

			vec3 posoffset = v3(0f, 0f, 0f);
			vec3 mid = v3(Zsc.mid);

			posoffset.xy -= v2(DIMENSION / 2.3f);
			posoffset.x += 40;
			posoffset.y -= 10;
			posoffset.xy -= (-30 + udata[0]) / 100f * DIMENSION / 2f;
			posoffset.xy += DIMENSION / 9.5f * scene.progress;
			posoffset.z += 30f;

			float angle = 10f;

			float rot = scene.reltime / 80f;
			rot = 45;// + sin(rad(rot)) * 10f;
			//rot = 225;

			for (int a = 0; a < SIZE; a++) {
				for (int b = 0; b < SIZE; b++) {
					int i = a * SIZE + b;
					vec3 point = v3(this.points[a, b]);
					point.z = heightat((float) a / SIZE, (float) b / SIZE, scene.reltime);
					point.z += mid.z;
					point.z -= 2f;
					point -= posoffset;
					point = turn(point, mid, quat(0f, 0, rad(rot)));
					point = turn(point, mid, quat(0f, rad(angle), 0));
					points[i] = point;
				}
			}
			Zsc.adjust(points);

			screen.clear();
			for (int a = 0; a < SIZE; a++) {
				for (int b = 0; b < SIZE; b++) {
					int i = a * SIZE + b;
					vec4 pos = project(points[i]);
					vec4 col = v4(1f);
					const float VIEWDIST = 200f;
					const float FADEDIST = 100f;
					float fadedist = (points[i] - Zsc.mid).length();
					col.w = 1f - clampx(fadedist, FADEDIST, VIEWDIST) / FADEDIST;
					col.w *= clamp(scene.reltime, 0f, 1500f) / 1500f;
					col.w *= 1f - clampx(scene.time, stop - 1000, stop) / 1000f;
					float size = col.w * 6f;

					dots[i].update(scene.time, col, pos, size);
					dots[i].draw(scene.g);

#if FILL
					if (a == SIZE - 1 || b == SIZE - 1) {
						continue;
					}
					int aa = a * SIZE + b;
					int bb = (a + 1) * SIZE + b;
					int cc = a * SIZE + b + 1;
					int dd = (a + 1) * SIZE + b + 1;
					Rect r = new Rect(this, col.col(), points, bb, aa, dd, cc);
					if (r.shouldcull()) {
						continue;
					}
					col = v4(.5f, .68f, .98f, 1f);
					col.xyz *= .5f + .5f * (r.surfacenorm().norm() ^ r.rayvec().norm());
					r.setColor(col.col());
					r.draw(screen);
#endif
				}
			}
			screen.draw(scene);

			Odot d = new Odot(Sprite.SPRITE_DOT_6_12, 0);
			d.update(scene.time, v4(1f, 0f, 1f, 1f), project(Zsc.mid), 3f);
			d.draw(scene.g);
			ICommand.round_scale_decimals.Pop();
		}

		public override void fin(Writer w) {
			ICommand.round_scale_decimals.Push(1);
			screen.fin(w);
			foreach (Odot o in dots) {
				o.fin(w);
			}
			ICommand.round_scale_decimals.Pop();
		}

	}
}
}
