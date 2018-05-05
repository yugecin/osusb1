﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace osusb1 {
partial class all {
	class Pixelscreen {

		int x, y, hpixels, vpixels, pixelsize, hpixeloffset, vpixeloffset;

		Color?[,] result;
		float[,] zbuf;
		Spixelscreendot[,] sdot;

		public Pixelscreen(int x, int y, int hpixels, int vpixels, int pixelsize) {
			this.x = x;
			this.y = y;
			this.hpixels = hpixels;
			this.vpixels = vpixels;
			this.pixelsize = pixelsize;
			init();
		}

		public Pixelscreen(int hpixels, int vpixels, int pixelsize) {
			this.x = 320 - hpixels * pixelsize / 2;
			this.y = 240 - vpixels * pixelsize / 2;
			this.hpixels = hpixels;
			this.vpixels = vpixels;
			this.pixelsize = pixelsize;
			init();
		}

		private void init() {
			this.zbuf = new float[hpixels,vpixels];
			this.result = new Color?[hpixels,vpixels];
			this.sdot = new Spixelscreendot[hpixels,vpixels];
			this.hpixeloffset = this.x / pixelsize;
			this.vpixeloffset = this.y / pixelsize;
		}

		public void clear() {
			for (int i = 0; i < zbuf.GetLength(0); i++) {
				for (int j = 0; j < zbuf.GetLength(1); j++) {
					result[i,j] = null;
				}
			}
		}

		public void draw(SCENE scene) {
			for (int i = 0; i < hpixels; i++) {
				for (int j = 0; j < vpixels; j++) {
					if (result[i, j] == null) {
						if (sdot[i, j] != null) {
							sdot[i,j].hide(scene.time);
						}
						continue;
					}
					Color res = (Color) result[i, j];
					if (scene.g != null) {
						scene.g.FillRectangle(
							new SolidBrush(res),
							x + i * pixelsize,
							y + j * pixelsize,
							pixelsize,
							pixelsize
						);
						continue;
					}
					if (sdot[i, j] != null) {
						sdot[i,j].update(scene.time, res);
						continue;
					}
					sdot[i, j] = new Spixelscreendot(
						x: hpixeloffset + i * pixelsize,
						y: vpixeloffset + j * pixelsize,
						time: scene.time,
						color: res
					);
				}
			}
		}

		public void fin(Writer w) {
			for (int i = 0; i < zbuf.GetLength(0); i++) {
				for (int j = 0; j < zbuf.GetLength(1); j++) {
					if (sdot[i, j] != null) {
						sdot[i, j].fin(w);
					}
				}
			}
		}

		public void tri(Color col, vec4[] points) {
			Array.Sort(points, sorter.instance);
			if (points[0].y == points[1].y) {
				toptri(col, points[0], points[1], points[2]);
				return;
			}
			if (points[1].y == points[2].y) {
				bottri(col, points[0], points[1], points[2]);
				return;
			}
			float perc = (points[1].y - points[0].y) / (float) (points[2].y - points[0].y);
			vec4 phantom = v4();
			phantom.x = perc * (points[2].x - points[0].x) + points[0].x;
			phantom.y = points[1].y;
			phantom.z = perc * (points[2].z - points[0].z) + points[0].z;
			bottri(col, points[0], phantom, points[1]);
			toptri(col, phantom, points[1], points[2]);
		}

		private void toptri(Color col, vec4 p0, vec4 p1, vec4 p2) {
			if (p1.x < p0.x) {
				vec4 _ = p1;
				p1 = p0;
				p0 = _;
			}
			if (p0.y - p2.y == 0) {
				return;
			}

			/*
			 0  1
			  \/
			  2 
			*/

			float minx = min(p0.x, p2.x);
			float miny = p0.y;
			float maxx = max(p1.x, p2.x);
			float maxy = p2.y;

			int p_minx = -hpixeloffset + (int) minx / pixelsize;
			int p_miny = -vpixeloffset + (int) miny / pixelsize;
			int p_maxx = -hpixeloffset + (int) maxx / pixelsize + 1;
			int p_maxy = -vpixeloffset + (int) maxy / pixelsize + 1;

			p_miny = max(0, min(p_miny, vpixels - 1));
			p_minx = max(0, min(p_minx, hpixels - 1));
			p_maxy = max(0, min(p_maxy, vpixels));
			p_maxx = max(0, min(p_maxx, hpixels));
			for (int y = p_miny; y < p_maxy; y++) {
				float realy = this.y + y * pixelsize + pixelsize / 2f;

				for (int x = p_minx; x < p_maxx; x++) {
					float realx = this.x + x * pixelsize + pixelsize / 2f;

					if (realy < p0.y) {
						continue;
					}
					if (realy >= p2.y) {
						continue;
					}

					float ypercleft = (realy - p0.y) / (p2.y - p0.y);
					float xminbound = (p2.x - p0.x) * ypercleft + p0.x;

					float ypercright = (realy - p1.y) / (p2.y - p1.y);
					float xmaxbound = (p2.x - p1.x) * ypercright + p1.x;

					if (realx < xminbound) {
						continue;
					}
					if (realx >= xmaxbound) {
						continue;
					}

					float xperc = (realx - xminbound) / (xmaxbound - xminbound);

					float dist1 = (p2.w - p0.w) * ypercleft + p0.w;
					float dist2 = (p1.w - p0.w) * ypercright + p0.w;
					float realdist = (dist2 - dist1) * xperc + dist1;

					/*
					if (realz < 1f) {
						continue;
					}
					*/
					if (result[x, y] != null && zbuf[x, y] < realdist) {
						continue;
					}
					zbuf[x, y] = realdist;
					result[x, y] = col;
				}
			}
		}

		private void bottri(Color col, vec4 p0, vec4 p1, vec4 p2) {
			if (p2.x < p1.x) {
				vec4 _ = p2;
				p2 = p1;
				p1 = _;
			}
			if (p0.y - p2.y == 0) {
				return;
			}

			/*
			   0
			  /\
			  1 2 
			*/

			float minx = min(p0.x, p1.x);
			float miny = p0.y;
			float maxx = max(p0.x, p2.x);
			float maxy = p2.y;

			int p_minx = -hpixeloffset + (int) minx / pixelsize;
			int p_miny = -vpixeloffset + (int) miny / pixelsize;
			int p_maxx = -hpixeloffset + (int) maxx / pixelsize + 1;
			int p_maxy = -vpixeloffset + (int) maxy / pixelsize + 1;

			p_miny = max(0, min(p_miny, vpixels - 1));
			p_minx = max(0, min(p_minx, hpixels - 1));
			p_maxy = max(0, min(p_maxy, vpixels));
			p_maxx = max(0, min(p_maxx, hpixels));
			for (int y = p_miny; y < p_maxy; y++) {
				float realy = this.y + y * pixelsize + pixelsize / 2f;

				for (int x = p_minx; x < p_maxx; x++) {
					float realx = this.x + x * pixelsize + pixelsize / 2f;

					if (realy <= p0.y) {
						continue;
					}
					if (realy >= p2.y) {
						continue;
					}

					float ypercleft = (realy - p0.y) / (p1.y - p0.y);
					float xminbound = (p1.x - p0.x) * ypercleft + p0.x;

					float ypercright = (realy - p0.y) / (p2.y - p0.y);
					float xmaxbound = (p2.x - p0.x) * ypercright + p0.x;

					if (realx < xminbound) {
						continue;
					}
					if (realx >= xmaxbound) {
						continue;
					}

					float xperc = (realx - xminbound) / (xmaxbound - xminbound);

					float dist1 = (p1.w - p0.w) * ypercleft + p0.w;
					float dist2 = (p2.w - p0.w) * ypercright + p0.w;
					float realdist = (dist2 - dist1) * xperc + dist1;

					/*
					if (realz < 1f) {
						continue;
					}
					*/
					if (result[x, y] != null && zbuf[x, y] < realdist) {
						continue;
					}
					zbuf[x, y] = realdist;
					result[x, y] = col;
				}
			}
		}
	}

	class sorter : IComparer<vec4> {
		public static sorter instance = new sorter();

		public int Compare(vec4 a, vec4 b) {
			return a.y.CompareTo(b.y);
		}
	}

}
}
