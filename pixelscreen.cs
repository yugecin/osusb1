using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;

namespace osusb1 {
partial class all {
	class Pixelscreen {

		int x, y, hpixels, vpixels, pixelsize, hpixeloffset, vpixeloffset;

		public object[,] owner;
		float[,] zbuf;
		Odot[,] odot;

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

		public object ownerAt(vec2 pos) {
			int x = (int) pos.x;
			int y = (int) pos.y;
			x -= this.x;
			y -= this.y;
			x /= pixelsize;
			y /= pixelsize;
			if (x < 0 || hpixels <= x || y < 0 || vpixels <= y) {
				return null;
			}
			return owner[x, y];
		}

		private void init() {
			this.zbuf = new float[hpixels,vpixels];
			this.owner = new object[hpixels,vpixels];
			this.odot = new Odot[hpixels,vpixels];
			this.hpixeloffset = this.x / pixelsize;
			this.vpixeloffset = this.y / pixelsize;
			for (int i = 0; i < hpixels; i++) {
				for (int j = 0; j < vpixels; j++) {
					odot[i,j] = new Odot(Sprite.SPRITE_SQUARE_6_6, 0);
				}
			}
		}

		public void clear() {
			for (int i = 0; i < zbuf.GetLength(0); i++) {
				for (int j = 0; j < zbuf.GetLength(1); j++) {
					owner[i,j] = null;
				}
			}
		}

		public void draw(SCENE scene) {
			for (int i = 0; i < hpixels; i++) {
				for (int j = 0; j < vpixels; j++) {
					if (owner[i, j] == null || !(owner[i,j] is Tri)) {
						odot[i,j].update(scene.time, null, null, 0f);
						odot[i,j].draw(scene.g);
						continue;
					}
					vec4 res = col(((Tri) owner[i, j]).color);
					vec4 pos = v4(x + i * pixelsize, y + j * pixelsize, 1f, 1f);
					odot[i,j].update(scene.time, res, pos);
					odot[i,j].draw(scene.g);
				}
			}
		}

		public void fin(Writer w) {
			for (int i = 0; i < zbuf.GetLength(0); i++) {
				for (int j = 0; j < zbuf.GetLength(1); j++) {
					odot[i,j].fin(w);
				}
			}
		}

		public void tri(object owner, vec4[] points) {
			if (points[0].z < 1 || points[1].z < 1 || points[2].z < 1) {
				return;
			}
			Array.Sort(points, sorter.instance);
			if (points[0].y == points[1].y) {
				toptri(owner, points[0], points[1], points[2]);
				return;
			}
			if (points[1].y == points[2].y) {
				bottri(owner, points[0], points[1], points[2]);
				return;
			}
			float perc = progress(points[0].y, points[2].y, points[1].y);
			vec4 phantom = (points[2] - points[0]) * perc + points[0];
			bottri(owner, points[0], phantom, points[1]);
			toptri(owner, phantom, points[1], points[2]);
		}

		private void toptri(object owner, vec4 p0, vec4 p1, vec4 p2) {
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

					float ypercleft = progress(p0.y, p2.y, realy);
					float xminbound = lerp(p0.x, p2.x, ypercleft);

					float ypercright = progress(p1.y, p2.y, realy);
					float xmaxbound = lerp(p1.x, p2.x, ypercright);

					if (realx < xminbound) {
						continue;
					}
					if (realx >= xmaxbound) {
						continue;
					}

					float xperc = progress(xminbound, xmaxbound, realx);

					float dist1 = lerp(p0.w, p2.w, ypercleft);
					float dist2 = lerp(p1.w, p2.w, ypercright);
					float realdist = lerp(dist1, dist2, xperc);

					/*
					if (realz < 1f) {
						continue;
					}
					*/
					if (this.owner[x, y] != null && zbuf[x, y] < realdist) {
						continue;
					}
					zbuf[x, y] = realdist;
					this.owner[x, y] = owner;
				}
			}
		}

		private void bottri(object owner, vec4 p0, vec4 p1, vec4 p2) {
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

					float ypercleft = progress(p0.y, p1.y, realy);
					float xminbound = lerp(p0.x, p1.x, ypercleft);

					float ypercright = progress(p0.y, p2.y, realy);
					float xmaxbound = lerp(p0.x, p2.x, ypercright);

					if (realx < xminbound) {
						continue;
					}
					if (realx >= xmaxbound) {
						continue;
					}

					float xperc = progress(xminbound, xmaxbound, realx);

					float dist1 = lerp(p0.w, p1.w, ypercleft);
					float dist2 = lerp(p0.w, p2.w, ypercright);
					float realdist = lerp(dist1, dist2, xperc);

					/*
					if (realz < 1f) {
						continue;
					}
					*/
					if (this.owner[x, y] != null && zbuf[x, y] < realdist) {
						continue;
					}
					zbuf[x, y] = realdist;
					this.owner[x, y] = owner;
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
