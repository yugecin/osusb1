using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace osusb1 {
	class Pixelscreen {

		int x, y, hpixels, vpixels, pixelsize;

		Color?[,] result;
		float[,] zbuf;

		public Pixelscreen(int x, int y, int hpixels, int vpixels, int pixelsize) {
			this.x = x;
			this.y = y;
			this.hpixels = hpixels;
			this.vpixels = vpixels;
			this.pixelsize = pixelsize;
			this.zbuf = new float[hpixels,vpixels];
			this.result = new Color?[hpixels,vpixels];
		}

		public Pixelscreen(int hpixels, int vpixels, int pixelsize) {
			this.x = 320 - hpixels * pixelsize / 2;
			this.y = 240 - vpixels * pixelsize / 2;
			this.hpixels = hpixels;
			this.vpixels = vpixels;
			this.pixelsize = pixelsize;
			this.zbuf = new float[hpixels,vpixels];
			this.result = new Color?[hpixels,vpixels];
		}

		public void clear() {
			for (int i = 0; i < zbuf.GetLength(0); i++) {
				for (int j = 0; j < zbuf.GetLength(1); j++) {
					zbuf[i,j] = float.MaxValue;
					result[i,j] = null;
				}
			}
		}

		public void draw(Graphics g) {
			for (int i = 0; i < hpixels; i++) {
				for (int j = 0; j < vpixels; j++) {
					if (result[i, j] != null) {
						Color res = (Color) result[i, j];
						g.FillRectangle(new SolidBrush(res), x + i * pixelsize, y + j * pixelsize, pixelsize, pixelsize);
					}
				}
			}
		}

		public void tri(Color col, P3D[] points) {
			Array.Sort(points, sorter.instance);
			if (points[0].y == points[1].y) {
				toptri(col, points);
				return;
			}
			if (points[1].y == points[2].y) {
				bottri(col, points);
				return;
			}
			float perc = (points[1].y - points[0].y) / (float) (points[2].y - points[0].y);
			P3D phantom = new P3D(perc * (points[2].x - points[0].x) + points[0].x, points[1].y, perc * (points[2].z - points[0].z));
			bottri(col, new P3D[] { points[0], phantom, points[1]});
			toptri(col, new P3D[] { phantom, points[1], points[2]});
		}

		private void toptri(Color col, P3D[] points) {
			if (points[1].x < points[0].x) {
				P3D _ = points[1];
				points[1] = points[0];
				points[0] = _;
			}
			if (points[0].y - points[2].y == 0) {
				return;
			}

			foreach (P3D p in points) {
				int xpixel = (int) p.x / pixelsize - this.x / pixelsize;
				int ypixel = (int) p.y / pixelsize - this.y / pixelsize;
				if (xpixel > 0 && xpixel < hpixels && ypixel > 0 && ypixel < vpixels) {
					result[xpixel, ypixel] = col;
				}
			}
			P3D p0 = points[0];
			P3D p1 = points[1];
			P3D p2 = points[2];

			int starty = (int) p0.y / pixelsize;
			int maxy = (int) p2.y;
			if (maxy % pixelsize == pixelsize / 2) {
				maxy -= 1;
			}

			int y = starty * pixelsize + pixelsize / 2;
			if (p0.y > y) {
				y += pixelsize;
			}

			for (;;) {
				if (y > maxy) {
					break;
				}
				int ypixel = (int) y / pixelsize;
				float yperc = (y - p0.y) / (p2.y - p0.y);
				float xstart = (p2.x - p0.x) * yperc + p0.x;
				int startx = (int) (xstart) / pixelsize;
				float xend = (p2.x - p1.x) * yperc + p1.x;
				int maxx = (int) (xend);
				float z1 = (p2.z - p0.z) * yperc + p0.z;
				float z2 = (p2.z - p1.z) * yperc + p1.z;

				if (maxx % pixelsize == pixelsize / 2) {
					maxx -= 1;
				}
				int x = startx * pixelsize + pixelsize / 2;
				if (xstart > x) {
					x += pixelsize;
				}

				for (;;) {
					if (x > maxx) {
						break;
					}

					int xpixel = (int) x / pixelsize;
					float xperc = (x - xstart) / (xend - xstart);
					float zz = (z2 - z1) * xperc + z1;

					if (zz < 1) {
						goto cont;
					}
					if (result[xpixel, ypixel] != null) {
						if (zz < zbuf[xpixel, ypixel]) {
							goto cont;
						}
						zbuf[xpixel, ypixel] = zz;
					}
					result[xpixel, ypixel] = col;
				cont:
					x += pixelsize;
				}

				y += pixelsize;
			}
		}

		private void bottri(Color col, P3D[] points) {
			if (points[2].x < points[1].x) {
				P3D _ = points[2];
				points[2] = points[1];
				points[1] = _;
			}
			if (points[0].y - points[2].y == 0) {
				return;
			}

		}

	}

	class sorter : IComparer<P3D> {
		public static sorter instance = new sorter();

		public int Compare(P3D a, P3D b) {
			return a.y.CompareTo(b.y);
		}
	}

}
