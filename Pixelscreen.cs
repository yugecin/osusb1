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
			P3D phantom;
			phantom.x = perc * (points[2].x - points[0].x) + points[0].x;
			phantom.y = points[1].y;
			phantom.z = perc * (points[2].z - points[0].z) + points[0].z;
			phantom.dist = perc * (points[2].dist - points[0].dist) + points[0].dist;
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

			P3D p0 = points[0];
			P3D p1 = points[1];
			P3D p2 = points[2];
			/*
			 0  1
			  \/
			  2 
			*/

			float minx = min(p0.x, p2.x);
			float miny = p0.y;
			float maxx = max(p1.x, p2.x);
			float maxy = p2.y;

			int p_minx = -this.x / pixelsize + (int) minx / pixelsize;
			int p_miny = -this.y / pixelsize + (int) miny / pixelsize;
			int p_maxx = -this.x / pixelsize + (int) maxx / pixelsize + 1;
			int p_maxy = -this.y / pixelsize + (int) maxy / pixelsize + 1;

			for (int y = p_miny; y < p_maxy; y++) {
				float realy = this.y * pixelsize + y * pixelsize + pixelsize / 2f;

				for (int x = p_minx; x < p_maxx; x++) {
					float realx = this.x * pixelsize + x * pixelsize + pixelsize / 2f;

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

					float z1 = (p2.dist - p0.dist) * ypercleft + p0.dist;
					float z2 = (p1.dist - p0.dist) * ypercright + p0.dist;

					float xperc = (realx - xminbound) / (xmaxbound - xminbound);
					float realz = (z2 - z1) * xperc + z1;

					if (realz < 1f) {
						continue;
					}
					if (result[x, y] != null) {
						if (zbuf[x, y] < realz) {
							continue;
						}
					}
					zbuf[x, y] = realz;
					result[x, y] = col;
				}
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

			P3D p0 = points[0];
			P3D p1 = points[1];
			P3D p2 = points[2];
			/*
			   0
			  /\
			  1 2 
			*/

			float minx = min(p0.x, p1.x);
			float miny = p0.y;
			float maxx = max(p0.x, p2.x);
			float maxy = p2.y;

			int p_minx = -this.x / pixelsize + (int) minx / pixelsize;
			int p_miny = -this.y / pixelsize + (int) miny / pixelsize;
			int p_maxx = -this.x / pixelsize + (int) maxx / pixelsize + 1;
			int p_maxy = -this.y / pixelsize + (int) maxy / pixelsize + 1;

			for (int y = p_miny; y < p_maxy; y++) {
				float realy = this.y * pixelsize + y * pixelsize + pixelsize / 2f;

				for (int x = p_minx; x < p_maxx; x++) {
					float realx = this.x * pixelsize + x * pixelsize + pixelsize / 2f;

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

					float z1 = (p1.dist - p0.dist) * ypercleft + p0.dist;
					float z2 = (p2.dist - p0.dist) * ypercright + p0.dist;

					float xperc = (realx - xminbound) / (xmaxbound - xminbound);
					float realz = (z2 - z1) * xperc + z1;

					if (realz < 1f) {
						continue;
					}
					if (result[x, y] != null) {
						if (zbuf[x, y] < realz) {
							continue;
						}
					}
					zbuf[x, y] = realz;
					result[x, y] = col;
				}
			}
		}

		private float min(float a, float b) {
			return (float) Math.Min(a, b);
		}

		private float max(float a, float b) {
			return (float) Math.Max(a, b);
		}

		private float min(float a, float b, float c) {
			return (float) Math.Min(Math.Min(a, b), c);
		}

		private float max(float a, float b, float c) {
			return (float) Math.Max(Math.Max(a, b), c);
		}

	}

	class sorter : IComparer<P3D> {
		public static sorter instance = new sorter();

		public int Compare(P3D a, P3D b) {
			return a.y.CompareTo(b.y);
		}
	}

}
