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
			P3D phantom = new P3D(perc * (points[2].x - points[0].x), points[1].y, perc * (points[2].z - points[0].z));
			bottri(col, new P3D[] { points[0], phantom, points[1]});
			toptri(col, new P3D[] { points[1], points[2], phantom});
		}

		private void toptri(Color col, P3D[] points) {
			if (points[1].x < points[0].x) {
				P3D _ = points[1];
				points[1] = points[0];
				points[0] = _;
			}

			//Console.WriteLine("{0} {1} {2} / {3} {4} {5} / {6} {7} {8}", points[0].x, points[0].y, points[0].z, points[1].x, points[1].y, points[1].z, points[2].x, points[2].y, points[2].z);
			int ystart = (int) points[0].y / pixelsize;
			int ystop = (int) points[2].y / pixelsize + 1;
			for (int y = ystart; y < ystop + 1; y++) {
				int pixelscreeny = y - this.y / pixelsize;

				if (y == ystart && points[0].y - ystart * pixelsize < pixelsize / 2) {
					continue;
				}
				if (y == ystop && points[2].y - ystop * pixelsize >= pixelsize / 2) {
					break;
				}

				float percy = 0;
				if (points[2].y - points[0].y != 0) {
					percy = (y * pixelsize + pixelsize / 2 - points[0].y) / (points[2].y - points[0].y);
				}
				if (percy < 0 || 1 < percy) {
					Console.WriteLine("percy is {0}", percy);
				}
				float startx = (points[2].x - points[0].x) * percy + points[0].x;
				float stopx = (points[2].x - points[1].x) * percy + points[1].x;
				int xstart = (int) startx / pixelsize;
				int xstop = (int) stopx / pixelsize + 1;

				for (int x = xstart; x < xstop + 1; x++) {
					int pixelscreenx = x - this.x / pixelsize;

					if (x == xstart && startx - xstart * pixelsize < pixelsize / 2) {
						continue;
					}
					if (x == xstop && stopx - xstop * pixelsize >= pixelsize / 2) {
						break;
					}

					if (pixelscreeny < 0 || pixelscreeny >= vpixels) {
						break;
					}
					if (pixelscreenx < 0 || pixelscreenx >= hpixels) {
						break;
					}

					float z1 = (points[2].z - points[0].z) * percy + points[0].z;
					float z2 = (points[2].z - points[1].z) * percy + points[1].z;
					float percx = 0;
					if (stopx - startx != 0) {
						percx = (x * pixelsize + pixelsize / 2 - startx) / (stopx - startx);
					}
					if (percx < 0 || 1 < percx) {
						Console.WriteLine("percx is {0}", percx);
					}
					float realz = (z2 - z1) * percx + z1;
					if (realz < 1) {
					   continue;
					}
					if (result[pixelscreenx, pixelscreeny] != null) {
						if (realz < zbuf[pixelscreenx, pixelscreeny]) {
							continue;
						}
						zbuf[pixelscreenx, pixelscreeny] = realz;
					}
					result[pixelscreenx, pixelscreeny] = col;
				}
			}
		}

		private void bottri(Color col, P3D[] points) {
			if (points[2].x < points[1].x) {
				P3D _ = points[2];
				points[2] = points[1];
				points[1] = _;
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
